using System.Text;

namespace Espejismo.Core.RichText.Parsing;

// The implementation of the tokenizer, based on the HTML tokenizer specification.
partial struct Tokenizer
{
	private TokenType ExecData()
	{
		var input = Consume();

		if (input is '&' or '<')
		{
			if (_position - 1 > StartIndex)
			{
				_position--;
				ReadValue = _source[StartIndex.._position];
				return TokenType.Text;
			}

			return SwitchTo((input == '&') ? State.CharacterReference : State.TagOpen);
		}

		if (input == Eof)
		{
			if (_position > StartIndex)
			{
				ReadValue = _source[StartIndex..];
				return TokenType.Text;
			}

			return TokenType.Eof;
		}

		return TokenType.None;
	}

	private TokenType ExecTagOpen()
	{
		var input = Consume();

		if (input == '/')
		{
			return SwitchTo(State.EndTagOpen);
		}

		if (char.IsAsciiLetter((char)input))
		{
			StartIndex = _position - 1;
			return ReconsumeIn(State.TagName);
		}

		if (input == Eof)
		{
			return ReconsumeIn(State.Data);
		}

		return ReconsumeIn(State.Data);
	}

	private TokenType ExecEndTagOpen()
	{
		// The specification purposely ignores the character sequence "</>". We emit it as regular text token instead.
		var input = Consume();

		if (char.IsAsciiLetter((char)input))
		{
			_isEndTag = true;
			StartIndex = _position - 1;
			return ReconsumeIn(State.TagName);
		}

		if (input == Eof)
		{
			return ReconsumeIn(State.Data);
		}

		return ReconsumeIn(State.Data);
	}

