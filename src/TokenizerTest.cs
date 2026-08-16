using Espejismo.Core.RichText.Parsing;
using Godot;
using System;

namespace Espejismo;

internal partial class TokenizerTest : Godot.Node
{
	[Export(PropertyHint.MultilineText)]
	private string _text = string.Empty;

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event is not InputEventKey { PhysicalKeycode: Key.Z, Pressed: true, Echo: false } key)
		{
			return;
		}

		var t = new Tokenizer(_text);

		while (t.Read())
		{
			var msg = $"Token[{t.TokenType}]: ReadValue = '{t.ReadValue}', StartIndex = {t.StartIndex}";

			if (t.TokenType == TokenType.CharacterEntity)
			{
				msg += $", CharacterEntity = '{t.CharacterEntity}'";
			}
			else if (t.TokenType == TokenType.StartTag)
			{
				msg += $", IsSelfClosing = {t.IsSelfClosing}, Attributes ({t.Attributes.Length}) = [\n";

				foreach (var attr in t.Attributes)
				{
					var name = _text.AsSpan(attr.NameStart, attr.NameLength);
					var value = _text.AsSpan(attr.ValueStart, attr.ValueLength);
					msg += $"  Name = '{name}', Value = '{value}'\n";
				}

				msg += ']';
			}

			GD.Print(msg);
			GD.Print();
		}
	}
}
