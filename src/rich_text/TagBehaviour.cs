using System;
using System.Collections.Generic;

namespace Spectrum.RichText;

/// <summary>
///   Provides a base class for describing the characteristics and behaviour of a rich-text tag during parsing.
/// </summary>
/// <param name="name">
///   The name of the tag, case-sensitive.
/// </param>
/// <param name="requiredProperties">
///   The name of the properties required by the tag.
/// </param>
public abstract class TagBehaviour(string name, IReadOnlyList<string> requiredProperties)
{
	/// <summary>
	///   Gets the name of the tag, case-sensitive. Used to activate the tag's effects in-text.
	/// </summary>
	public string Name { get; } = name;
	
	/// <summary>
	///   Gets a collection of the property names required for the tag to function.
	/// </summary>
	/// <remarks>
	///   Returns an empty list if the tag does not require any obligatory properties.
	/// </remarks>
	public IReadOnlyList<string> RequiredPropertyNames { get; } = requiredProperties;

	/// <summary>
	///   Called before the parser begins processing the tag elements.
	/// </summary>
	/// <param name="context">
	///   The current state of the parser.
	/// </param>
	/// <param name="properties">
	///   A read-only span of properties associated with the tag.
	/// </param>
	/// <returns>
	///   <see langword="true"/> if the tag effect was applied successfully; otherwise, <see langword="false"/>.
	/// </returns>
	public abstract bool Begin(ParseContext context, ReadOnlySpan<TagProperty> properties);

	/// <summary>
	///   Called after the parser finishes processing the tag elements.
	/// </summary>
	/// <remarks>
	///   The method is only called if <see cref="Begin(ParseContext, ReadOnlySpan{TagProperty})"/> returned
	///   <see langword="true"/>.
	/// </remarks>
	/// <param name="context">
	///   The current state of the parser.
	/// </param>
	public virtual void End(ParseContext context)
	{
	}

	/// <summary>
	///   Searches for a <see cref="TagProperty"/> with the specified name within a <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="properties">
	///   The properties to search through.
	/// </param>
	/// <param name="name">
	///   The name of the property to find, case-sensitive.
	/// </param>
	/// <returns>
	///   The matching <see cref="TagProperty"/> if found; otherwise, the <see langword="default"/> value for
	///   <see cref="TagProperty"/>.
	/// </returns>
	protected static TagProperty FindProperty(ReadOnlySpan<TagProperty> properties, string name)
	{
		foreach (TagProperty property in properties)
		{
			if (property.Name.SequenceEqual(name))
			{
				return property;
			}
		}

		return default;
	}
}
