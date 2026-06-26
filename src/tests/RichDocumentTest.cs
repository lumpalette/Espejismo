using Godot;
using Spectrum.RichText.Parsing;

namespace Spectrum.Tests;

internal partial class RichDocumentTest : Godot.Node
{
	[Export(PropertyHint.MultilineText)]
	private string _input = "<color value=black>La, la.<wait time=1s/>\nTime to wake\nup and <color value=red>smell</color>\nthe<wait time=1.33s/> pain.</color><next/>";

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey key)
		{
			return;
		}

		if (key.Pressed && key.Keycode == Key.R)
		{
			var doc = new Document();
			doc.Parse(_input);
			GD.Print(doc);
		}
	}
}
