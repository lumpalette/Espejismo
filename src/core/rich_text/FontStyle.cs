using System;

namespace Espejismo.Core.RichText;

/// <summary>
///   Specifies the style variants of a text font.
/// </summary>
[Flags]
public enum FontStyle
{
	/// <summary>
	///   Normal text.
	/// </summary>
#pragma warning disable CA1008 // Enums should have zero value
	Regular = 0,
#pragma warning restore CA1008 // Enums should have zero value

	/// <summary>
	///   Bold text.
	/// </summary>
	Bold = 1,

	/// <summary>
	///   Italic text.
	/// </summary>
	Italic = 2
}
