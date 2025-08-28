using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;

namespace NetworkInspector.Converters;

public class StringFormatter : IValueConverter
{
    private const string insertPattern = @"\{\d+\}";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
#if DEBUG
        Trace.WriteLine("Convert In: " + value);
#endif
        string formatString = parameter as string ?? string.Empty;

        if (targetType.IsAssignableTo(typeof(IEnumerable)))
        {
            IEnumerable values = (value as IEnumerable) ?? throw new ArgumentException();
            Collection<string> strings = new();

            foreach (object obj in values)
                strings.Add(string.Format(formatString, System.Convert.ToString(obj)) ?? string.Empty);
#if DEBUG
            Trace.WriteLine("Convert Out: " + strings);
#endif
            return strings;

        }

        try
        {
            string valueString = System.Convert.ToString(value) ?? string.Empty;
#if DEBUG
            Trace.WriteLine("Convert Out: " + string.Format(formatString, valueString));
#endif
            return string.Format(formatString, valueString);

        }
        catch
        {
            return new NotSupportedException();

        }


    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
#if DEBUG
        Trace.WriteLine("ConvertBack In: " + value);
#endif
        string valueString = System.Convert.ToString(value) ?? string.Empty,
               formatString = parameter as string ?? string.Empty;

        formatString = Regex.Replace(formatString, insertPattern, string.Empty);
        valueString = valueString.Replace(string.Concat(formatString.Intersect(formatString)), string.Empty);

#if DEBUG
        Trace.WriteLine("ConvertBack Out: " + valueString);
#endif
        return valueString;

    }
                

}