using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Spectrum.RichText;

/// <summary>
///		Provides an interface to query and mutate the state of a rich-text parsing operation.
/// </summary>
/// <remarks>
///		The class mantains three lists for the text runs, icons and commands, along with a stack of text styles that
///		affects the rendering properties of subsequent text runs.
/// </remarks>
public class ParseContext
{
	private readonly Stack<TextRunStyle> _styleStack = [];
	private readonly List<TextRun> _runs = [];
	private readonly List<TextCommand> _commands = [];
	private readonly Stack<HorizontalAlignment> _alignmentStack = [];
	private readonly List<AlignmentBlock> _alignmentBlocks = [];

	private int _textPosition;
	private int _blockStart;

	/// <summary>
	///		Gets the text style currently at the top of the style stack.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///		Thrown if the style stack is empty.
	/// </exception>
	public TextRunStyle TopStyle
	{
		get
		{
			if (!_styleStack.TryPeek(out TextRunStyle top))
			{
				throw new InvalidOperationException("Style stack is empty");
			}

			return top;
		}
	}

	/// <summary>
	///		Gets the current <see cref="TextRun"/> instances appended so far, sorted by order of addition.
	/// </summary>
	/// <remarks>
	///		The returned span becomes invalid when a new run is added.
	/// </remarks>
	public ReadOnlySpan<TextRun> Runs => CollectionsMarshal.AsSpan(_runs);

	/// <summary>
	///		Gets the current <see cref="TextCommand"/> instances appended so far, sorted by order of addition.
	/// </summary>
	/// <remarks>
	///		The returned span becomes invalid when a new command is added.
	/// </remarks>
	public ReadOnlySpan<TextCommand> Commands => CollectionsMarshal.AsSpan(_commands);

	/// <summary>
	///		Gets the current <see cref="AlignmentBlock"/> instances appended so far, sorted by order of addition.
	/// </summary>
	/// <remarks>
	///		The returned span becomes invalid when a new command is added.
	/// </remarks>
	public ReadOnlySpan<AlignmentBlock> AlignmentOverrides => CollectionsMarshal.AsSpan(_alignmentBlocks);

	/// <summary>
	///		Adds a <see cref="TextRunStyle"/> at the top of the stack.
	/// </summary>
	/// <param name="style">
	///		The style to push onto the stack.
	/// </param>
	public void PushStyle(TextRunStyle style)
	{
		_styleStack.Push(style);
	}

	/// <summary>
	///		Removes the style currently at the top of the stack.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///		Thrown if the style stack is empty.
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
	///		Creates and appends a new <see cref="TextRun"/> with the specified text using the current top style.
	/// </summary>
	/// <param name="text">
	///		The characters in the text run.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///		Thrown if <paramref name="text"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	///		Thrown if the style stack is empty.
	/// </exception>
	public void AppendText(string text)
	{
		ArgumentNullException.ThrowIfNull(text, nameof(text));

		_runs.Add(new TextRun(text, TopStyle));
		_textPosition += text.Length;
	}

	/// <summary>
	///		Creates a new <see cref="TextCommand"/> with the specified name and properties and inserts it at the
	///		current text position.
	/// </summary>
	/// <param name="name">
	///		The name of the command.
	/// </param>
	/// <param name="properties">
	///		The properties passed to the command.
	/// </param>
	/// <exception cref="ArgumentNullException">
	///		Thrown if <paramref name="name"/> is <see langword="null"/>.
	/// </exception>
	public void AppendCommand(string name, ReadOnlySpan<TagProperty> properties)
	{
		ArgumentNullException.ThrowIfNull(name, nameof(name));

		// Commands generally outlive the document tree, so we have to store its properties in the heap.
		_commands.Add(new TextCommand(name, _textPosition, properties.ToArray()));
	}

	/// <summary>
	///		Opens a new alignment block, applying the specified alignment to every subsequent <see cref="TextRun"/>
	/// </summary>
	/// <param name="alignment">
	///		The alignment to apply.
	/// </param>
	public void BeginAlignment(HorizontalAlignment alignment)
	{
		if (_alignmentStack.TryPeek(out HorizontalAlignment current) && current == alignment)
		{
			return;
		}

		FlushCurrentAlignment();
		_alignmentStack.Push(alignment);
		
	}

	/// <summary>
	///		Closes the current alignment block and restores the previous alignment, if any.
	/// </summary>
	/// <remarks>
	///		If there is no pending alignment block, the method call is ignored.
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
	///		Clears the state of the parser.
	/// </summary>
	public void Reset()
	{
		_styleStack.Clear();
		_runs.Clear();
		_commands.Clear();
		_alignmentStack.Clear();
		_alignmentBlocks.Clear();
		_textPosition = 0;
		_blockStart = 0;
	}

	private void FlushCurrentAlignment()
	{
		int length = _runs.Count - _blockStart;

		if (length > 0 && _alignmentStack.TryPeek(out HorizontalAlignment current))
		{
			_alignmentBlocks.Add(new AlignmentBlock(_blockStart, length, current));
		}

		_blockStart = _runs.Count;
	}
}
