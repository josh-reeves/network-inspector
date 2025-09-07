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
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using NetworkInspector.Interfaces;
using NetworkInspector.Models;
using AddressRange = NetworkInspector.Models.AddressRange;

namespace NetworkInspector.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region Fields
    private const int timeout = 60,
                      bitsPerByte = 8;

    private AddressRange addressRange;
    private Ping ping;
    private CancellationTokenSource cancellationTokens;
    private bool scanIsRunning;
    SemaphoreSlim semaphore;

    #endregion

    #region Constructor(s)
    public MainWindowViewModel()
    {
        addressRange = new([192, 168, 1, 1], [192, 168, 1, 254]);
        ping = new();
        ScannedHosts = new();
        cancellationTokens = new();
        CIDRMask = CIDRMasks[23];
        semaphore = new(1, Environment.ProcessorCount > 1 ? Environment.ProcessorCount / 2 : 1);

    }

    #endregion

    #region Properties 
    public static List<int> CIDRMasks
    {
        get
        {
            List<int> results = new();

            for (int i = 1; i <= 32; i++)
                results.Add(i);

            return results;

        }

    }

    public int CIDRMask
    {
        get => addressRange.CIDRMask;

        set
        {
            if (!CIDRMasks.Contains(value))
                return;

            addressRange.CIDRMask = value;

            int addressLengthInBits = FirstAddress.GetAddressBytes().Length * bitsPerByte;

            uint subnetMaskInt = uint.MaxValue << (addressLengthInBits - addressRange.CIDRMask),
                 firstAddressInt = IPToInt(FirstAddress),
                 networkAddressInt = firstAddressInt & subnetMaskInt,
                 broadcastAddressInt = firstAddressInt | ~subnetMaskInt;

            FirstAddress = IntToIP(networkAddressInt + 1);
            LastAddress = IntToIP(broadcastAddressInt - 1);
            addressRange.SubnetMask = IntToIP(subnetMaskInt);
            addressRange.NetworkAddress = IntToIP(networkAddressInt);
            addressRange.BroadcastAddress = IntToIP(broadcastAddressInt);

            OnPropertyChanged(nameof(NetworkInformation));

        }

    }

    public bool ScanIsRunning
    {
        get => scanIsRunning;

        set
        {
            scanIsRunning = value;

            OnPropertyChanged();
        }

    }

    public string NetworkInformation
    {
        get => $"Network Information:\n" +
               $"\tFirst Address: {FirstAddress}\n" +
               $"\tLast Address: {LastAddress}\n" +
               $"\tNetwork Address: {addressRange.NetworkAddress}\n" +
               $"\tBroadcast Address: {addressRange.BroadcastAddress}\n" +
               $"\tSubnet Mask: {addressRange.SubnetMask}";

    }

    public IPAddress FirstAddress
    {
        get => addressRange.FirstAddress;
        set
        {
            addressRange.FirstAddress = value;
            addressRange.SubnetMask = null;
            addressRange.NetworkAddress = null;
            addressRange.BroadcastAddress = null;

            OnPropertyChanged();
            OnPropertyChanged(nameof(NetworkInformation));

        }

    }

    public IPAddress LastAddress
    {
        get => addressRange.LastAddress;
        set
        {
            addressRange.LastAddress = value;
            addressRange.SubnetMask = null;
            addressRange.NetworkAddress = null;
            addressRange.BroadcastAddress = null;

            OnPropertyChanged();
            OnPropertyChanged(nameof(NetworkInformation));

        }

    }

    public ObservableCollection<int> Ports
    {
        get => addressRange.Ports;
        set => addressRange.Ports = value;

    }

    public ObservableCollection<HostAddress> ScannedHosts { get; set; }

    #endregion

    #region Methods
    public void StartScan()
    {
        if (ScanIsRunning)
        {
            cancellationTokens.Cancel();

            cancellationTokens.Dispose();

            cancellationTokens = new();

            return;

        }

        ScannedHosts.Clear();

        Dispatcher.UIThread.InvokeAsync(() => Scan(cancellationTokens.Token, FirstAddress, LastAddress));

    }

    public async Task Scan(CancellationToken cancellationToken, IPAddress firstAddress, IPAddress lastAddress)
    {
        
        ScanIsRunning = true;

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

                host.Status = (await ping.SendPingAsync(host.Address, timeout)).Status;

                ScannedHosts.Add(host);

                portScanTasks.Add(PortScan(host, Ports, cancellationToken));

            }

            await Task.WhenAll(portScanTasks);

        }
        catch (Exception ex)
        {
#if DEBUG
            Trace.WriteLine(ex);
#endif
        }

        ScanIsRunning = false;
#if DEBUG
        Trace.WriteLine("Scan complete");
#endif
    }

    public async Task PortScan(IHostAddress host, IEnumerable<int> ports, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        for(int i = 0; i < ports.Count() && !cancellationToken.IsCancellationRequested; i++)
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    await tcpClient.ConnectAsync(host.Address, ports.ElementAt(i)).WaitAsync(TimeSpan.FromMilliseconds(timeout), cancellationToken);

                    host.OpenPorts.Add(ports.ElementAt(i));
                    host.Status = IPStatus.Success;

                }
                catch (Exception ex)
                {
#if DEBUG                    
                    Trace.WriteLine(ex);
#endif
                }
                finally
                {
                    tcpClient.Close();
                    tcpClient.Dispose();

                }

            }

        semaphore.Release();

    }

    private IPAddress IntToIP(uint integer) =>
        new(BitConverter.GetBytes(integer).Reverse().ToArray());

    private uint IPToInt(IPAddress address) =>
        BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

    #endregion


}
