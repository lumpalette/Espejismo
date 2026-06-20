namespace Spectrum.RichText.Parsing;

internal enum TokenType
{
	None,
	Text,
	StartTag, // also includes self-closing tags.
	EndTag,
	CharacterEntity,
	Eof
}
