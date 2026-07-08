namespace Spectrum.RichText.Shaping;

internal readonly struct ParsedItem
{
	public ParsedItem(TextRun run)
	{
		IsRun = true;
		Index = run.SequenceIndex;
		Run = run;
	}

	public ParsedItem(InlineIcon icon)
	{
		IsRun = false;
		Index = icon.SequenceIndex;
		Icon = icon;
	}

	public bool IsRun { get; }

	public int Index { get; }

	public TextRun Run { get; }

	public InlineIcon Icon { get; }
}
