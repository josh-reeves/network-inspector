using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace NetworkInspector.Converters;

public class ListConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IList list)
            return new ArgumentException();

        if (targetType.IsAssignableTo(typeof(string)))
        {
            string result = string.Empty,
                   sep = parameter as string ?? string.Empty;

            foreach (object item in list)
            {
                if (list.IndexOf(item) == (list.Count - 1))
                    result += item.ToString()?.Trim();
                else
                    result += item + sep;

            }

        }

        return new NotSupportedException();

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        return new NotSupportedException();

    }
                

}