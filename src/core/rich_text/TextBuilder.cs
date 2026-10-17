using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Espejismo.Core.RichText;

/// <summary>
/// Provides a mechanism for sequentially constructing <see cref="Text"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// A rich-text string is composed of five types of items, known as <b>shape items</b>:
/// <list type="bullet">
///     <item>
///         <term>Runs</term>
///         <description>A sequence of characters that shares the same style.</description>
///     </item>
///     <item>
///         <term>Icons</term>
///         <description>A texture embedded directly into the text.</description>
///     </item>
///     <item>
///         <term>Markers</term>
///         <description>A named container of tag attributes, inserted at a specific glyph index.</description>
///     </item>
///     <item>
///         <term>Line breaks</term>
///         <description>The position of an explicit line break.</description>
///     </item>
///     <item>
///         <term>Alignment</term>
///         <description>The horizontal alignment in which text runs or icons are positioned.</description>
///     </item>
/// </list>
/// </para>
/// <para>
/// To synthesize rich-text, use the methods provided by this class to sequentially append shape items to the output.
/// At the end, use <see cref="Build"/> to generate a <see cref="Text"/> instance based on the final state of the
/// builder.
/// </para>
/// <para>
/// The class also maintains a stack of style overrides and alignments, which you can modify using the <c>Push/Pop*</c>
/// methods, allowing for complex nesting if required.
/// </para>
/// <para>
/// At construction, you can specify the maximum number of characters (grapheme clusters) that can be appended to the
/// builder. This mechanism can be used to perform typewriter effects naturally before shaping. Only text and icons
/// consume this limit.
/// </para>
/// </remarks>
public class TextBuilder
{
    private readonly List<ShapeItem> _items = [];
    private readonly Stack<TextStyle> _styleStack = [];
    private readonly Stack<HorizontalAlignment> _alignmentStack = [];

    private readonly int _initialChars;

    private int _remainingChars;

    /// <summary>
    /// Creates a new <see cref="TextBuilder"/> instance with no character limit imposed.
    /// </summary>
    public TextBuilder() : this(-1)
    {
    }

    /// <summary>
    /// Creates a new <see cref="TextBuilder"/> instance by specifying its character limit.
    /// </summary>
    /// <param name="charLimit">
    /// The number of characters that can be appended. If set to -1, no limit is applied.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="charLimit"/> is less than -1.
    /// </exception>
    public TextBuilder(int charLimit)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(charLimit, -1);

