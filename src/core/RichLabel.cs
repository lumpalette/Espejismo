using Godot;

namespace Espejismo.Core.RichText.Nodes;

/// <summary>
/// Minimal Label-like Control used to smoke-test the rich-text pipeline. Not feature-complete.
/// </summary>
[Tool]
[GlobalClass]
public partial class RichLabel : Control
{
	private readonly TextServer _TS = TextServerManager.GetPrimaryInterface();
	private Text? _text;

	[Export(PropertyHint.MultilineText)]
	public string Content
	{
		get;
		set
		{
			field = value ?? string.Empty;
			RebuildText();
		}
	} = string.Empty;

	[Export]
	public StyleTemplate? BaseStyle
	{
		get;
		set
		{
			if (field is not null)
			{
				field.Changed -= OnBaseStyleChanged;
			}

			field = value;

			if (field is not null)
			{
				field.Changed += OnBaseStyleChanged;
			}

			RebuildText();
		}
	}

	public override void _Ready()
	{
		RebuildText();
	}

	public override void _ExitTree()
	{
		if (BaseStyle is not null)
		{
			BaseStyle.Changed -= OnBaseStyleChanged;
		}
	}

	public override void _Notification(int what)
	{
		if (what != NotificationResized || _text is null)
		{
			return;
		}

		_text.Width = Size.X;
		QueueRedraw();
	}

	private void OnBaseStyleChanged()
	{
		RebuildText();
	}

	private void RebuildText()
	{
		if (string.IsNullOrEmpty(Content))
		{
			_text = null;
			QueueRedraw();
			return;
		}

		var style = (BaseStyle is not null) ? new TextStyle(BaseStyle) : new TextStyle();

		_text = Text.Parse(Content, style);
		_text.Width = Size.X;

		QueueRedraw();
	}

	public override Vector2 _GetMinimumSize()
	{
		if (_text is null)
		{
			return Vector2.Zero;
		}

		var height = 0f;
		var width = 0f;

		foreach (var line in _text.Lines)
		{
			height += line.Height;
			width = Mathf.Max(width, line.Width);
		}

		return new Vector2(width, height);
	}

	public override void _Draw()
	{
		if (_text is null)
		{
			return;
		}

		var canvas = GetCanvasItem();
		var y = 0f;

		foreach (var line in _text.Lines)
		{
			y += line.Ascent;

			var x = line.Alignment switch
			{
				HorizontalAlignment.Center => (Size.X - line.Width) * 0.5f,
				HorizontalAlignment.Right => Size.X - line.Width,
				_ => 0f
			};

			foreach (var glyph in line)
			{
				var pos = new Vector2(x, y) + glyph.Offset;

				if (glyph.IconTexture is not null)
				{
					DrawTexture(glyph.IconTexture, pos, glyph.Color);
				}
				else if (glyph.Font.IsValid)
				{
					if (glyph.OutlineSize > 0)
					{
						_TS.FontDrawGlyphOutline(glyph.Font, canvas, glyph.FontSize, glyph.OutlineSize, pos, glyph.Index, glyph.OutlineColor);
					}

					if (glyph.ShadowSize > 0)
					{
						_TS.FontDrawGlyph(glyph.Font, canvas, glyph.FontSize, pos + glyph.ShadowOffset, glyph.Index, glyph.ShadowColor);
					}

					_TS.FontDrawGlyph(glyph.Font, canvas, glyph.FontSize, pos, glyph.Index, glyph.Color);
				}

				x += glyph.Advance;
			}

			y += line.Descent;
		}
	}
}
