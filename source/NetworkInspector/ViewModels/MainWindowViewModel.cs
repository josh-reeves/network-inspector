using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using NetworkInspector.Models;
using AddressRange = NetworkInspector.Models.AddressRange;

namespace NetworkInspector.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region Fields
    private const int timeoutInSeconds = 60;

    private AddressRange addressRange;
    private Ping ping;
    private CancellationTokenSource cancellationTokens;
    private bool scanIsRunning;

    #endregion

    #region Constructor(s)
    public MainWindowViewModel()
    {
        addressRange = new([192, 168, 1, 1], [192, 168, 1, 254]);
        ping = new();
        ScannedHosts = new();
        cancellationTokens = new();

    }

    #endregion

    #region Properties 
    public static List<string> CIDRMasks
    {
        get
        {
            List<string> results = new();

            for (int i = 0; i <= 32; i++)
                results.Add('/' + Convert.ToString(i));

            return results;

        }

    }

    public IPAddress FirstAddress
    {
        get => addressRange.FirstAddress;
        set
        {
            addressRange.FirstAddress = value;

            OnPropertyChanged();

        }

    }

    public IPAddress LastAddress
    {
        get => addressRange.LastAddress;
        set
        {
            addressRange.LastAddress = value;

            OnPropertyChanged();

        }

    }

    public string Subnet
    {
        set
        {
            const int bitsPerByte = 8;
            const char sep = '/';

            string cidrString = value.Substring(value.IndexOf(sep) + 1);
            int cidrLength = Convert.ToInt32(cidrString);

            int addressLengthInBits = FirstAddress.GetAddressBytes().Length * bitsPerByte;

            uint subnetMaskInt = uint.MaxValue << (addressLengthInBits - cidrLength),
                 firstAddressInt = IPToInt(FirstAddress),
                 networkAddressInt = firstAddressInt & subnetMaskInt,
                 broadcastAddressInt = firstAddressInt | ~subnetMaskInt;

            FirstAddress = IntToIP(networkAddressInt + 1);
            LastAddress = IntToIP(broadcastAddressInt - 1);
            addressRange.SubnetMask = IntToIP(subnetMaskInt);
            addressRange.NetworkAddress = IntToIP(networkAddressInt);
            addressRange.BroadcastAddress = IntToIP(broadcastAddressInt);

        }

    }

    public ObservableCollection<HostAddressViewModel> ScannedHosts { get; set; }

    #endregion

    #region Methods
    public void StartScan()
    {
        if (scanIsRunning)
        {
            cancellationTokens.Cancel();

            cancellationTokens.Dispose();

            cancellationTokens = new();

            return;

        }

        ScannedHosts.Clear();

        Dispatcher.UIThread.InvokeAsync(() => Scan(cancellationTokens.Token, FirstAddress, LastAddress));

        scanIsRunning = true;

    }

    public async Task Scan(CancellationToken cancellationToken, IPAddress firstAddress, IPAddress lastAddress)
    {
        List<Task> portScanTasks = new();
        try
        {
            if (firstAddress.AddressFamily != AddressFamily.InterNetwork)
                return;

            uint firstAddressInt = IPToInt(firstAddress),
                 lastAddressInt = IPToInt(lastAddress);

            for (uint i = firstAddressInt; i <= lastAddressInt && !cancellationToken.IsCancellationRequested; i++)
            {
                HostAddress host = new(IntToIP(i));

                host.Status = (await ping.SendPingAsync(host.Address, timeoutInSeconds)).Status;

                ScannedHosts.Add(new(host));

                portScanTasks.Add(PortScan(ScannedHosts.Last().HostAddress, cancellationToken));

            }

            await Task.WhenAll(portScanTasks);
 
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);

        }

        scanIsRunning = false;

#if DEBUG
        Trace.WriteLine("Scan complete");
#endif
    }

    public async Task PortScan(HostAddress host, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    if (tcpClient.ConnectAsync(host.Address, 80).Wait(timeoutInSeconds, cancellationToken))
                        Trace.WriteLine("open");

                }
                catch (Exception ex)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                    Trace.WriteLine(ex);

                }

            }

        },
        cancellationToken);

    }

    private IPAddress IntToIP(uint integer) =>
        new(BitConverter.GetBytes(integer).Reverse().ToArray());

    private uint IPToInt(IPAddress address) =>
        BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

    #endregion


}
