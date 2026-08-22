using Godot;
using System.Collections.ObjectModel;

namespace Espejismo.Core.RichText.Nodes;

/// <summary>
/// Minimal Label-like Control used to smoke-test the rich-text pipeline. Not feature-complete.
/// </summary>
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

	[ExportGroup("Alignment")]
	[Export]
	public HorizontalAlignment HorizontalAlignment
	{
		get;
		set
		{
			field = value;

			if (_text is not null)
			{
				_text.Alignment = value;
			}

			QueueRedraw();
		}
	}

	[Export]
	public VerticalAlignment VerticalAlignment
	{
		get;
		set
		{
			field = value;
			QueueRedraw();
		}
	}

	public override void _Ready()
	{
		RebuildText();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
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

		var style = (BaseStyle is not null) ? BaseStyle.Create() : new TextStyle();

		_text = Text.Parse(Content, style);
		_text.Width = Size.X;
		_text.Alignment = HorizontalAlignment;

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

	private float GetContentHeight()
	{
		if (_text is null || _text.Lines.Length == 0)
		{
			return 0f;
		}

		var totalHeight = 0f;

		foreach (var line in _text.Lines)
		{
			totalHeight += line.Height;
		}

		totalHeight -= _text.Lines[^1].Leading;

		return totalHeight;
	}

	private float GetStartY()
	{
		if (VerticalAlignment == VerticalAlignment.Top || _text is null)
		{
			return 0f;
		}

		var totalHeight = GetContentHeight();

		return Mathf.Round(VerticalAlignment switch
		{
			VerticalAlignment.Center => (Size.Y - totalHeight) * 0.5f,
			VerticalAlignment.Bottom => Size.Y - totalHeight,
			_ => 0f
		});
	}

	public override void _Draw()
	{
		if (_text is null)
		{
			return;
		}

		var canvas = GetCanvasItem();
		var y = 0f;
		var lineGap = 0f;
		var isFirstLine = true;

		if (VerticalAlignment == VerticalAlignment.Fill)
		{
			var lineCount = _text.Lines.Length;

			if (lineCount > 1)
			{
				lineGap = (Size.Y - GetContentHeight()) / (lineCount - 1);
			}
		}
		else
		{
			y = GetStartY();
		}

		foreach (var line in _text.Lines)
		{
			if (!isFirstLine)
			{
				y += lineGap;
			}

			isFirstLine = false;
			y += line.Ascent;

			var x = Mathf.Round(line.Alignment switch
			{
				HorizontalAlignment.Center => (Size.X - line.Width) * 0.5f,
				HorizontalAlignment.Right => Size.X - line.Width,
				_ => 0f
			});

			var glyphCount = 0;

			foreach (var glyph in line)
			{
				var pos = new Vector2(x, y) + glyph.Offset;
				var col = glyph.Style.Color;

				if (glyph.Style.Effect is not null)
				{
					var trans = new GlyphTransform
					{
						Glyph = glyph,
						Index = 0,
						LinePosition = glyphCount / (float)line.Length,
						LineLength = line.Length,
						ElapsedTime = Time.GetTicksMsec() / 1000f,
						Color = col
					};

					glyph.Style.Effect.Process(ref trans);

					if (trans.Visibility == GlyphVisibility.Omitted)
					{
						continue;
					}
					else if (trans.Visibility == GlyphVisibility.Invisible)
					{
						x += glyph.Advance;
						glyphCount++;
						continue;
					}

					pos += trans.Offset;
					col = trans.Color;
				}

				if (glyph.IconTexture is not null)
				{
					DrawTextureRect(glyph.IconTexture, new(pos, glyph.IconSize), false, col);
				}
				else if (glyph.Font.IsValid)
				{
					if (glyph.Style.OutlineSize > 0)
					{
						_TS.FontDrawGlyphOutline(glyph.Font, canvas, glyph.FontSize, glyph.Style.OutlineSize, pos, glyph.Index, glyph.Style.OutlineColor);
					}

					if (glyph.Style.ShadowSize > 0)
					{
						_TS.FontDrawGlyph(glyph.Font, canvas, glyph.Style.ShadowSize, pos + glyph.Style.ShadowOffset, glyph.Index, glyph.Style.ShadowColor);
					}

					_TS.FontDrawGlyph(glyph.Font, canvas, glyph.FontSize, pos, glyph.Index, col);
				}

				x += glyph.Advance;
				glyphCount++;
			}

			y += line.Descent + line.Leading;
		}
	}
}
