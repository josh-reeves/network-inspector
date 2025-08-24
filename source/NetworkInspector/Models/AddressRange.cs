using System.Collections.Generic;
using System.Net;

namespace NetworkInspector.Models;

public class AddressRange
{
    #region Constructor(s)
    public AddressRange(byte[] firstAddress, byte[] lastAddress)
    {
        FirstAddress = new IPAddress(
            [
                firstAddress[0],
                firstAddress[1],
                firstAddress[2],
                firstAddress[3]

            ]);

        LastAddress = new IPAddress(
            [
                lastAddress[0],
                lastAddress[1],
                lastAddress[2],
                lastAddress[3]

            ]);

        Ports = new();

    }

    #endregion

    #region Properties
    public IPAddress FirstAddress { get; set; }
    public IPAddress LastAddress { get; set; }

    public IPAddress? SubnetMask { get; set; }
    public IPAddress? NetworkAddress { get; set; }
    public IPAddress? BroadcastAddress { get; set; }

    public List<int> Ports { get; set; }
    #endregion

}