        _initialChars = charLimit;
        _remainingChars = charLimit;
    }

    /// <summary>
    /// Gets the style override currently at the top of the style stack.
    /// </summary>
    /// <remarks>
    /// Returns an empty (default) style if the style stack is empty.
    /// </remarks>
    public TextStyle TopStyle
    {
        get
        {
            _ = _styleStack.TryPeek(out var result);
            return result;
        }
    }

    /// <summary>
    /// Gets the alignment currently at the top of the alignment stack.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="null"/> if the alignment stack is empty, which indicates that the base alignment should
    /// be used instead.
    /// </remarks>
    public HorizontalAlignment? TopAlignment
    {
        get
        {
            if (!_alignmentStack.TryPeek(out var result))
            {
                return null;
            }

            return result;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the character limit has been reached.
    /// </summary>
    /// <remarks>
    /// Once the character limit is reached, further attempts to append characters or line breaks will be ignored.
    /// </remarks>
    public bool IsExhausted => _remainingChars == 0;

    /// <summary>
    /// Inserts the specified <see cref="TextStyle"/> at the top of the style stack.
    /// </summary>
    /// <param name="style">The style to push onto the stack.</param>
    /// <returns>A reference to this instance.</returns>
    public TextBuilder PushStyle(TextStyle style)
    {
        _styleStack.Push(style);
        return this;
    }

    /// <summary>
    /// Removes the <see cref="TextStyle"/> currently at the top of the style stack.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// If the stack is empty, the method does nothing.
    /// </remarks>
    public TextBuilder PopStyle()
    {
        _ = _styleStack.TryPop(out _);
        return this;
    }

    /// <summary>
    /// Appends a run of text using the current <see cref="TopStyle"/>.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// <para>
    /// This method consumes a number of characters from the character limit equivalent to the number of grapheme
    /// clusters in <paramref name="text"/>. If the text contains more grapheme clusters than the remaining limit, it
    /// is truncated.
    /// </para>
    /// <para>
    /// If <paramref name="text"/> is empty or the character limit has been reached, the method does nothing.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="text"/> is <see langword="null"/>.
    /// </exception>
    public TextBuilder AppendText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length == 0 || IsExhausted)
        {
            return this;
        }

        if (_remainingChars != -1)
        {
            text = TakeFromRemaining(text);

            if (text.Length == 0)
            {
                return this;
            }
        }

        _items.Add(new ShapeItem.Run(text, TopStyle));
        return this;
    }

    /// <summary>
    /// Appends a <see cref="Texture2D"/> as a text icon, specifying the properties of its bounding box.
    /// </summary>
    /// <param name="texture">The texture to append.</param>
    /// <param name="alignment">The vertical alignment of the icon relative to the surrounding text.</param>
    /// <param name="size">The size of the texture rect, in pixels.</param>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// <para>
    /// This method consumes exactly one character from the character limit.
    /// </para>
    /// <para>
    /// If the character limit has been reached, the method does nothing.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="size"/> has a negative component.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="texture"/> is <see langword="null"/>.
    /// </exception>
    public TextBuilder AppendIcon(Texture2D texture, InlineAlignment alignment, Vector2 size)
    {
        ArgumentNullException.ThrowIfNull(texture);

        if (size.X < 0 || size.Y < 0)
        {
            throw new ArgumentException($"Icon size ({size}) cannot be negative");
        }

        if (IsExhausted)
        {
            return this;
        }

        if (_remainingChars != -1)
        {
            _remainingChars--;
        }

        _items.Add(new ShapeItem.Icon(texture, alignment, size, TopStyle));
        return this;
    }

    /// <summary>
    /// Appends a marker with the specified name and the <see cref="TagAttribute"/> values associated with it.
    /// </summary>
    /// <param name="name">The name for the marker.</param>
    /// <param name="attributes">The attributes associated with the marker.</param>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// The method copies the elements of <paramref name="attributes"/> into the new marker.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    public TextBuilder AppendMarker(string name, ReadOnlySpan<TagAttribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(name);

        _items.Add(new ShapeItem.Marker(name, attributes.ToArray()));
        return this;
    }

    /// <summary>
    /// Appends a hard line break.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// The method does not consume any character from the character limit. However, if the visible character limit has
    /// been reached, the method does nothing.
    /// </remarks>
    public TextBuilder AppendBreak()
    {
        _items.Add(new ShapeItem.LineBreak());
        return this;
    }

    /// <summary>
    /// Inserts the specified alignment at the top of the alignment stack, affecting subsequent text runs and icons.
    /// </summary>
    /// <param name="alignment">The alignment to push onto the stack.</param>
    /// <returns>A reference to this instance.</returns>
    public TextBuilder PushAlignment(HorizontalAlignment alignment)
    {
        _alignmentStack.Push(alignment);
        _items.Add(new ShapeItem.Align(alignment));
        return this;
    }

    /// <summary>
    /// Removes the alignment currently at the top of the alignment stack, reverting to the previous alignment.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    /// <remarks>
    /// If the stack is empty, the method does nothing.
    /// </remarks>
    public TextBuilder PopAlignment()
    {
        if (_alignmentStack.TryPop(out _))
        {
            _items.Add(new ShapeItem.Align(TopAlignment));
        }

        return this;
    }

    /// <summary>
    /// Creates a new <see cref="Text"/> based on the state of the builder.
    /// </summary>
    /// <param name="style">The base style to apply to the text.</param>
    /// <returns>A <see cref="Text"/> containing the shape items appended so far.</returns>
    /// <remarks>
    /// The builder is not cleared after calling this method.
    /// </remarks>
    public Text Build(TextStyle style)
    {
        return new Text([.. _items], style);
    }

    /// <summary>
    /// Resets the state of the builder, clearing the list of shape items, stacks and restoring the character limit.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        _styleStack.Clear();
        _alignmentStack.Clear();
        _remainingChars = _initialChars;
    }

    private string TakeFromRemaining(string text)
    {
        var span = text.AsSpan();
        var index = 0;

        while (index < span.Length)
        {
            if (_remainingChars <= 0)
            {
                _remainingChars = 0;
                return text[..index];
            }

            index += StringInfo.GetNextTextElementLength(span[index..]);
            _remainingChars--;
        }

        return text;
    }
}
