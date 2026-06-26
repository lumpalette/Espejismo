using Godot;
 
namespace Spectrum.RichText;
 
/// <summary>
/// Represents a range of <see cref="TextRun"/> instances that share the same text alignment.
/// </summary>
/// <param name="start">
/// The index of the starting run.
/// </param>
/// <param name="length">
/// The number of runs in the block.
/// </param>
/// <param name="alignment">
/// The alignment applied to the block.
/// </param>
public readonly struct AlignmentBlock(int start, int length, HorizontalAlignment alignment)
{
	/// <summary>
	/// Gets the index of the first text run in the block, relative to <see cref="ParsedText.Runs"/>.
	/// </summary>
	public int Start { get; } = start;
 
	/// <summary>
	/// Gets the number of text runs in the block.
	/// </summary>
	public int Length { get; } = length;
 
	/// <summary>
	/// Gets the alignment shared by every text run in the block.
	/// </summary>
	public HorizontalAlignment Alignment { get; } = alignment;
}
