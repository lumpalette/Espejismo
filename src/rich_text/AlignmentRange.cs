using Godot;
 
namespace Spectrum.RichText;
 
/// <summary>
///   Represents a range within a sequence of <see cref="TextRun"/> and <see cref="InlineIcon"/> instances that shares
///   the same horizontal alignment.
/// </summary>
/// <param name="start">
///   The inclusive start index of the range.
/// </param>
/// <param name="end">
///   The exclusive end index of the range.
/// </param>
/// <param name="alignment">
///   The alignment applied to the range.
/// </param>
public readonly struct AlignmentRange(int start, int end, HorizontalAlignment alignment)
{
	/// <summary>
	///   Gets the inclusive start index of the range within the sequence.
	/// </summary>
	public int Start { get; } = start;
 
	/// <summary>
	///   Gets the exclusive end index of the range within the sequence.
	/// </summary>
	public int End { get; } = end;
 
	/// <summary>
	///   Gets the alignment shared by every run or icon in the range.
	/// </summary>
	public HorizontalAlignment Alignment { get; } = alignment;
}
