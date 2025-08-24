using System;
using System.Globalization;
using System.Net;
using Avalonia.Data.Converters;

namespace NetworkInspector.Converters;

public class IPAddressConverter : IValueConverter
{
    public static readonly IPAddressConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        IPAddress? address;

        if (targetType.IsAssignableTo(typeof(string)))
        {
            IPAddress.TryParse(value?.ToString(), out address);

            return address?.ToString() ?? value;

        }

        if (targetType.IsAssignableTo(typeof(IPAddress)))
        {
            IPAddress.TryParse(value?.ToString(), out address);

            return address;

        }

        return new NotSupportedException();

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        IPAddress? address;

        if (!targetType.IsAssignableTo(typeof(IPAddress)))
            return new NotSupportedException();

        IPAddress.TryParse(value?.ToString(), out address);

        return address;

    }

}