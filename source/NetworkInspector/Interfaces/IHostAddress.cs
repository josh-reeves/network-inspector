using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;

namespace NetworkInspector.Interfaces;

public interface IHostAddress
{
    public IPAddress Address { get; set; }

    public IPStatus Status { get; set; }

    public byte[] MacAddress { get; set; }

    public ObservableCollection<int> OpenPorts { get; set; }
    
}