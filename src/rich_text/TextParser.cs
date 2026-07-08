using Spectrum.RichText.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Spectrum.RichText;

/// <summary>
///   Provides a mechanism for transforming rich-text strings into structured <see cref="Text"/> representations.
/// </summary>
public class TextParser
{
	private readonly ParseContext _context = new();
	private readonly StringBuilder _accumulatedText = new();
	
	private readonly Dictionary<string, TagBehaviour>.AlternateLookup<ReadOnlySpan<char>> _tags;

	private PropertyBuffer _properties;
	
	/// <summary>
	///   Initializes a new instance of the <see cref="TextParser"/> class with no tags registered.
	/// </summary>
	public TextParser()
	{
		var tagDict = new Dictionary<string, TagBehaviour>();
		_tags = tagDict.GetAlternateLookup<ReadOnlySpan<char>>();
	}

	/// <summary>
	///   Parses the specified rich-text formatted string into a <see cref="Text"/> instance using the specified
	///   <see cref="TextStyle"/>.
	/// </summary>
	/// <param name="richText">
	///   The rich-text string to parse.
	/// </param>
	/// <param name="style">
	///   The style attributes to use.
	/// </param>
	/// <returns>
	///   The resulting <see cref="Text"/> from parsing the <paramref name="richText"/> input.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="richText"/> or <paramref name="style"/> is <see langword="null"/>.
	/// </exception>
	public Text Parse(string richText, TextStyle style)
	{
		ArgumentNullException.ThrowIfNull(richText, nameof(richText));
		ArgumentNullException.ThrowIfNull(style, nameof(style));
		
		_context.Reset();
		_accumulatedText.Clear();

		// This allocates two lists per call, but I don't think it matters too much right now.
		ProcessNode(0, Document.Parse(richText));

		return new Text(style, _context);
	}

	/// <summary>
	///   Adds the specified <see cref="TagBehaviour"/> to the tag registry.
	/// </summary>
	/// <param name="tag">
	///   The text tag to register.
	/// </param>
	/// <exception cref="ArgumentException">
	///   Thrown if a tag with the name of <paramref name="tag"/> is already registered.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="tag"/> is <see langword="null"/>.
	/// </exception>
	public void RegisterTag(TagBehaviour tag)
	{
		ArgumentNullException.ThrowIfNull(tag, nameof(tag));

		if (!_tags.Dictionary.TryAdd(tag.Name, tag))
		{
			throw new ArgumentException($"Tag with name '{tag.Name}' is already defined");
		}
	}

	private void ProcessNode(int parentIndex, in Document document)
	{
		for (var i = document.Nodes[parentIndex].FirstChildIndex; i != -1; i = document.Nodes[i].SiblingIndex)
		{
			var child = document.Nodes[i];

			switch (child.Type)
			{
				case NodeType.Root:
					throw new UnreachableException("how the fuck this happened");
				case NodeType.Element:
					ProcessElement(child, i, document);
					break;
				case NodeType.Text:
					ProcessText(document.Text, child.ValueStart, child.ValueLength);
					break;
				case NodeType.CharacterEntity:
					ProcessCharacterEntity(child.CharacterEntity);
					break;
			}
		}

		if (_accumulatedText.Length > 0)
		{
			FlushAccumulatedText();
		}
	}

	private void ProcessElement(Node element, int elementIndex, in Document document)
	{
		// 1. Commit accumulated text into a single text run.
		if (_accumulatedText.Length > 0)
		{
			FlushAccumulatedText();
		}

		// 2. Retrieve tag from the registry.
		var tagName = document.Text.AsSpan(element.ValueStart, element.ValueLength);

		if (!_tags.TryGetValue(tagName, out TagBehaviour? tag))
		{
			if (!Godot.Engine.IsEditorHint())
			{
				Godot.GD.PushWarning($"Unknown text tag with name '{tagName}'.");
			}

			ProcessNode(elementIndex, document);
			return;
		}

		// 3. Extract and validate tag properties.
		var attributes = document.Attributes.Slice(element.AttributeStart, element.AttributeCount);
		var properties = AttributesToProperties(attributes, document.Text);

		if (!HasRequiredProperties(tag, properties))
		{
			if (!Godot.Engine.IsEditorHint())
			{
				Godot.GD.PushWarning($"Text tag with name '{tagName}' is missing required properties.");
			}

			ProcessNode(elementIndex, document);
			return;
		}

		// 4. Initialize tag effects and process child nodes.
		var success = tag.Begin(_context, properties);
		ProcessNode(elementIndex, document);

		if (success)
		{
			tag.End(_context);
		}
	}

	// I only added this method for consistency with the other Process*() methods xd
	private void ProcessText(string str, int start, int length)
	{
		_accumulatedText.Append(str, start, length);
	}

	private void FlushAccumulatedText()
	{
		_context.AppendText(_accumulatedText.ToString());
		_accumulatedText.Clear();
	}

	private void ProcessCharacterEntity(Rune entity)
	{
		var buffer = (stackalloc char[2]);

		if (entity.TryEncodeToUtf16(buffer, out int charsWritten))
		{
			_accumulatedText.Append(buffer[..charsWritten]);
		}
	}

	private ReadOnlySpan<TagProperty> AttributesToProperties(ReadOnlySpan<AttributeSpan> attributes, string text)
	{
		for (var i = 0; i < attributes.Length; i++)
		{
			var attribute = attributes[i];

			_properties[i] = new TagProperty(
				text,
				attribute.NameStart,
				attribute.NameLength,
				attribute.ValueStart,
				attribute.ValueLength
			);
		}

		return _properties[..attributes.Length];
	}

	private static bool HasRequiredProperties(TagBehaviour tag, ReadOnlySpan<TagProperty> properties)
	{
		var requiredPropertyCount = tag.RequiredPropertyNames.Count;

		// It doesn't matter the length of properties because we can pass optional properties to any tag.
		if (requiredPropertyCount == 0)
		{
			return true;
		}

		for (var i = 0; i < requiredPropertyCount; i++)
		{
			var found = false;

			for (var j = 0; j < properties.Length; j++)
			{
				if (properties[j].Name.SequenceEqual(tag.RequiredPropertyNames[i]))
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				return false;
			}
		}

		return true;
	}
	
	[InlineArray(Tokenizer.MaxAttributes)]
	private struct PropertyBuffer
	{
		public TagProperty Element;
	}
} 
