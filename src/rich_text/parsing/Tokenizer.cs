using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Spectrum.RichText.Parsing;

internal ref partial struct Tokenizer(string text)
{
	public const int MaxAttributes = 8;
	private const int Eof = -1;

	private readonly ReadOnlySpan<char> _inputText = text.AsSpan();

	private State _state = State.Data;
	private int _position;
	private AttributeBuffer _attributes;
	private int _attributeCount;
	private int _currentAttributeNameStart;
	private int _currentAttributeNameLength;
	private int _currentAttributeValueStart;
	private int _currentAttributeValueLength;
	private char _attributeDelimiter;
	
	public TokenType TokenType { get; private set; }

	// Depends on the token type:
	// * For text tokens, returns the text content
	// * For tag tokens, returns the tag name
	// * For anything else, the value is invalid and returns an empty string
	public ReadOnlySpan<char> ReadValue { get; private set; }

	// Marks the beginning of ReadValue.
	public int StartIndex { get; private set; }

	public readonly ReadOnlySpan<AttributeSpan> Attributes
	{
		[UnscopedRef]
		get
		{
			var attributes = (ReadOnlySpan<AttributeSpan>)_attributes;
			return attributes[.._attributeCount];
		}
	}

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

	private int Consume()
	{
		if (_position < _inputText.Length)
		{
			return _inputText[_position++];
		}

		// We always advance the cursor even when we have reached the end for reconsumes.
		_position++;
		return Eof;
	}

	private void AppendCurrentAttribute()
	{
		Debug.Assert(_attributeCount < MaxAttributes);

		if (_currentAttributeNameLength == 0 || _attributeCount >= MaxAttributes)
		{
			return;
		}

		_attributes[_attributeCount++] = new AttributeSpan(
			_currentAttributeNameStart,
			_currentAttributeNameLength,
			_currentAttributeValueStart,
			_currentAttributeValueLength
		);

		_currentAttributeNameStart = 0;
		_currentAttributeNameLength = 0;
		_currentAttributeValueStart = 0;
		_currentAttributeValueLength = 0;
	}

	private void ResetToken()
	{
		TokenType = (_position < _inputText.Length) ? TokenType.None : TokenType.Eof;
		ReadValue = [];
		StartIndex = _position;
		IsSelfClosing = false;
		CharacterEntity = default;
		_attributeCount = 0;
	}

	[InlineArray(MaxAttributes)]
	private struct AttributeBuffer
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
