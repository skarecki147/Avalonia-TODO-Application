using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TodoApp.Shared.Converters;

/// <summary>MultiValue: (SelectedNav, IsDarkTheme), Parameter: target nav name. Returns Foreground brush for nav button.</summary>
public class NavActiveForegroundConverter : IMultiValueConverter
{
    private static readonly SolidColorBrush White = new(Colors.White);
    private static readonly SolidColorBrush DarkInactive = new(Color.Parse("#c0c8d8"));
    private static readonly SolidColorBrush LightInactive = new(Color.Parse("#1a1a2e"));

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not { Count: >= 1 } || parameter is not string target)
            return White;

        var selected = values[0]?.ToString() ?? "";
        var isDark = values.Count < 2 || values[1] is true;
        var active = selected == target;

        if (active)
            return White;
        return isDark ? DarkInactive : LightInactive;
    }
}
