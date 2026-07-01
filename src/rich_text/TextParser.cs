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
	private readonly Dictionary<string, TagBehaviour>.AlternateLookup<ReadOnlySpan<char>> _tags;
	private readonly ParseContext _context = new();
	private readonly StringBuilder _accumulatedText = new();
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
	/// <param name="text">
	///   The rich-text string to parse.
	/// </param>
	/// <param name="style">
	///   The style attributes to use.
	/// </param>
	/// <returns>
	///   A <see cref="Text"/> representing the parsed <paramref name="text"/>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	///   Thrown if <paramref name="text"/>, <paramref name="style"/> or the font of <paramref name="style"/> is
	/// <see langword="null"/>.
	/// </exception>
	public Text Parse(string text, TextStyle style)
	{
		ArgumentNullException.ThrowIfNull(text, nameof(text));
		ArgumentNullException.ThrowIfNull(style, nameof(style));
		ArgumentNullException.ThrowIfNull(style.Font, nameof(style.Font));

		_context.Reset();
		_accumulatedText.Clear();

		// This allocates two lists per call, but I don't think it matters too much right now.
		ProcessNode(0, Document.Parse(text));

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

	private void ProcessNode(int parent, in Document document)
	{
		var entityBuffer = (stackalloc char[2]);
		
		for (int i = document.Nodes[parent].FirstChild; i != -1; i = document.Nodes[i].Sibling)
		{
			var childNode = document.Nodes[i];

			switch (childNode.Type)
			{
				case NodeType.Root:
					throw new UnreachableException("how the fuck this happened");

				case NodeType.Element:
					// 1. Commit accumulated text into a single text run.
					if (_accumulatedText.Length > 0)
					{
						_context.AppendText(_accumulatedText.ToString());
						_accumulatedText.Clear();
					}

					// 2. Retrieve tag from the registry.
					var tagName = document.Text.AsSpan(childNode.ValueStart, childNode.ValueLength);

					if (!_tags.TryGetValue(tagName, out TagBehaviour? tag))
					{
						Godot.GD.PushWarning($"Unknown text tag with name '{tagName}'.");
						ProcessNode(i, document);
						continue;
					}

					// 3. Extract and validate tag properties.
					var attributes = document.Attributes.Slice(childNode.AttributeStart, childNode.AttributeCount);
					var properties = AttributesToProperties(attributes, document.Text);

					if (!HasRequiredProperties(tag, properties))
					{
						Godot.GD.PushWarning($"Text tag with name '{tagName}' is missing required properties.");
						ProcessNode(i, document);
						continue;
					}

					// 4. Initialize tag effects and process child nodes.
					bool success = tag.Begin(_context, properties);
					ProcessNode(i, document);

					if (success)
					{
						tag.End(_context);
					}
					break;
				
				case NodeType.Text:
					_accumulatedText.Append(document.Text, childNode.ValueStart, childNode.ValueLength);
					break;

				case NodeType.CharacterEntity:
					if (childNode.CharacterEntity.TryEncodeToUtf16(entityBuffer, out int charsWritten))
					{
						_accumulatedText.Append(entityBuffer[..charsWritten]);
					}
					break;
			}
		}

		if (_accumulatedText.Length > 0)
		{
			_context.AppendText(_accumulatedText.ToString());
		}
	}

	private ReadOnlySpan<TagProperty> AttributesToProperties(ReadOnlySpan<AttributeSpan> attributes, string text)
	{
		for (int i = 0; i < attributes.Length; i++)
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
		int requiredPropertyCount = tag.RequiredPropertyNames.Count;

		// It doesn't matter the length of properties because we can pass optional properties to any tag.
		if (requiredPropertyCount == 0)
		{
			return true;
		}

		for (int i = 0; i < requiredPropertyCount; i++)
		{
			bool found = false;

			for (int j = 0; j < properties.Length; j++)
			{
				if (tag.RequiredPropertyNames[i] == properties[j].Name)
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
