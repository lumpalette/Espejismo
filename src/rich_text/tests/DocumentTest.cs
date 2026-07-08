using Godot;
using Spectrum.RichText.Parsing;

namespace Spectrum.RichText.Tests;

internal sealed partial class DocumentTest : Godot.Node
{
	[Export(PropertyHint.MultilineText)]
	private string _input = "<color value=black>La, la.<wait time=1s/>\nTime to wake\nup and <color value=red>smell</color>\nthe<wait time=1.33s/> pain.</color><next/>";

	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey key || !key.Pressed || key.Keycode != Key.R)
		{
			return;
		}

		GD.Print(Document.Parse(_input).ToString());
	}
}
