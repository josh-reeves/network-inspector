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

    }

    #endregion

    #region Properties
    public IPAddress FirstAddress { get; set; }
    public IPAddress LastAddress { get; set; }

    #endregion

}
