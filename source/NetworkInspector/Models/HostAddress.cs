using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkInspector.Models;

public class HostAddress
{
    #region Constructor(s)
    public HostAddress(IPAddress address, IPStatus status = IPStatus.Unknown)
    {
        Address = address;
        Status = status;
        MacAddress = new byte[6];
        OpenPorts = new();

    }

    #endregion

    public IPAddress Address { get; set; }

    public IPStatus Status { get; set; }

    public byte[] MacAddress { get; set; }

    public ObservableCollection<int> OpenPorts { get; set; }
    
}