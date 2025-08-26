using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace NetworkInspector.Converters;

public class PortListConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ObservableCollection<int> collection)
            return new ArgumentException();

        if (targetType.IsAssignableTo(typeof(string)))
        {
            string result = string.Empty,
                   sep = parameter as string ?? string.Empty;

            foreach (int integer in collection)
            {
                if (collection.IndexOf(integer) == (collection.Count - 1))
                    result += integer.ToString();
                else
                    result += integer + sep;

            }

            return result;

        }

        return new NotSupportedException();

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string collectionString = value as string ?? string.Empty;

        if (targetType.IsAssignableTo(typeof(ObservableCollection<int>)))
        {
            string sep = parameter as string ?? string.Empty;

            ObservableCollection<int> collection = new();

            foreach (string str in collectionString.Split(sep))
            {
                str.Trim().Replace(sep, string.Empty);

                if (int.TryParse(str.Trim(), out int integer))
                    collection.Add(System.Convert.ToInt32(integer));

            }

            return collection;

        }

        return new NotSupportedException();

    }
                

}