	private TokenType ExecTagName()
	{
		var input = Consume();

		if (input is ' ' or '/' or '>')
		{
			ReadValue = _source[StartIndex..(_position - 1)];

			if (input == ' ')
			{
				return SwitchTo(State.BeforeAttributeName);
			}

			if (input == '/')
			{
				return SwitchTo(State.SelfClosingStartTag);
			}

			return SwitchTo(State.Data, GetCurrentTagType());
		}

		// This is not part of the HTML standard. Allows for defining a main tag attribute (e.g. <color=red>).
		if (input == '=')
		{
			ReadValue = _source[StartIndex..(_position - 1)];

			// Main tag attributes are nameless.
			_currentAttributeNameStart = StartIndex;
			_currentAttributeNameLength = 0;
			_attributeStarted = true;

			return SwitchTo(State.BeforeAttributeValue);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		return TokenType.None;
	}

	private TokenType ExecBeforeAttributeName()
	{
		var input = Consume();

		if (input == ' ')
		{
			return TokenType.None;
		}

		// In this system, attributes cannot start with a '<' sign, which is different to what HTML does.
		if (input == '<')
		{
			StartIndex -= _isEndTag ? 2 : 1;
			return ReconsumeIn(State.Data);
		}

		if (input is '/' or '>' or Eof)
		{
			return ReconsumeIn(State.AfterAttributeName);
		}

		_currentAttributeNameStart = _position - 1;
		_attributeStarted = true;

		if (input == '=')
		{
			// Handle empty space around the equal sign of the main tag attribute.
			if (_attributeCount == 0)
			{
				_currentAttributeNameStart = StartIndex;
				_currentAttributeNameLength = 0;
				
				return SwitchTo(State.BeforeAttributeValue);
			}

			// Off-by-one stupid error. We also need to account for the '=', as it's part of the attribute name.
			_currentAttributeNameLength = 1;
			return SwitchTo(State.AttributeName);
		}
		
		return ReconsumeIn(State.AttributeName);
	}

	private TokenType ExecAttributeName()
	{
		var input = Consume();

		if (input is ' ' or '/' or '>' or '=' or Eof)
		{
			if (input == '=')
			{
				return SwitchTo(State.BeforeAttributeValue);
			}

			return ReconsumeIn(State.AfterAttributeName);
		}

		_currentAttributeNameLength++;
		return TokenType.None;
	}

	private TokenType ExecAfterAttributeName()
	{
		var input = Consume();

		if (input == ' ')
		{
			return TokenType.None;
		}

		if (input == '/')
		{
			AppendCurrentAttribute();
			return SwitchTo(State.SelfClosingStartTag);
		}

		if (input == '=')
		{
			return SwitchTo(State.BeforeAttributeValue);
		}

		if (input == '>')
		{
			AppendCurrentAttribute();
			return SwitchTo(State.Data, GetCurrentTagType());
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		AppendCurrentAttribute();
		_currentAttributeNameStart = _position - 1;

		return ReconsumeIn(State.AttributeName);
	}

	private TokenType ExecBeforeAttributeValue()
	{
		var input = Consume();

		if (input == ' ')
		{
			return TokenType.None;
		}

		if (input == '>')
		{
			AppendCurrentAttribute();
			return SwitchTo(State.Data, GetCurrentTagType());
		}

		if (input is '"' or '\'')
		{
			_currentAttributeValueStart = _position;
			_attributeDelimiter = (char)input;
			return SwitchTo(State.AttributeValueQuoted);
		}

		_currentAttributeValueStart = _position - 1;
		return ReconsumeIn(State.AttributeValueUnquoted);
	}

	private TokenType ExecAttributeValueQuoted()
	{
		var input = Consume();

		if (input == _attributeDelimiter)
		{
			AppendCurrentAttribute();
			return SwitchTo(State.AfterAttributeValueQuoted);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		_currentAttributeValueLength++;
		return TokenType.None;
	}

	private TokenType ExecAttributeValueUnquoted()
	{
		var input = Consume();

		// The '/' cannot switch to the self-closing start tag state here according to the HTML specification, but
		// we do it regardless because it's more convenient.
		if (input is ' ' or '/' or '>')
		{
			AppendCurrentAttribute();

			if (input == ' ')
			{
				return SwitchTo(State.BeforeAttributeName);
			}

			if (input == '/')
			{
				return SwitchTo(State.SelfClosingStartTag);
			}

			return SwitchTo(State.Data, GetCurrentTagType());
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		_currentAttributeValueLength++;
		return TokenType.None;
	}

	private TokenType ExecAfterAttributeValueQuoted()
	{
		var input = Consume();

		if (input == ' ')
		{
			return SwitchTo(State.BeforeAttributeName);
		}

		if (input == '/')
		{
			return SwitchTo(State.SelfClosingStartTag);
		}

		if (input == '>')
		{
			return SwitchTo(State.Data, GetCurrentTagType());
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		return ReconsumeIn(State.BeforeAttributeName);
	}

	private TokenType ExecSelfClosingStartTag()
	{
		var input = Consume();

		if (input == '>')
		{
			IsSelfClosing = true;
			return SwitchTo(State.Data, GetCurrentTagType());
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		// Tread the '/' as white-space.
		return ReconsumeIn(State.BeforeAttributeName);
	}

	private TokenType ExecCharacterReference()
	{
		var input = Consume();

		if (input == ';')
		{
			ReadValue = _source[StartIndex.._position];

			if (EntityDecoder.TryDecode(ReadValue[1..], out Rune decoded))
			{
				CharacterEntity = decoded;
				return SwitchTo(State.Data, TokenType.CharacterEntity);
			}

			return SwitchTo(State.Data, TokenType.Text);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		// This is not part of the specification, but this avoids malformed entities consume the rest entire text.
		var length = _position - StartIndex;
		var validInput = false;

		if (length == 2 && input == '#')
		{
			validInput = true;
		}
		else if (length == 3 && _source[StartIndex + 1] == '#' && (input == 'x' || input == 'X'))
		{
			validInput = true;
		}
		else if (char.IsAsciiLetterOrDigit((char)input))
		{
			validInput = true;
		}

		if (!validInput)
		{
			return ReconsumeIn(State.Data);
		}

		return TokenType.None;
	}
}
