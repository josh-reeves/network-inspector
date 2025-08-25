using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace NetworkInspector.Converters;

public class ListConverter : IValueConverter
{
    public static readonly ListConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ObservableCollection<int> list)
            return new ArgumentException();

        if (targetType.IsAssignableTo(typeof(string)))
        {
            string result = string.Empty,
                   sep = parameter as string ?? string.Empty;

            foreach (int item in list)
            {
                if (list.IndexOf(item) == (list.Count - 1))
                    result += item.ToString()?.Trim();
                else
                    result += item + sep;

            }

            return result;

        }

        return new NotSupportedException();

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string collectionString)
            return new ArgumentException();

        if (targetType.IsAssignableTo(typeof(IList)))
        {
            string sep = parameter as string ?? string.Empty;

            ObservableCollection<int> collection = new();

            foreach (string str in collectionString.Split(sep))
                if (int.TryParse(str, out int integer))
                    collection.Add(System.Convert.ToInt32(integer));

            return collection;

        }

        return new NotSupportedException();

    }
                

}