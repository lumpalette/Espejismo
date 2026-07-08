using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Spectrum.RichText;

/// <summary>
///   Provides an interface to query and mutate the state of a rich-text parsing operation.
/// </summary>
public class ParseContext
{
	private readonly Stack<StyleOverride> _styleStack = [];
	private readonly List<TextRun> _currentRuns = [];
	private readonly List<InlineIcon> _currentIcons = [];
	private readonly List<InlineCommand> _currentCommands = [];
	private readonly List<AlignmentRange> _currentAlignments = [];
	private readonly Stack<HorizontalAlignment> _alignmentStack = [];

	private int _textPosition;
	private int _sequenceIndex;
	private int _alignStart;

	/// <summary>
	///   Gets the style currently at the top of the style stack.
	/// </summary>
	/// <remarks>
	///   If the style stack is empty, the <see langword="default"/> value for <see cref="StyleOverride"/> is returned.
	/// </remarks>
	public StyleOverride TopStyle
	{
		get
		{
			_styleStack.TryPeek(out StyleOverride top);
			return top;
		}
	}

	/// <summary>
	///   Gets the <see cref="TextRun"/> instances appended so far.
	/// </summary>
	/// <remarks>
	///   The returned span becomes invalid when a new run is added.
	/// </remarks>
	public ReadOnlySpan<TextRun> Runs => CollectionsMarshal.AsSpan(_currentRuns);

	/// <summary>
	///   Gets the <see cref="InlineIcon"/> instances appended so far.
	/// </summary>
	/// <remarks>
	///   The returned span becomes invalid when a new icon is added.
	/// </remarks>
	public ReadOnlySpan<InlineIcon> Icons => CollectionsMarshal.AsSpan(_currentIcons);

	/// <summary>
	///   Gets the <see cref="InlineCommand"/> instances appended so far.
	/// </summary>
	/// <remarks>
	///   The returned span becomes invalid when a new command is added.
	/// </remarks>
	public ReadOnlySpan<InlineCommand> Commands => CollectionsMarshal.AsSpan(_currentCommands);

	/// <summary>
	///   Gets the <see cref="AlignmentRange"/> instances appended so far.
	/// </summary>
	/// <remarks>
	///   The returned span becomes invalid when a new range begins.
	/// </remarks>
	public ReadOnlySpan<AlignmentRange> AlignmentRanges => CollectionsMarshal.AsSpan(_currentAlignments);

	/// <summary>
	///   Adds a <see cref="StyleOverride"/> at the top of the stack.
	/// </summary>
	/// <param name="style">
	///   The style to push onto the stack.
	/// </param>
	public void PushStyle(StyleOverride style)
	{
		_styleStack.Push(style);
	}

	/// <summary>
	///   Removes the style at the top of the style stack.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///   Thrown if the style stack is empty.
	/// </exception>
	public void PopStyle()
	{
		if (_styleStack.Count == 0)
		{
			throw new InvalidOperationException("Style stack is empty");
		}

		_styleStack.Pop();
	}
	
	/// <summary>
	///   Creates and appends a new <see cref="TextRun"/> with the specified text and the current
	///   <see cref="TopStyle"/>.
	/// </summary>
	/// <param name="text">
	///   The characters in the run.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="text"/> is <see langword="null"/>.
	/// </exception>
	public void AppendText(string text)
	{
		ArgumentNullException.ThrowIfNull(text, nameof(text));

		_currentRuns.Add(new TextRun(_sequenceIndex, text, TopStyle));
		
		_textPosition += text.Length;
		_sequenceIndex++;
	}

	/// <summary>
	///   Creates a new <see cref="InlineCommand"/> with the specified name and properties and inserts it at the
	///   current text position.
	/// </summary>
	/// <param name="name">
	///   The name of the command.
	/// </param>
	/// <param name="properties">
	///   The properties passed to the command.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="name"/> is <see langword="null"/>.
	/// </exception>
	public void AppendCommand(string name, ReadOnlySpan<TagProperty> properties)
	{
		ArgumentNullException.ThrowIfNull(name, nameof(name));

		// Commands are consumed after the text is shaped, which outlives the source document (ref struct),
		// so we need to store the properties in the heap.
		_currentCommands.Add(new InlineCommand(name, properties.ToArray(), _textPosition));
	}

	/// <summary>
	///   Begins a new <see cref="AlignmentRange"/> at the current text position.
	/// </summary>
	/// <param name="alignment">
	///   The alignment to apply.
	/// </param>
	public void BeginAlignment(HorizontalAlignment alignment)
	{
		// Always push even when the previous alignment is the same, because it would break end tags otherwise.
		FlushCurrentAlignment();
		_alignmentStack.Push(alignment);
	}

	/// <summary>
	///   Ends the current <see cref="AlignmentRange"/> and restores the previous alignment, if any.
	/// </summary>
	/// <remarks>
	///   If there is no <see cref="AlignmentRange"/> to end, the method call is ignored.
	/// </remarks>
	public void EndAlignment()
	{
		FlushCurrentAlignment();
		
		if (_alignmentStack.Count > 0)
		{
			_alignmentStack.Pop();
		}
	}

	/// <summary>
	///   Clears the state of the parser.
	/// </summary>
	public void Reset()
	{
		_styleStack.Clear();
		
		_currentRuns.Clear();
		_currentIcons.Clear();
		_currentCommands.Clear();
		_currentAlignments.Clear();

		_alignmentStack.Clear();

		_textPosition = 0;
		_sequenceIndex = 0;
		_alignStart = 0;
	}

	private void FlushCurrentAlignment()
	{
		if (_sequenceIndex > _alignStart && _alignmentStack.TryPeek(out HorizontalAlignment current))
		{
			_currentAlignments.Add(new AlignmentRange(_alignStart, _sequenceIndex, current));
		}

		_alignStart = _sequenceIndex;
	}
}
