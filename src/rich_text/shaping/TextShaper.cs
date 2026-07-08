using Godot;
using System.Xml.Schema;

namespace Spectrum.RichText.Shaping;

internal readonly ref struct TextShaper
{
	public required TextServer Server { get; init; }

	public required TextData Text { get; init; }

	public required BufferData Buffer { get; init; }

	public required LayoutOptions Layout { get; init; }

	public void Shape()
	{
		Buffer.Lines.Clear();
		Buffer.Glyphs.Clear();
		Buffer.Blocks.Clear();

		BuildShapedBlocks();
		
		if (Buffer.Blocks.Count > 0)
		{
			ExtractGlyphs();
		}
	}

	private void BuildShapedBlocks()
	{
		var sequenceIndex = 0;

		foreach (var range in Layout.Alignments)
		{
			// Is there a sequence item that preceds the current range?
			if (sequenceIndex < range.Start)
			{
				ProcessSequenceRange(sequenceIndex, range.Start, Layout.BaseAlignment);
			}

			ProcessSequenceRange(range.Start, range.End, range.Alignment);
			sequenceIndex = range.End;
		}

		if (sequenceIndex < Text.ParsedSequence.Length)
		{
			ProcessSequenceRange(sequenceIndex, Text.ParsedSequence.Length, Layout.BaseAlignment);
		}
	}

	private void ProcessSequenceRange(int sequenceStart, int sequenceEnd, HorizontalAlignment alignment)
	{
		var current = new ShapedBlock(Server, Layout.Direction, Layout.Orientation, alignment);

		for (var i = sequenceStart; i < sequenceEnd; i++)
		{
			var item = Text.ParsedSequence[i];

			if (item.IsRun)
			{
				var run = item.Run;
				var textLen = run.Text.Length;
				var textPos = 0;

				for (var j = 0; j < textLen; j++)
				{
					if (run.Text[j] != '\n')
					{
						continue;
					}

					if (textPos < j)
					{
						AddRunToShaped(run, textPos, j, current.Shaped);
					}

					Buffer.Blocks.Add(current);
					current = new ShapedBlock(Server, Layout.Direction, Layout.Orientation, alignment);

					textPos = j + 1;
				}

				if (textPos < textLen)
				{
					AddRunToShaped(run, textPos, textLen, current.Shaped);
				}
			}
			else
			{
				// TODO: implement icons!
			}
		}

		Buffer.Blocks.Add(current);
	}

	private void AddRunToShaped(in TextRun run, int start, int end, Rid shaped)
	{
		var text = run.Text[start..end];

		var runStyle = run.StyleOverride;
		var baseStyle = Text.BaseStyle;

		var font = runStyle.Font ?? baseStyle.Font!;
		var fontSize = runStyle.FontSize ?? baseStyle.FontSize;
		var spacingX = runStyle.LetterSpacing ?? baseStyle.LetterSpacing;
		var spacingY = runStyle.LineSpacing ?? baseStyle.LineSpacing;

		if (spacingX != 0 || spacingY != 0)
		{
			font = new FontVariation
			{
				BaseFont = font,
				SpacingGlyph = spacingX,
				SpacingBottom = spacingY
			};
		}
		
		Server.ShapedTextAddString(shaped, text, font.GetRids(), fontSize, meta: run.SequenceIndex);
	}


	private void ExtractGlyphs()
	{
		var current = new TextLine();

		foreach (var block in Buffer.Blocks)
		{
			var breaks = CalculateLineBreaks(block.Shaped);
			
			// Shitass (and I'm not sure if it even works) hack for handling empty blocks.
			if (breaks.Length > 0 && breaks[0] == breaks[^1])
			{
				float ascent, descent;

				if (current.Height > 0f)
				{
					ascent = current.Ascent;
					descent = current.Descent;
				}
				else
				{
					var font = Text.BaseStyle.Font!.GetRids()[0];
					var fontSize = Text.BaseStyle.FontSize;

					ascent = (float)Server.FontGetAscent(font, fontSize);
					descent = (float)Server.FontGetDescent(font, fontSize) + Text.BaseStyle.LineSpacing;
				}

				var empty = new TextLine(Buffer.Glyphs.Count, 0, ascent, descent, 0f, block.Alignment);
				Buffer.Lines.Add(empty);
			}

			// Extract the glyphs from every line in the block.
			for (var i = 0; i < breaks.Length; i += 2)
			{
				var shaped = Server.ShapedTextSubstr(block.Shaped, breaks[i], breaks[i + 1] - breaks[i]);
				var start = Buffer.Glyphs.Count;
				
				ProcessShapedLine(shaped, block.Alignment);

				current = new TextLine(
					start:     start,
					length:    Buffer.Glyphs.Count - start,
					ascent:    (float)Server.ShapedTextGetAscent(shaped),
					descent:   (float)Server.ShapedTextGetDescent(shaped),
					width:     (float)Server.ShapedTextGetWidth(shaped),
					alignment: block.Alignment
				);

				Buffer.Lines.Add(current);
				
				Server.FreeRid(shaped);
			}

			Server.FreeRid(block.Shaped);
		}
	}

	// Gets the line ranges from the specified shaped text using the default break flags.
	private int[] CalculateLineBreaks(Rid shaped)
	{
		var width = Layout.Width;

		if (width <= 0f)
		{
			width = float.MaxValue;
		}

		var breakFlags = TextServer.LineBreakFlag.Mandatory
			| TextServer.LineBreakFlag.WordBound
			| TextServer.LineBreakFlag.Adaptive
			| TextServer.LineBreakFlag.TrimStartEdgeSpaces
			| TextServer.LineBreakFlag.TrimEndEdgeSpaces;
		
		return Server.ShapedTextGetLineBreaks(shaped, width, start: 0, breakFlags);
	}

	private void ProcessShapedLine(Rid shaped, HorizontalAlignment alignment)
	{
		if (alignment == HorizontalAlignment.Fill && Layout.Width > 0)
		{
			Server.ShapedTextFitToWidth(shaped, Layout.Width);
		}

		var glyphs = Server.ShapedTextGetGlyphs(shaped);
		var glyphCount = glyphs.Count;

		for (var i = 0; i < glyphCount; i++)
		{
			ProcessRawGlyph(glyphs[i], shaped);
		}
	}

	private void ProcessRawGlyph(Godot.Collections.Dictionary glyph, Rid shaped)
	{
		var flags = (TextServer.GraphemeFlag)(long)glyph["flags"];

		if (flags.HasFlag(TextServer.GraphemeFlag.EmbeddedObject))
		{
			// TODO: implement icons
		}
		else
		{
			var spanIndex = (int)glyph["span_index"];
			var runIndex  = (int)Server.ShapedGetSpanMeta(shaped, spanIndex);

			var run = Text.ParsedSequence[runIndex].Run;
			var runStyle = run.StyleOverride;
			
			Buffer.Glyphs.Add(new Glyph
			{
				Start    = (int)glyph["start"],
				End      = (int)glyph["end"],
				Count    = (byte)glyph["count"],
				Repeat   = (byte)glyph["repeat"],
				Flags    = flags,
				Offset   = (Vector2)glyph["offset"],
				Advance  = (float)glyph["advance"],
				Font     = (Rid)glyph["font_rid"],
				FontSize = (int)glyph["font_size"],
				Index    = (int)glyph["index"],
				Color    = runStyle.Color ?? Text.BaseStyle.Color
			});
		}
	}
}
