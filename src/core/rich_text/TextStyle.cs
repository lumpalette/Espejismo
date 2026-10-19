using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that describe how text is rendered.
/// </summary>
[GlobalClass, Tool]
public sealed partial class TextStyle : TextResource, IEquatable<TextStyle>
{
    private static readonly Property[] s_propertyTable =
    [
        #region Typography
        new()
        {
            Name = "font",
            Type = Variant.Type.Object,
            Default = default,
            Get = static (s) => ToVariant(s._font),
            Set = static (s, v) => s.SetResource(ref s._font, v.As<FontFamily?>()),
            Has = static (s) => s._font is not null,
            Hint = PropertyHint.ResourceType,
            HintString = nameof(FontFamily),
            IsCheckable = false
        },
        new()
        {
            Name = "font_style",
            Type = Variant.Type.Int,
            Default = (int)RichText.FontStyle.None,
            Get = static (s) => ToVariant(s._fontStyle),
            Set = static (s, v) => s._fontStyle = (v.VariantType != Variant.Type.Nil) ? v.As<FontStyle>() : null,
            Has = static (s) => s._fontStyle is not null,
            Hint = PropertyHint.Flags,
            HintString = "Bold,Italic"
        },
        new()
        {
            Name = "font_size",
            Type = Variant.Type.Int,
            Default = 8,
            Get = static (s) => ToVariant(s._fontSize),
            Set = static (s, v) => s._fontSize = AsNullable<ushort>(v),
            Has = static (s) => s._fontSize is not null,
            Hint = PropertyHint.Range,
            HintString = "1,256,suffix:px"
        },
        new()
        {
            Name = "color",
            Type = Variant.Type.Color,
            Default = Colors.White,
            Get = static (s) => ToVariant(s._color),
            Set = static (s, v) => s._color = AsNullable<Color>(v),
            Has = static (s) => s._color is not null
        },
        new()
        {
            Name = "spacing",
            Type = Variant.Type.Vector2I,
            Default = Vector2I.Zero,
            Get = static (s) => ToVariant(s._spacing),
            Set = static (s, v) => s._spacing = AsNullable<Vector2I>(v),
            Has = static (s) => s._spacing is not null,
            HintString = "suffix:px"
        },
        #endregion

        #region Effects
        new()
        {
            Name = "effect",
            Type = Variant.Type.Object,
            Default = default,
            Get = static (s) => ToVariant(s._effect),
            Set = static (s, v) => s.SetResource(ref s._effect, v.As<TextEffect?>()),
            Has = static (s) => s._effect is not null,
            Hint = PropertyHint.ResourceType,
            HintString = nameof(TextEffect),
            IsCheckable = false
        },
        #endregion

        #region Shadow
        new()
        {
            Name = "shadow_size",
            Type = Variant.Type.Int,
            Default = 0,
            Get = static (s) => ToVariant(s._shadowSize),
            Set = static (s, v) => s._shadowSize = AsNullable<ushort>(v),
            Has = static (s) => s._shadowSize is not null,
            Hint = PropertyHint.Range,
            HintString = "0,256,suffix:px"
        },
        new()
        {
            Name = "shadow_color",
            Type = Variant.Type.Color,
            Default = Colors.Transparent,
            Get = static (s) => ToVariant(s._shadowColor),
            Set = static (s, v) => s._shadowColor = AsNullable<Color>(v),
            Has = static (s) => s._shadowColor is not null
        },
        new()
        {
            Name = "shadow_offset",
            Type = Variant.Type.Vector2,
            Default = Vector2.One,
            Get = static (s) => ToVariant(s._shadowOffset),
            Set = static (s, v) => s._shadowOffset = AsNullable<Vector2>(v),
            Has = static (s) => s._shadowOffset is not null,
            HintString = "suffix:px"
        },
        #endregion

        #region Outline
        new()
        {
            Name = "outline_size",
            Type = Variant.Type.Int,
            Default = 0,
            Get = static (s) => ToVariant(s._outlineSize),
            Set = static (s, v) => s._outlineSize = AsNullable<ushort>(v),
            Has = static (s) => s._outlineSize is not null,
            Hint = PropertyHint.Range,
            HintString = "0,256,suffix:px"
        },
        new()
        {
            Name = "outline_color",
            Type = Variant.Type.Color,
            Default = Colors.Transparent,
            Get = static (s) => ToVariant(s._outlineColor),
            Set = static (s, v) => s._outlineColor = AsNullable<Color>(v),
            Has = static (s) => s._outlineColor is not null
        },
        #endregion
    ];

    /// <summary>
    /// Gets a style with all its properties unset, which can be used to cascade back to the default style.
    /// </summary>
    public static TextStyle Default { get; } = new();

    /// <summary>
    /// Gets the font family used for the text.
    /// </summary>
    /// <remarks>
    /// The actual <see cref="Godot.Font"/> used for the text is determined by <see cref="FontStyle"/>.
    /// </remarks>
    public FontFamily? Font { get => _font; init => _font = value; }

    private FontFamily? _font;

    /// <summary>
    /// Gets the style variant used to select the font from the family.
    /// </summary>
    public FontStyle? FontStyle { get => _fontStyle; init => _fontStyle = value; }

    private FontStyle? _fontStyle;

    /// <summary>
    /// Gets the size of the text, in pixels.
    /// </summary>
    public ushort? FontSize { get => _fontSize; init => _fontSize = value; }

    private ushort? _fontSize;

