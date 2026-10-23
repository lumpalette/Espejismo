using Godot;
using System;
using System.IO;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides static, read-only access to the global configuration used by the rich-text system.
/// </summary>
/// <remarks>
/// The configuration is loaded from <see cref="Path"/> on first access. Accessing any static member throws an
/// exception if the file is missing, the default style is not set, or no regular font is assigned.
/// </remarks>
[GlobalClass, Tool]
public partial class TextConfig : TextResource
{
    /// <summary>
    /// The resource path where the configuration resource file is located.
    /// </summary>
    public const string Path = "res://src/core/rich_text/config.tres";

    [ExportGroup("Defaults")]
    [Export]
    private TextStyleTemplate? _fallbackStyle;
    [Export]
    private float _fauxBoldThickness;
    [Export]
    private float _fauxItalicSlant;

    [ExportGroup("Markup")]
    [Export]
    private Godot.Collections.Dictionary<string, TextTag> _tags = [];

    [ExportGroup("Resources")]
    [Export]
    private Godot.Collections.Dictionary<string, TextStyleTemplate> _styles = [];
    [Export]
    private Godot.Collections.Dictionary<string, FontFamily> _fonts = [];
    [Export]
    private Godot.Collections.Dictionary<string, Texture2D> _icons = [];

    /// <summary>
    /// Gets the style that provides the default values at the bottom of the cascade.
    /// </summary>
    public static TextStyleTemplate FallbackStyle => Active._fallbackStyle!;

    /// <summary>
    /// Gets the stroke thickness used to synthesize bold text when a font lacks a dedicated bold variant, in pixels.
    /// </summary>
    public static float FauxBoldThickness => Active._fauxBoldThickness;

    /// <summary>
    /// Gets the horizontal skew factor used to synthesize italic text when a font lacks a dedicated italic variant.
    /// </summary>
    public static float FauxItalicSlant => Active._fauxItalicSlant;

    /// <summary>
    /// Gets a map containing the <see cref="TextTag"/> instances available when parsing text.
    /// </summary>
    public static ResourceMap<TextTag> Tags
    {
        get
        {
#if TOOLS
            return new ResourceMap<TextTag>(Active._tags);
#else
            field ??= new ResourceMap<TextTag>(Active._tags);
            return field;
#endif
        }
    }

    /// <summary>
    /// Gets a map containing the <see cref="TextStyleTemplate"/> instances defined in the project.
    /// </summary>
    public static ResourceMap<TextStyleTemplate> Styles
    {
        get
        {
#if TOOLS
            return new ResourceMap<TextStyleTemplate>(Active._styles);
#else
            field ??= new ResourceMap<TextStyle>(Active._styles);
            return field;
#endif
        }
    }

    /// <summary>
    /// Gets a map containing the <see cref="FontFamily"/> instances defined in the project.
    /// </summary>
    public static ResourceMap<FontFamily> Fonts
    {
        get
        {
#if TOOLS
            return new ResourceMap<FontFamily>(Active._fonts);
#else
            field ??= new ResourceMap<FontFamily>(Active._fonts);
            return field;
#endif
        }
    }

    /// <summary>
    /// Gets a map containing the <see cref="Texture2D"/> instances that can be used as text icons in the system.
    /// </summary>
    public static ResourceMap<Texture2D> Icons
    {
        get
        {
#if TOOLS
            return new ResourceMap<Texture2D>(Active._icons);
#else
            field ??= new ResourceMap<Texture2D>(Active._icons);
            return field;
#endif
        }
    }

    private static TextConfig Active
    {
        get
        {
#if TOOLS
            return LoadFromPath();
#else
            field ??= LoadFromPath();
            return field;
#endif
        }
    }

    private static TextConfig LoadFromPath()
    {
        if (!ResourceLoader.Exists(Path))
        {
            throw new FileNotFoundException(
                $"Failed to load TextConfig: No resource found at path '{Path}'.", Path);
        }

        var config = GD.Load<TextConfig>(Path);

        if (config._fallbackStyle is null)
        {
            throw new InvalidOperationException(
                $"Failed to load TextConfig at '{Path}': The 'Default Style' slot in the inspector is empty.");
        }

        if (!config._fallbackStyle.Value.IsComplete)
        {
            throw new InvalidOperationException(
                $"Failed to load TextConfig at '{Path}': The assigned 'Default Style' has missing properties.");
        }

        return config;
    }
}
