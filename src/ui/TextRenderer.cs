using Godot;

namespace Espejismo.UI;

[GlobalClass, Tool]
public partial class TextRenderer : Control
{
	[Export(PropertyHint.MultilineText)]
	public string Text { get; set; }
}
