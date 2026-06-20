namespace Spectrum.RichText.Parsing;

internal readonly struct AttributeSpan(int nameStart, int nameLength, int valueStart, int valueLength)
{
	public int NameStart { get; } = nameStart;

	public int NameLength { get; } = nameLength;

	public int ValueStart { get; } = valueStart;

	public int ValueLength { get; } = valueLength;
}
