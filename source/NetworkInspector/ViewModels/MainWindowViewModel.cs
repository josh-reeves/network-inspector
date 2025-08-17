using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using AddressRange = NetworkInspector.Models.AddressRange;

namespace NetworkInspector.ViewModels;
 
public partial class MainWindowViewModel : ViewModelBase
{
    private AddressRange addressRange;
    private Ping ping;
    private ObservableCollection<KeyValuePair<string, string>> scanResults;
    private CancellationTokenSource cancellationTokens;
    private bool scanIsRunning;

    public MainWindowViewModel()
    {
        addressRange = new([192, 168, 1, 1], [192, 168, 1, 254]);
        ping = new();
        scanResults = new();
        cancellationTokens = new();

    }

    #region Properties
    public string FirstHost
    {
        get => addressRange.FirstAddress.ToString();
        set
        {
            if (IPAddress.TryParse(value, out IPAddress? address))
                addressRange.FirstAddress = address;

            OnPropertyChanged();

        }

    }

    public string LastHost
    {
        get => addressRange.LastAddress.ToString();
        set
        {
            if (IPAddress.TryParse(value, out IPAddress? address))
                addressRange.LastAddress = address;

            OnPropertyChanged();

        }

    }

    public ObservableCollection<KeyValuePair<string, string>> ScanResults
    {
        get => scanResults;
        set
        {
            scanResults = value;
            OnPropertyChanged();

        }

    }

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

        ScanResults.Clear();

        Dispatcher.UIThread.InvokeAsync(() => Scan(cancellationTokens.Token));

        scanIsRunning = true;

    }

    public async Task Scan(CancellationToken cancellationToken)
    {
        try
        {
            byte[] firsAddressBytes = addressRange.FirstAddress.GetAddressBytes();
            byte[] lastAddressBytes = addressRange.LastAddress.GetAddressBytes();

            // GetAddressBytes return bytes in Network Byte Order, which is Big-Endian. The below reverses the arrays to account for this:
            Array.Reverse(firsAddressBytes);
            Array.Reverse(lastAddressBytes);

            uint firstAddress;
            uint lastAddress;

            if (addressRange.FirstAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                firstAddress = BitConverter.ToUInt32(firsAddressBytes, 0);
                lastAddress = BitConverter.ToUInt32(lastAddressBytes, 0);

            }
            else
                return;
                

            for (uint i = firstAddress; i <= lastAddress && !cancellationToken.IsCancellationRequested; i++)
            {
                byte[] currentIPBytes = BitConverter.GetBytes(i);

                Array.Reverse(currentIPBytes);

                IPAddress currentIP = new(currentIPBytes);

                PingReply reply = await ping.SendPingAsync(currentIP, 60);

                ScanResults.Add(new KeyValuePair<string, string>(currentIP.ToString(), reply.Status.ToString()));
#if DEBUG
                Trace.WriteLine(scanResults.Last().Key + ": " + scanResults.Last().Value);
#endif
            }

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

    #endregion


}
