using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Microsoft.VisualBasic;

namespace NetworkInspector.Converters;

public class PortListConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IList collection)
            return new NotSupportedException();

        if (targetType.IsAssignableTo(typeof(string)))
        {
            string result = string.Empty,
                   separator = parameter as string ?? string.Empty;

            for (int i = 0; i < collection.Count; i++)
            {
                object? obj = collection[i];
                result += obj ?? string.Empty;

                while ((System.Convert.ToInt32(collection[i + 1]) == System.Convert.ToInt32(collection[1]) + 1 ||
                        System.Convert.ToInt32(collection[i + 1]) == System.Convert.ToInt32(collection[1]) - 1) &&
                        i < collection.Count)
                    i++;

                if (collection[i] != obj)
                    result += " - ";
                else
                    result += separator;

            }
            
            return result;

        }

        return new NotSupportedException();

    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string collectionString = value as string ?? string.Empty,
               sep = parameter as string ?? string.Empty;

        if (targetType.IsAssignableTo(typeof(IList)))
        {
            ObservableCollection<int> collection = new();

            foreach (string str in collectionString.Split(sep))
            {
                if (str.Contains("-"))
                {
                    string[] range = str.Split('-');

                    int start = int.TryParse(range.First().Trim(), out start) ? start : 0,
                        end = int.TryParse(range.Last().Trim(), out end) ? end : 0;

                    while (start <= end)
                    {
                        collection.Add(System.Convert.ToInt32(start));
                        start++;

                    }

                }
                else if (int.TryParse(str.Trim(), out int integer))
                    collection.Add(System.Convert.ToInt32(integer));

            }

            return collection;

        }

        return new NotSupportedException();

    }
                
}