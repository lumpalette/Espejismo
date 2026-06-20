namespace Spectrum.RichText;

/// <summary>
///		Represents a name-value string property associated to a text tag or command.
/// </summary>
/// <param name="name">
///		The name of the property.
///	</param>
/// <param name="value">
///		The value of the property.
///	</param>
public readonly struct TagProperty(string name, string value)
{
	/// <summary>
	///		Gets the name of the property.
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	///		Gets the value of the property.
	/// </summary>
	public string Value { get; } = value;
}
