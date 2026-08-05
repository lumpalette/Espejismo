using Godot;

namespace Espejismo.Core.RichText.Shaping;

// One-shot shaping engine that reads a sequence of shaped items and produces renderable glyphs.
internal readonly struct Shaper(TextServer TS, TextData text, LayoutOptions layout, OutputBuffers output)
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

	public void Shape()
	{
		output.Glyphs.Clear();
		output.Lines.Clear();
		output.Paragraphs.Clear();
		
		WriteParagraphs();

		if (output.Paragraphs.Count > 0)
		{
			GenerateGlyphs();
		}
	}

	private void WriteParagraphs()
	{
		if (text.Items.Length == 0)
		{
			return;
		}

		var shaped = CreateParagraphRid();
		var alignment = layout.BaseAlignment;
		var hasContent = false;
		var isIndependent = true;

		for (var i = 0; i < text.Items.Length; i++)
		{
			var item = text.Items[i];

			switch (item.Type)
			{
				case ShapeItemType.Run:
					var font = ResolveFont(item.Style, out var fontSize);
					TS.ShapedTextAddString(shaped, item.Text, font.GetRids(), fontSize, meta: i);
					hasContent = true;
					isIndependent = true;
					break;

				case ShapeItemType.Texture:
					TS.ShapedTextAddObject(shaped, i, item.Texture.GetSize(), item.TextureAlignment);
					hasContent = true;
					isIndependent = true;
					break;

				case ShapeItemType.Marker:
					TS.ShapedTextAddObject(shaped, i, Vector2.Zero);
					hasContent = true;
					isIndependent = true;
					break;

				case ShapeItemType.Break:
					if (isIndependent)
					{
						FlushParagraph(shaped, alignment, hasContent);
						shaped = CreateParagraphRid();
						hasContent = false;
					}

					isIndependent = true;
					break;

				case ShapeItemType.Align:
					if (hasContent)
					{
						FlushParagraph(shaped, alignment, hasContent);
						shaped = CreateParagraphRid();
						hasContent = false;
						isIndependent = false;
					}

					alignment = item.Alignment ?? layout.BaseAlignment;
					break;
			}
		}

		if (isIndependent)
		{
			FlushParagraph(shaped, alignment, hasContent);
		}
		else
		{
			TS.FreeRid(shaped);
		}
	}

	private Rid CreateParagraphRid()
	{
		return TS.CreateShapedText(layout.Direction, layout.Orientation);
	}

	private void FlushParagraph(Rid shaped, HorizontalAlignment alignment, bool hasContent)
	{
		output.Paragraphs.Add(new Paragraph(shaped, alignment, hasContent));
	}

	private Font ResolveFont(in TextStyle current, out int size)
	{
		var @base = text.BaseStyle;
		var @default = ResourceDB.DefaultStyle;

		var font = current.Font ?? @base.Font ?? @default.Font;
		var spcX = current.LetterSpacing ?? @base.LetterSpacing ?? @default.LetterSpacing;
		var spcY = current.LineSpacing ?? @base.LineSpacing ?? @default.LineSpacing;

		if (spcX != 0 || spcY != 0)
		{
			font = new FontVariation
			{
				BaseFont = font,
				SpacingGlyph = spcX,
				SpacingBottom = spcY
			};
		}

		size = current.FontSize ?? @base.FontSize ?? @default.FontSize;
		return font;
	}

	private void GenerateGlyphs()
	{
		LineSpan line = default;

		foreach (var paragraph in output.Paragraphs)
		{
			if (!paragraph.HasContent)
			{
				float ascent, descent;

				if (line.Height > 0f)
				{
					ascent = line.Ascent;
					descent = line.Descent;
				}
				else
				{
					// This unnecessary allocates a FontVariation, but I guess it doesn't matter too much...
					var font = ResolveFont(text.BaseStyle, out var fontSize).GetRids()[0];
					ascent  = (float)TS.FontGetAscent(font, fontSize);
					descent = (float)TS.FontGetDescent(font, fontSize);
				}

				var empty = new LineSpan(
					glyphs:    output.Glyphs,
					start:     output.Glyphs.Count,
					length:    0,
					width:     0f,
					ascent:    ascent,
					descent:   descent,
					alignment: paragraph.Alignment);

				output.Lines.Add(empty);

				TS.FreeRid(paragraph.Shaped);
				continue;
			}

			// Extract the glyphs from every line range.
			var breaks = CalculateLineBreaks(paragraph.Shaped);

			for (var i = 0; i < breaks.Length; i += 2)
			{
				var shaped = TS.ShapedTextSubstr(paragraph.Shaped, breaks[i], breaks[i + 1] - breaks[i]);
				
				if (paragraph.Alignment == HorizontalAlignment.Fill && layout.MaxWidth > 0)
				{
					TS.ShapedTextFitToWidth(shaped, layout.MaxWidth);
				}

				var glyphCount = output.Glyphs.Count;

				foreach (var g in TS.ShapedTextGetGlyphs(shaped))
				{
					ProcessRawGlyph(g, shaped);
				}

				line = new LineSpan(
					glyphs:    output.Glyphs,
					start:     glyphCount,
					length:    output.Glyphs.Count - glyphCount,
					width:     (float)TS.ShapedTextGetWidth(shaped),
					ascent:    (float)TS.ShapedTextGetAscent(shaped),
					descent:   (float)TS.ShapedTextGetDescent(shaped),
					alignment: paragraph.Alignment);

				output.Lines.Add(line);

				TS.FreeRid(shaped);
			}

			TS.FreeRid(paragraph.Shaped);
		}
	}

	private int[] CalculateLineBreaks(Rid shaped)
	{
		var width = layout.MaxWidth;

		if (width <= 0)
		{
			width = float.MaxValue;
		}

		var breakFlags = TextServer.LineBreakFlag.WordBound
			| TextServer.LineBreakFlag.Adaptive
			| TextServer.LineBreakFlag.TrimStartEdgeSpaces
			| TextServer.LineBreakFlag.TrimEndEdgeSpaces;

		return TS.ShapedTextGetLineBreaks(shaped, width, start: 0, breakFlags);
	}

	private void ProcessRawGlyph(Godot.Collections.Dictionary g, Rid shaped)
	{
		var @base = text.BaseStyle;
		var @default = ResourceDB.DefaultStyle;

		var start = (int)g["start"];
		var end = (int)g["end"];
		var flags = (TextServer.GraphemeFlag)(long)g["flags"];
		var advance = (float)g["advance"];
		var spanIndex = (int)g["span_index"];

		if (flags.HasFlag(TextServer.GraphemeFlag.EmbeddedObject))
		{
			var key = (int)TS.ShapedGetSpanObject(shaped, spanIndex);
			var item = text.Items[key];

			if (item.Type == ShapeItemType.Texture)
			{
				output.Glyphs.Add(new Glyph
				{
					Start = start,
					End = end,
					Count = 1,
					Repeat = 1,
					Flags = flags,
					Advance = advance,
					IconTexture = item.Texture,
					Color = item.Style.Color ?? @base.Color ?? @default.Color,
					Effect = item.Style.Effect ?? @base.Effect ?? @default.Effect
				});
			}
			else
			{
				output.Markers.Add(new TextMarker(item.Text, item.Attributes, output.Glyphs.Count));
			}
		}
		else
		{
			var runIndex = (int)TS.ShapedGetSpanMeta(shaped, spanIndex);
			var runStyle = text.Items[runIndex].Style;

			// Oh my god bruh.
			output.Glyphs.Add(new Glyph
			{
				Start = start,
				End = end,
				Count = (byte)g["count"],
				Repeat = (byte)g["repeat"],
				Flags = flags,
				Offset = (Vector2)g["offset"],
				Advance = advance,
				Index = (int)g["index"],
				Font = (Rid)g["font_rid"],
				FontSize = (int)g["font_size"],
				Color = runStyle.Color ?? @base.Color ?? @default.Color,
				Effect = runStyle.Effect ?? @base.Effect ?? @default.Effect,
				ShadowSize = runStyle.ShadowSize ?? @base.ShadowSize ?? @default.ShadowSize,
				ShadowColor = runStyle.ShadowColor ?? @base.ShadowColor ?? @default.ShadowColor,
				ShadowOffset = runStyle.ShadowOffset ?? @base.ShadowOffset ?? @default.ShadowOffset,
				OutlineSize = runStyle.OutlineSize ?? @base.OutlineSize ?? @default.OutlineSize,
				OutlineColor = runStyle.OutlineColor ?? @base.OutlineColor ?? @default.OutlineColor,
			});
		}
	}
}
