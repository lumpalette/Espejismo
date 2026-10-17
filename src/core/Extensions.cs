using Godot;
using System;
using System.Globalization;

namespace Espejismo.Core;

/// <summary>
/// Contains extension methods for types defined in core subsystems.
/// </summary>
public static partial class Extensions
{
    // Parses a string in the format "X{sep}Y" and converts it into a Vector2.
    private static bool TryParseVector2(ReadOnlySpan<char> s, char sep, out Vector2 result)
    {
        var sepIndex = s.IndexOf(sep);

        if (sepIndex == -1)
        {
            result = Vector2.Zero;
            return false;
        }

        var xSpan = s[..sepIndex].Trim();
        var ySpan = s[(sepIndex + 1)..].Trim();

        if (!float.TryParse(xSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
            || !float.TryParse(ySpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
        {
            result = Vector2.Zero;
            return false;
        }

        result = new Vector2(x, y);
        return true;
    }

    // Parses a string and converts it into a Color. The string can be either an HTML color code or a named color.
    private static bool TryParseColor(ReadOnlySpan<char> s, out Color color)
    {
        var fallback = new Color(-1f, -1f, -1f, -1f);
        color = Color.FromString(s.ToString(), fallback);

        if (color == fallback)
        {
            color = default;
            return false;
        }

        return true;
    }
}
