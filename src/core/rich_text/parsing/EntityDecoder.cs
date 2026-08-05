using System;
using System.Text;

namespace Espejismo.Core.RichText.Parsing;

// A decoder for HTML character entities. Entity identifiers don't contain the leading ampersand.
internal static class EntityDecoder
{
	public static bool TryDecode(ReadOnlySpan<char> entity, out Rune decoded)
	{
		if (entity.Length <= 1 || entity[^1] != ';')
		{
			decoded = new Rune('�');
			return false;
		}

		if (entity[0] == '#')
		{
			if (entity.Length >= 2 && (entity[1] == 'x' || entity[1] == 'X'))
			{
				decoded = DecodeHexadecimalEntity(entity);
			}
			else
			{
				decoded = DecodeDecimalEntity(entity);
			}
		}
		else
		{
			decoded = DecodeNamedEntity(entity);
		}

		return decoded.Value != '�';
	}

	private static Rune DecodeNamedEntity(ReadOnlySpan<char> entity)
	{
		// this is gonna be useful later i swear
		return new Rune(entity switch
		{
			"Alpha;"   => 'Α',
			"alpha;"   => 'α',
			"amp;"     => '&',
			"apos;"    => '\'',
			"asymp;"   => '≈',
			"bdquo;"   => '„',
			"Beta;"    => 'Β',
			"beta;"    => 'β',
			"bull;"    => '•',
			"cent;"    => '¢',
			"Chi;"     => 'Χ',
			"chi;"     => 'χ',
			"copy;"    => '©',
			"curren;"  => '¤',
			"darr;"    => '↓',
			"deg;"     => '°',
			"Delta;"   => 'Δ',
			"delta;"   => 'δ',
			"divide;"  => '÷',
			"Epsilon;" => 'Ε',
			"epsilon;" => 'ε',
			"Eta;"     => 'Η',
			"eta;"     => 'η',
			"euro;"    => '€',
			"frac12;"  => '½',
			"frac14;"  => '¼',
			"frac34;"  => '¾',
			"Gamma;"   => 'Γ',
			"gamma;"   => 'γ',
			"ge;"      => '≥',
			"gt;"      => '>',
			"harr;"    => '↔',
			"hellip;"  => '…',
			"infin;"   => '∞',
			"Iota;"    => 'Ι',
			"iota;"    => 'ι',
			"Kappa;"   => 'Κ',
			"kappa;"   => 'κ',
			"Lambda;"  => 'Λ',
			"lambda;"  => 'λ',
			"laquo;"   => '«',
			"larr;"    => '←',
			"ldquo;"   => '“',
			"le;"      => '≤',
			"lrm;"      => '\u200E', // left-right mark
			"lsquo;"   => '‘',
			"lt;"      => '<',
			"mdash;"   => '—',
			"micro;"   => 'µ',
			"middot;"  => '·',
			"Mu;"      => 'Μ',
			"mu;"      => 'μ',
			"nbsp;"    => '\u00A0', // non-breaking space
			"ndash;"   => '–',
			"ne;"      => '≠',
			"not;"     => '¬',
			"Nu;"      => 'Ν',
			"nu;"      => 'ν',
			"Omega;"   => 'Ω',
			"omega;"   => 'ω',
			"Omicron;" => 'Ο',
			"omicron;" => 'ο',
			"para;"    => '¶',
			"permil;"  => '‰',
			"Phi;"     => 'Φ',
			"phi;"     => 'φ',
			"Pi;"      => 'Π',
			"pi;"      => 'π',
			"plusmn;"  => '±',
			"pound;"   => '£',
			"Prime;"   => '″',
			"prime;"   => '′',
			"Psi;"     => 'Ψ',
			"psi;"     => 'ψ',
			"quot;"    => '"',
			"raquo;"   => '»',
			"rarr;"    => '→',
			"rdquo;"   => '”',
			"reg;"     => '®',
			"Rho;"     => 'Ρ',
			"rho;"     => 'ρ',
			"rlm;"     => '\u200F', // right-left mark
			"rsquo;"   => '’',
			"sbquo;"   => '‚',
			"sect;"    => '§',
			"Sigma;"   => 'Σ',
			"sigma;"   => 'σ',
			"sup1;"    => '¹',
			"sup2;"    => '²',
			"sup3;"    => '³',
			"Tau;"     => 'Τ',
			"tau;"     => 'τ',
			"Theta;"   => 'Θ',
			"theta;"   => 'θ',
			"times;"   => '×',
			"trade;"   => '™',
			"uarr;"    => '↑',
			"Upsilon;" => 'Υ',
			"upsilon;" => 'υ',
			"Xi;"      => 'Ξ',
			"xi;"      => 'ξ',
			"yen;"     => '¥',
			"Zeta;"    => 'Ζ',
			"zeta;"    => 'ζ',
			_          => '�'
		});
	}

	private static Rune DecodeDecimalEntity(ReadOnlySpan<char> entity)
	{
		var code = 0;

		for (var i = 1; i < entity.Length - 1; i++)
		{
			if (!char.IsAsciiDigit(entity[i]))
			{
				return new Rune('\uFFFD');
			}

			code = (10 * code) + (entity[i] - '0');

			if (code > 0x10FFFF)
			{
				break;
			}
		}

		return ResolveCharacterReferenceCode(code);
	}

	private static Rune DecodeHexadecimalEntity(ReadOnlySpan<char> entity)
	{
		var code = 0;

		for (var i = 2; i < entity.Length - 1; i++)
		{
			var offset = entity[i] switch
			{
				>= '0' and <= '9' => 0x30,
				>= 'A' and <= 'F' => 0x37,
				>= 'a' and <= 'f' => 0x57,
				_ => -1
			};

			if (offset == -1)
			{
				return new Rune('\uFFFD');
			}

			code = (16 * code) + (entity[i] - offset);

			if (code > 0x10FFFF)
			{
				break;
			}
		}

		return ResolveCharacterReferenceCode(code);
	}

	private static Rune ResolveCharacterReferenceCode(int code)
	{
		// Either the code is null, outside the unicode range or a unicode surrogate.
		if (code == 0x00 || code > 0x10FFFF || (code is >= 0xD800 and <= 0xDFFF))
		{
			return new Rune('\uFFFD');
		}

		return new Rune(code);
	}
}
