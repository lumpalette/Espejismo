using Godot;
using System.Diagnostics.CodeAnalysis;

namespace Espejismo.Core.RichText;

/// <summary>
///   Represents a group of <see cref="Font"/> resources that share a common design.
/// </summary>
[GlobalClass]
public partial class FontFamily : Resource
{
	/// <summary>
	///   Gets the regular (upright, normal weight) font resource of the family.
	/// </summary>
	[NotNull]
	[Export] public Font? Regular { get; private set; }

	/// <summary>
	///   Gets the bold font resource of the family, if provided.
	/// </summary>
	[Export] public Font? Bold { get; private set; }

	/// <summary>
	///   Gets the italic font resource of the family, if provided.
	/// </summary>
	[Export] public Font? Italic { get; private set; }

	/// <summary>
	///   Gets the bold-italic font resource of the family, if provided.
	/// </summary>
	[Export] public Font? BoldItalic { get; private set; }

	/// <summary>
	///   Gets the <see cref="Font"/> that best matches the specified <see cref="FontStyle"/>, synthesizing a faux
	///   variant if the dedicated resource is not available.
	/// </summary>
	/// <param name="style">
	///   Identifier for the font variant to get.
	/// </param>
	/// <returns>
	///   The matching <see cref="Font"/> resource.
	/// </returns>
	public Font GetVariant(FontStyle style)
	{
		var bold = style.HasFlag(FontStyle.Bold);
		var italic = style.HasFlag(FontStyle.Italic);

		var font = (bold, italic) switch
		{
			(true, true)  => BoldItalic,
			(true, false) => Bold,
			(false, true) => Italic,
			_             => Regular
		};

		if (font is not null)
		{
			return font;
		}

		return CreateFaux(bold, italic);
	}

	private FontVariation CreateFaux(bool bold, bool italic)
	{
		var variation = new FontVariation
		{
			BaseFont = ((bold, italic) == (true, true)) ? (Bold ?? Italic ?? Regular) : Regular
		};

		if (bold && Bold is null)
		{
			variation.VariationEmbolden = TextConfig.FauxBoldThickness;
		}

		if (italic && Italic is null)
		{
			variation.VariationTransform = new Transform2D(1f, TextConfig.FauxItalicSlant, 0f, 1f, 0f, 0f);
		}

		return variation;
	}
}