    /// <summary>
    /// Gets the color tint of the text.
    /// </summary>
    public Color? Color { get => _color; init => _color = value; }

    private Color? _color;

    /// <summary>
    /// Gets the additional space added between letters (X) and lines of text (Y), in pixels.
    /// </summary>
    public Vector2I? Spacing { get => _spacing; init => _spacing = value; }

    private Vector2I? _spacing;

    /// <summary>
    /// Gets the visual effect applied to the text.
    /// </summary>
    public TextEffect? Effect { get => _effect; init => _effect = value; }

    private TextEffect? _effect;

    /// <summary>
    /// Gets the size of the shadow effect, in pixels.
    /// </summary>
    public ushort? ShadowSize { get => _shadowSize; init => _shadowSize = value; }

    private ushort? _shadowSize;

    /// <summary>
    /// Gets the color for the shadow effect.
    /// </summary>
    public Color? ShadowColor { get => _shadowColor; init => _shadowColor = value; }

    private Color? _shadowColor;

    /// <summary>
    /// Gets the displacement for the shadow effect, relative to the main text.
    /// </summary>
    public Vector2? ShadowOffset { get => _shadowOffset; init => _shadowOffset = value; }

    private Vector2? _shadowOffset;

    /// <summary>
    /// Gets the size for the text outline, in pixels.
    /// </summary>
    public ushort? OutlineSize { get => _outlineSize; init => _outlineSize = value; }

    private ushort? _outlineSize;

    /// <summary>
    /// Gets the color for the text outline.
    /// </summary>
    public Color? OutlineColor { get => _outlineColor; init => _outlineColor = value; }

    private Color? _outlineColor;

    /// <summary>
    /// Gets a value that indicates whether every property of the style is set.
    /// </summary>
    [MemberNotNullWhen(true,
        nameof(Font),
        nameof(FontStyle),
        nameof(FontSize),
        nameof(Color),
        nameof(Spacing),
        nameof(Effect),
        nameof(ShadowSize),
        nameof(ShadowColor),
        nameof(ShadowOffset),
        nameof(OutlineSize),
        nameof(OutlineColor))]
    public bool IsComplete
    {
        get
        {
            foreach (var prop in s_propertyTable)
            {
                if (!prop.Has(this))
                {
                    return false;
                }
            }

#pragma warning disable CS8775 // Member must have a non-null value when exiting in some condition.
            return true;
#pragma warning restore CS8775 // Member must have a non-null value when exiting in some condition.
        }
    }

    /// <summary>
    /// Determines whether the style is equal to the specified <see cref="TextStyle"/>.
    /// </summary>
    /// <param name="other">The style to compare with.</param>
    /// <returns>
    /// <see langword="true"/> if the style and <paramref name="other"/> are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals([NotNullWhen(true)] TextStyle? other)
    {
        if (other is null)
        {
            return false;
        }

        // what the fuck is this
        // TODO: consider if it's worth doing this vs using the property table
        return (_font == other._font)
            && (_fontStyle == other._fontStyle)
            && (_fontSize == other._fontSize)
            && (_color == other._color)
            && (_spacing == other._spacing)
            && (_effect == other._effect)
            && (_shadowSize == other._shadowSize)
            && (_shadowColor == other._shadowColor)
            && (_shadowOffset == other._shadowOffset)
            && (_outlineSize == other._outlineSize)
            && (_outlineColor == other._outlineColor);
    }

    /// <summary>
    /// Determines whether the style is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="TextStyle"/> and is equal; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals(obj as TextStyle);
    }

    /// <summary>
    /// Serves as the hash function for <see cref="TextStyle"/>.
    /// </summary>
    /// <returns>The hash code for this style.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(_font);
        hash.Add(_fontStyle);
        hash.Add(_fontSize);
        hash.Add(_color);
        hash.Add(_effect);
        hash.Add(_shadowSize);
        hash.Add(_shadowColor);
        hash.Add(_shadowOffset);
        hash.Add(_outlineSize);
        hash.Add(_outlineColor);

        return hash.ToHashCode();
    }

    private static Variant ToVariant<[MustBeVariant] T>(T? value)
    {
        return (value is { } v) ? Variant.From(v) : default;
    }

    private static T? AsNullable<[MustBeVariant] T>(Variant v) where T : struct
    {
        return (v.VariantType != Variant.Type.Nil) ? v.As<T>() : null;
    }

    private void SetResource<T>(ref T? field, T? value) where T : Resource
    {
        if (ReferenceEquals(field, value))
        {
            return;
        }

        field?.Changed -= EmitChanged;
        field = value;
        field?.Changed += EmitChanged;
    }

    // Record used to expose a property from the class to the inspector. See s_properties for its usage.
    private sealed record class Property
    {
        // How the property is serialized as a .tres.
        public required string Name { get; init; }

        public required Variant.Type Type { get; init; }

        // The default value that shows up in the inspector when the resource is created.
        public required Variant Default { get; init; }

        // A function to retrieve the value of the property during serialization.
        public required Func<TextStyle, Variant> Get { get; init; }

        // Same as above, but for setting it during deserialization.
        public required Action<TextStyle, Variant> Set { get; init; }

        // Used to determine if a property currently has a value assigned.
        public required Predicate<TextStyle> Has { get; init; }

        public PropertyHint Hint { get; init; }

        public string HintString { get; init; } = string.Empty;

        // Whether a checkbox appears next to the property to indicate if the property is unset or not.
        public bool IsCheckable { get; init; } = true;
    }
}
