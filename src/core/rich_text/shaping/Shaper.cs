using Godot;
using System.Collections.Generic;

namespace Espejismo.Core.RichText.Shaping;

// One-shot shaping engine that reads a sequence of shaped items and produces renderable glyphs.
internal readonly struct Shaper()
{
	/* Nunca suelo escribir acerca de mis experiencias diseñando un sistema cuando programo, y mucho menos hacerlo
	 * dentro del código fuente, pero la sensación que tuve al escribir esto se puede resumir con la frase aquella
	 * mística y poderosa señora, intentando deducir cómo se opera una cámara de teléfono móvil
	 * 
	 * 
	 * 
	 * 
	 * 
	 * no puedo martha
	 */

	// Input.
	public required TextServer TS { get; init; }
	public required ShapeItem[] Items { get; init; }
	public required Dictionary<TextStyle, ResolvedStyle> StyleMap { get; init; }

	// Layout options.
	public required float MaxWidth { get; init; }
	public required HorizontalAlignment BaseAlignment { get; init; }
	public required TextServer.Direction Direction { get; init; }
	public required TextServer.Orientation Orientation { get; init; }

	// Output, the lists must be cleared by caller.
	public required List<Glyph> Glyphs { get; init; }
	public required List<LineSpan> Lines { get; init; }
	public required List<TextMarker> Markers { get; init; }
	public required List<Paragraph> Paragraphs { get; init; }

	// Fallback values.
	public required Font DefaultFont { get; init; }
	public required long DefaultFontSize { get; init; }

	public void Shape()
	{
		WriteParagraphs();

		if (Paragraphs.Count > 0)
		{
			WriteLines();
		}
	}

	private void WriteParagraphs()
	{
		if (Items.Length == 0)
		{
			return;
		}

		var paragraph = new Paragraph(TS, Direction, Orientation) { Alignment = BaseAlignment };
		var independent = true;
		
		for (var i = 0; i < Items.Length; i++)
		{
			var item = Items[i];

			switch (item.Type)
			{
				case ShapeItemType.Run:
					var resolved = StyleMap[item.Style];
					var fonts = resolved.Font.GetRids();
					var fontSize = resolved.FontSize;

					TS.ShapedTextAddString(paragraph.Shaped, item.Text, fonts, fontSize, meta: i);
					break;

				case ShapeItemType.Texture:
					TS.ShapedTextAddObject(paragraph.Shaped, i, item.Texture.GetSize(), item.TextureAlignment);
					break;

				case ShapeItemType.Marker:
					TS.ShapedTextAddObject(paragraph.Shaped, i, Vector2.Zero);
					break;

				case ShapeItemType.Break:
					if (independent)
					{
						Paragraphs.Add(paragraph);
						paragraph = new Paragraph(TS, Direction, Orientation) { Alignment = paragraph.Alignment };
					}

					independent = true;
					break;

				case ShapeItemType.Align:
					var alignment = item.Alignment ?? BaseAlignment;

					if (paragraph.HasContent)
					{
						Paragraphs.Add(paragraph);
						paragraph = new Paragraph(TS, Direction, Orientation) { Alignment = alignment };
					}
					else
					{
						paragraph = paragraph with { Alignment = alignment };
					}

					independent = false; // stupid
					break;
			}

			if (item.Type is ShapeItemType.Run or ShapeItemType.Texture or ShapeItemType.Marker)
			{
				paragraph = paragraph with { HasContent = true };
				independent = true;
			}
		}

		if (independent)
		{
			Paragraphs.Add(paragraph);
		}
		else
		{
			TS.FreeRid(paragraph.Shaped);
		}
	}

	private void WriteLines()
	{
		for (var i = 0; i < Paragraphs.Count; i++)
		{
			var paragraph = Paragraphs[i];

			if (paragraph.HasContent)
			{
				var breaks = CalculateLineBreaks(paragraph.Shaped);

				for (var j = 0; j < breaks.Length; j += 2)
				{
					var lineShaped = SplitParagraph(paragraph, breaks[j], breaks[j + 1] - breaks[j]);
					var initialGlyphCount = Glyphs.Count;

					foreach (var g in TS.ShapedTextGetGlyphs(lineShaped))
					{
						ProcessRawGlyph(g, lineShaped);
					}
					
					// Vertical spacing shouldn't be added to the last line of the text.
					var descent = (float)TS.ShapedTextGetDescent(lineShaped);
					var lastLine = i + 1 >= Paragraphs.Count && j + 2 >= breaks.Length;

					if (lastLine)
					{
						var spcY = (float)TS.ShapedTextGetSpacing(lineShaped, TextServer.SpacingType.Bottom);

						if (spcY > 0f)
						{
							descent -= spcY;
						}
					}

					// Generate the final line.
					Lines.Add(new LineSpan(
						glyphs:    Glyphs,
						start:     initialGlyphCount,
						length:    Glyphs.Count - initialGlyphCount,
						width:     (float)TS.ShapedTextGetWidth(lineShaped),
						ascent:    (float)TS.ShapedTextGetAscent(lineShaped),
						descent:   descent,
						alignment: paragraph.Alignment));

					TS.FreeRid(lineShaped);
				}
			}
			else
			{
				InsertEmptyLine(paragraph.Alignment);
			}

			TS.FreeRid(paragraph.Shaped);
		}
	}

	private void InsertEmptyLine(HorizontalAlignment alignment)
	{
		float ascent, descent;
		
		if (Lines.Count == 0)
		{
			// No previous line, so we have to make some bullshit metrics by ourselves.
			var font = DefaultFont.GetRids()[0];

			ascent = (float)TS.FontGetAscent(font, DefaultFontSize);
			descent = (float)TS.FontGetDescent(font, DefaultFontSize);
		}
		else
		{
			// Just copy the previous line bro it doesn't matter.
			var previousLine = Lines[^1];

			ascent = previousLine.Ascent;
			descent = previousLine.Descent;
		}

		var emptyLine = new LineSpan(Glyphs, Glyphs.Count, 0, 0f, ascent, descent, alignment);
		Lines.Add(emptyLine);
	}

	private int[] CalculateLineBreaks(Rid shaped)
	{
		var width = (MaxWidth > 0) ? MaxWidth : float.MaxValue; 

		var breakFlags = TextServer.LineBreakFlag.WordBound
			| TextServer.LineBreakFlag.Adaptive
			| TextServer.LineBreakFlag.TrimStartEdgeSpaces
			| TextServer.LineBreakFlag.TrimEndEdgeSpaces;

		return TS.ShapedTextGetLineBreaks(shaped, width, start: 0, breakFlags);
	}

	private Rid SplitParagraph(Paragraph paragraph, int start, int length)
	{
		var lineShaped = TS.ShapedTextSubstr(paragraph.Shaped, start, length);

		if (paragraph.Alignment == HorizontalAlignment.Fill && MaxWidth > 0)
		{
			TS.ShapedTextFitToWidth(lineShaped, MaxWidth);
		}

		return lineShaped;
	}

	private void ProcessRawGlyph(Godot.Collections.Dictionary g, Rid lineShaped)
	{
		var flags = (TextServer.GraphemeFlag)(long)g["flags"];
		var spanIndex = (int)g["span_index"];

		if (flags.HasFlag(TextServer.GraphemeFlag.EmbeddedObject))
		{
			var key = (int)TS.ShapedGetSpanObject(lineShaped, spanIndex);
			var item = Items[key];

			if (item.Type == ShapeItemType.Texture)
			{
				AddIconGlyph(g, StyleMap[item.Style].Style, item.Texture);
			}
			else
			{
				Markers.Add(new TextMarker(item.Text, item.Attributes, Glyphs.Count));
			}
		}
		else
		{
			var itemIndex = (int)TS.ShapedGetSpanMeta(lineShaped, spanIndex);
			var item = Items[itemIndex];

			AddCharGlyph(g, StyleMap[item.Style].Style);
		}
	}

	private void AddCharGlyph(Godot.Collections.Dictionary g, GlyphStyle style)
	{
		Glyphs.Add(new Glyph
		{
			Start = (int)g["start"],
			End = (int)g["end"],
			Index = (ushort)g["index"],
			Font = (Rid)g["font_rid"],
			FontSize = (ushort)g["font_size"],
			Style = style,
			Advance = (float)g["advance"],
			Offset = (Vector2)g["offset"],
			Count = (byte)g["count"],
			Repeat = (byte)g["repeat"],
			Flags = (ushort)g["flags"]
		});
	}

	private void AddIconGlyph(Godot.Collections.Dictionary g, GlyphStyle style, Texture2D tex)
	{
		Glyphs.Add(new Glyph
		{
			Start = (int)g["start"],
			End = (int)g["end"],
			IconTexture = tex,
			Style = style,
			Advance = (float)g["advance"],
			Offset = (Vector2)g["offset"],
			Count = (byte)g["count"],
			Repeat = (byte)g["repeat"],
			Flags = (ushort)g["flags"]
		});
	}
}
