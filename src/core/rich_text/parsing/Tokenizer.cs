using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Espejismo.Core.RichText.Parsing;

// The heart of this whole system. Reads and splits an HTML-like string into a sequence of tokens.
internal ref partial struct Tokenizer(string source)
{
	public const int MaxAttributes = 8;

	private const int Eof = -1;

	private readonly ReadOnlySpan<char> _source = source.AsSpan();

	private State _state;
	private int _position;
	private bool _isEndTag;
	private AttributeArray _attributes;
	private int _attributeCount;
	private bool _attributeStarted;
	private char _attributeDelimiter;
	private int _currentAttributeNameStart;
	private int _currentAttributeNameLength;
	private int _currentAttributeValueStart;
	private int _currentAttributeValueLength;

	public TokenType TokenType { get; private set; }

	// Depends on the token type:
	// * For text tokens, returns the text content
	// * For tag tokens, returns the tag name
	// * For anything else, the returned value is invalid
	public ReadOnlySpan<char> ReadValue { get; private set; }

	// Marks the beginning of ReadValue.
	public int StartIndex { get; private set; }

	[UnscopedRef]
	public readonly ReadOnlySpan<AttributeSpan> Attributes => _attributes[.._attributeCount];

	public bool IsSelfClosing { get; private set; }

	public Rune CharacterEntity { get; private set; }

	public bool Read()
	{
		ResetToken();

		// What are you willing to do?
		// Oh, tell me what you're willing to do
		// (Kiss it, kiss it better, baby)
		while (TokenType == TokenType.None)
		{
			TokenType = _state switch
			{
				State.Data => ExecData(),
				State.TagOpen => ExecTagOpen(),
				State.EndTagOpen => ExecEndTagOpen(),
				State.TagName => ExecTagName(),
				State.BeforeAttributeName => ExecBeforeAttributeName(),
				State.AttributeName => ExecAttributeName(),
				State.AfterAttributeName => ExecAfterAttributeName(),
				State.BeforeAttributeValue => ExecBeforeAttributeValue(),
				State.AttributeValueQuoted => ExecAttributeValueQuoted(),
				State.AttributeValueUnquoted => ExecAttributeValueUnquoted(),
				State.AfterAttributeValueQuoted => ExecAfterAttributeValueQuoted(),
				State.SelfClosingStartTag => ExecSelfClosingStartTag(),
				State.CharacterReference => ExecCharacterReference(),
				_ => throw new UnreachableException($"Invalid state reached ({_state})")
			};
		}

		return TokenType != TokenType.Eof;
	}

	private int Consume()
	{
		if (_position < _source.Length)
		{
			return _source[_position++];
		}

		// We always advance the cursor even when we have reached the end for reconsumes.
		_position++;
		return Eof;
	}

	private TokenType SwitchTo(State state, TokenType returnToken = TokenType.None)
	{
		_state = state;
		return returnToken;
	}

	private TokenType ReconsumeIn(State state)
	{
		_position--;
		return SwitchTo(state);
	}

	private void AppendCurrentAttribute()
	{
		if (!_attributeStarted || _attributeCount >= MaxAttributes)
		{
			ResetCurrentAttribute();
			return;
		}

		_attributes[_attributeCount++] = new AttributeSpan(
			_currentAttributeNameStart,
			_currentAttributeNameLength,
			_currentAttributeValueStart,
			_currentAttributeValueLength);

		ResetCurrentAttribute();
	}

	private void ResetToken()
	{
		TokenType = (_position < _source.Length) ? TokenType.None : TokenType.Eof;
		ReadValue = [];
		StartIndex = _position;
		IsSelfClosing = false;
		CharacterEntity = default;
		_isEndTag = false;
		_attributeCount = 0;
	}

	private void ResetCurrentAttribute()
	{
		_currentAttributeNameStart = 0;
		_currentAttributeNameLength = 0;
		_currentAttributeValueStart = 0;
		_currentAttributeValueLength = 0;
		_attributeStarted = false;
	}

	private TokenType GetCurrentTagType()
	{
		return _isEndTag ? TokenType.EndTag : TokenType.StartTag;
	}

	[InlineArray(MaxAttributes)]
	private struct AttributeArray
	{
		public AttributeSpan Element;
	}

	private enum State
	{
		Data,
		TagOpen,
		EndTagOpen,
		TagName,
		BeforeAttributeName,
		AttributeName,
		AfterAttributeName,
		BeforeAttributeValue,
		AttributeValueQuoted,
		AttributeValueUnquoted,
		AfterAttributeValueQuoted,
		SelfClosingStartTag,
		CharacterReference
	}
}
