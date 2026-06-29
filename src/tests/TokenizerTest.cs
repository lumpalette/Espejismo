using Godot;
using Spectrum.RichText;
using Spectrum.RichText.Parsing;

namespace Spectrum.Tests;

internal sealed partial class TokenizerTest : Godot.Node
{
	[Export(PropertyHint.MultilineText)]
	private string _text = "Etiquete <tag ";

	[Export]
	private TextStyle? _style;

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey key || !key.Pressed || key.Keycode != Key.R)
		{
			return;
		}

		var tokenizer = new Tokenizer(_text);

		while (tokenizer.Read())
		{
			if (true)
			{
				var entity = tokenizer.CharacterEntity == default ? "null" : tokenizer.CharacterEntity.ToString();

				var s = $"Token[{tokenizer.TokenType}]:\n    Value: '{tokenizer.ReadValue}'\n    Start: {tokenizer.StartIndex}\n    Entity: '{entity}'\n    Self-closing: {tokenizer.IsSelfClosing}\n    Attributes: {tokenizer.Attributes.Length}";
				
				if (tokenizer.Attributes.Length > 0)
				{
					for (int i = 0; i < tokenizer.Attributes.Length; i++)
					{
						s += '\n';

						var attribute = tokenizer.Attributes[i];
						var name = _text.Substring(attribute.NameStart, attribute.NameLength);
						var value = _text.Substring(attribute.ValueStart, attribute.ValueLength);

						s += $"        Name: '{name}', Value: '{value}'";
					}
				}
				
				GD.Print(s);
			}
		}
	}
}
