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
	[Export, NotNull]
	public Font? Normal { get; private set; }

	/// <summary>
	///   Gets the bold font resource of the family, if provided.
	/// </summary>
	[Export]
	public Font? Bold { get; private set; }

	/// <summary>
	///   Gets the italic font resource of the family, if provided.
	/// </summary>
	[Export]
	public Font? Italic { get; private set; }

	/// <summary>
	///   Gets the bold-italic font resource of the family, if provided.
	/// </summary>
	[Export]
	public Font? BoldItalic { get; private set; }
}
