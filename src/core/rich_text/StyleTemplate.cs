using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a set of style properties that serves as a template for creating <see cref="TextStyle"/> instances.
/// </summary>
[GlobalClass, Tool]
public partial class StyleTemplate : TextResource
{
    private StyleTemplate()
    {
    }

    /// <summary>
    /// Gets the font family used for the text. This property is required.
    /// </summary>
    /// <value>The default is <see langword="null"/>.</value>
    /// <remarks>
    /// The actual <see cref="Godot.Font"/> used for the text is determined by <see cref="FontStyle"/>.
    /// </remarks>
    [ExportGroup("Typography")]
    [Export, NotNull]
    public FontFamily? Font
    {
        get;
        private set
        {
            if (field != value)
            {
                field?.Changed -= EmitChanged;
                field = value;
                field?.Changed += EmitChanged;

                EmitChanged();
            }
        }
    }

    /// <summary>
    /// Gets the style variant used to select the font from the family.
    /// </summary>
    /// <value>The default is <see cref="FontStyle.None"/>.</value>
    [Export]
    public FontStyle FontStyle { get; private set => SetAndNotify(ref field, value); }

    /// <summary>
    /// Gets the size of the text, in pixels.
    /// </summary>
    /// <value>The default is 8px.</value>
    [Export(PropertyHint.Range, "1,512,suffix:px")]
    public ushort FontSize { get; private set => SetAndNotify(ref field, value); } = 8;

    /// <summary>
    /// Gets the color tint of the text.
    /// </summary>
    /// <value>The default is white.</value>
    [Export]
    public Color Color { get; private set => SetAndNotify(ref field, value); } = Colors.White;

    /// <summary>
    /// Gets the additional space added between letters (X) and lines of text (Y), in pixels.
    /// </summary>
    /// <value>The default is <c>(0px,0px)</c>.</value>
    [Export(hintString: "suffix:px")]
    public Vector2I Spacing { get; private set => SetAndNotify(ref field, value); }

    /// <summary>
    /// Gets the visual effect applied to the text.
    /// </summary>
    /// <value>The default is <see langword="null"/> (no effect).</value>
    [ExportGroup("Effects")]
    [Export]
    public TextEffect? Effect { get; private set => SetAndNotify(ref field, value); }

    /// <summary>
    /// Gets the size for the shadow effect, in pixels.
    /// </summary>
    /// <value>The default is 0px.</value>
    [ExportGroup("Shadow", prefix: "Shadow")]
    [Export(PropertyHint.Range, "0,512,suffix:px")]
    public ushort ShadowSize { get; private set => SetAndNotify(ref field, value); }

    /// <summary>
    /// Gets the color for the shadow effect.
    /// </summary>
    /// <value>The default is black.</value>
    [Export]
    public Color ShadowColor { get; private set => SetAndNotify(ref field, value); } = Colors.Black;

    /// <summary>
    /// Gets the displacement for the shadow effect, relative to the main text, in pixels.
    /// </summary>
    /// <value>The default is <c>(1px,1px)</c>.</value>
    [Export(hintString: "suffix:px")]
    public Vector2 ShadowOffset { get; private set => SetAndNotify(ref field, value); } = Vector2.One;

    /// <summary>
    /// Gets the size for the text outline, in pixels.
    /// </summary>
    /// <value>The default is 0px.</value>
    [ExportGroup("Outline", prefix: "Outline")]
    [Export(PropertyHint.Range, "0,512,suffix:px")]
    public ushort OutlineSize { get; private set => SetAndNotify(ref field, value); }

    /// <summary>
    /// Gets the color for the text outline.
    /// </summary>
    /// <value>The default is black.</value>
    [Export]
    public Color OutlineColor { get; private set => SetAndNotify(ref field, value); } = Colors.Black;

    /// <summary>
    /// Creates a new <see cref="TextStyle"/> based on the data from this template.
    /// </summary>
    /// <returns>A <see cref="TextStyle"/> with every property set.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Font"/> is <see langword="null"/>.</exception>
    public TextStyle Create()
    {
        ValidateState();

        return new TextStyle
        {
            Font = Font,
            FontSize = FontSize,
            FontStyle = FontStyle,
            Color = Color,
            Effect = Effect,
            Spacing = Spacing,
            ShadowSize = ShadowSize,
            ShadowColor = ShadowColor,
            ShadowOffset = ShadowOffset,
            OutlineSize = OutlineSize,
            OutlineColor = OutlineColor
        };
    }

    /// <summary>
    /// Creates a new <see cref="TextStyle"/> by filling the unset properties of the specified style with those from
    /// this template.
    /// </summary>
    /// <param name="overrides">The style whose set properties take precedence over the template's.</param>
    /// <returns>
    /// A <see cref="TextStyle"/> that takes each property from <paramref name="overrides"/> if it is set, otherwise,
    /// from this template.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Font"/> is <see langword="null"/>.</exception>
    public TextStyle Create(in TextStyle overrides)
    {
        var style = Create();
        return overrides.MergedWith(style);
    }

    private void ValidateState()
    {
        if (Font is null)
        {
            throw new InvalidOperationException("Template's font was not specified from the editor");
        }
    }
}
