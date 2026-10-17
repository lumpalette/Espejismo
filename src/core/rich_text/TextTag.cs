using Godot;
using System;

namespace Espejismo.Core.RichText;

/// <summary>
/// Serves as the base class for defining the behavior of a rich-text tag during parsing.
/// </summary>
[GlobalClass, Tool]
public abstract partial class TextTag : TextResource
{
    /// <summary>
    /// Gets a value indicating whether the tag is a void element, that is, one that cannot have child nodes.
    /// </summary>
    public abstract bool IsVoid { get; }

    /// <summary>
    /// Called when an element begins, before its children are processed.
    /// </summary>
    /// <param name="builder">The working text state.</param>
    /// <param name="attributes">The attributes associated to the tag.</param>
    /// <returns>
    /// <see langword="true"/> if <see cref="End(TextBuilder)"/> should be called once the element's children have been
    /// processed; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Use this method to implement whatever effect the element represents, such as pushing a new style onto
    /// <paramref name="builder"/>. Any resource the tag may require (fonts, textures, etc.) should be resolved through
    /// the static <see cref="TextConfig"/> API.
    /// </remarks>
    public abstract bool Begin(TextBuilder builder, ReadOnlySpan<TagAttribute> attributes);

    /// <summary>
    /// Called when an element ends, after its children have been processed.
    /// </summary>
    /// <param name="builder">The working text state.</param>
    /// <remarks>
    /// <para>
    /// Use this method to revert any effects done to the <paramref name="builder"/> during
    /// <see cref="Begin(TextBuilder, ReadOnlySpan{TagAttribute})"/>.
    /// </para>
    /// <para>
    /// The method is only called if the corresponding call to
    /// <see cref="Begin(TextBuilder, ReadOnlySpan{TagAttribute})"/> returned <see langword="true"/>.
    /// </para>
    /// </remarks>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public virtual void End(TextBuilder builder)
    {
    }
#pragma warning restore CA1716 // Identifiers should not match keywords
}
