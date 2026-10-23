using Godot;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents a <see cref="Resource"/> that wraps a <see cref="TextStyle"/>, allowing it to be accessed through the
/// inspector.
/// </summary>
[GlobalClass, Tool]
public partial class TextStyleTemplate : TextResource
{
    /// <summary>
    /// Gets the style represented by the template.
    /// </summary>
    /// <remarks>
    /// The property can only be edited through the inspector, emitting <see cref="Resource.Changed"/> appropriately.
    /// </remarks>
    public TextStyle Value { get; private set; } = TextStyle.Empty;
}
