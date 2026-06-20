using Godot;
using System.Text;

namespace Spectrum.RichText.Parsing;

partial struct Tokenizer
{
	private TokenType ExecData()
	{
		int input = Consume();

		if (input is '&' or '<')
		{
			if (_position - 1 > StartIndex)
			{
				ReadValue = _inputText[StartIndex..--_position];
				return TokenType.Text;
			}

			return SwitchTo(input == '&' ? State.CharacterReference : State.TagOpen);
		}

		if (input == Eof)
		{
			if (_position > StartIndex)
			{
				ReadValue = _inputText[StartIndex.._position];
				return TokenType.Text;
			}

			return TokenType.Eof;
		}

		return TokenType.None;
	}

	private TokenType ExecTagOpen()
	{
		int input = Consume();

		if (input == '/')
		{
			return SwitchTo(State.EndTagOpen);
		}

		if (input is ' ' or Eof)
		{
			ReadValue = _inputText[StartIndex.._position];
			return SwitchTo(State.Data, TokenType.Text);
		}

		StartIndex = _position - 1;
		return ReconsumeIn(State.TagName);
	}

	private TokenType ExecEndTagOpen()
	{
		int input = Consume();

		if (input is ' ' or Eof)
		{
			ReadValue = _inputText[StartIndex.._position];
			return SwitchTo(State.Data, TokenType.Text);
		}

		StartIndex = _position - 1;
		return ReconsumeIn(State.TagName);
	}

	private TokenType ExecTagName()
	{
		int input = Consume();

		if (input is ' ' or '/' or '>')
		{
			ReadValue = _inputText[StartIndex..(_position - 1)];

			if (input == ' ')
			{
				return SwitchTo(State.BeforeAttributeName);
			}

			if (input == '/')
			{
				return SwitchTo(State.SelfClosingStartTag);
			}

			return SwitchTo(State.Data, (_inputText[StartIndex - 1] != '/') ? TokenType.StartTag : TokenType.EndTag);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		return TokenType.None;
	}

	private TokenType ExecBeforeAttributeName()
	{
		int input = Consume();

		if (input == ' ')
		{
			return TokenType.None;
		}

		if (input is '/' or '>' or Eof)
		{
			return ReconsumeIn(State.AfterAttributeName);
		}

		_currentAttributeNameStart = _position - 1;

		if (input == '=')
		{
			return SwitchTo(State.AttributeName);
		}

		return ReconsumeIn(State.AttributeName);
	}

	private TokenType ExecAttributeName()
	{
		int input = Consume();

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
		int input = Consume();

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
			return SwitchTo(State.Data, TokenType.StartTag);
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
		int input = Consume();

		if (input == ' ')
		{
			return TokenType.None;
		}

		if (input == '>')
		{
			AppendCurrentAttribute();
			return SwitchTo(State.Data, TokenType.StartTag);
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
		int input = Consume();

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
		int input = Consume();

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

			return SwitchTo(State.Data, TokenType.StartTag);
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
		int input = Consume();

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
			return SwitchTo(State.Data, TokenType.StartTag);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		return ReconsumeIn(State.BeforeAttributeName);
	}

	private TokenType ExecSelfClosingStartTag()
	{
		int input = Consume();

		if (input == '>')
		{
			IsSelfClosing = true;
			return SwitchTo(State.Data, TokenType.StartTag);
		}

		if (input == Eof)
		{
			return TokenType.Eof;
		}

		return ReconsumeIn(State.BeforeAttributeName);
	}

	private TokenType ExecCharacterReference()
	{
		int input = Consume();

		if (input == ';')
		{
			ReadValue = _inputText[StartIndex.._position];

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

		return TokenType.None;
	}
}
