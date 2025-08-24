using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using NetworkInspector.Models;

namespace NetworkInspector.ViewModels;

public class HostAddressViewModel : ViewModelBase
{

    #region Constructor(s)
    public HostAddressViewModel(HostAddress address)
    {
        HostAddress = address;

    }

    public HostAddress HostAddress { get; set; }

    public IPAddress Address
    {
        get => HostAddress.Address;

        set
        {
            HostAddress.Address = value;

            OnPropertyChanged();

        }

    }

    public IPStatus Status
    {
        get => HostAddress.Status;

        set
        {
            HostAddress.Status = value;

            OnPropertyChanged();

        }

    }

    public byte[] MacAddress
    {
        get => HostAddress.MacAddress;

        set
        {
            HostAddress.MacAddress = value;

            OnPropertyChanged();

        }

    }

    public ObservableCollection<int> OpenPorts
    {
        get => new(HostAddress.OpenPorts);

        set
        {
            HostAddress.OpenPorts = value.ToList();

            OnPropertyChanged();

        }

    }

    #endregion
    }