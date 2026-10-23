namespace Espejismo.Core.RichText;

public sealed partial class Text
{
    /*
    private struct Shaper()
    {
        // Text data,
        public required ShapeItem[] ShapeItems { get; init; }
        public required TextServer Server { get; init; }
        public required Dictionary<TextStyle, ComputedStyle> StyleMap { get; init; }

        // Layout options.
        public required float MaxWidth { get; init; }
        public required HorizontalAlignment BaseAlignment { get; init; }

        // Fallback values.
        public required Font FallbackFont { get; init; }
        public required ushort FallbackFontSize { get; init; }
        public required int FallbackLeading { get; init; }

        // Output.
        public required List<Glyph> Glyphs { get; init; }
        public required List<LineLayout> Lines { get; init; }
        public required List<TextMarker> Markers { get; init; }

        public readonly void Run()
        {
            // 1. Clear the output buffers.
            Glyphs.Clear();
            Lines.Clear();
            Markers.Clear();

            // 2. Split the text into paragraphs.
            if (ShapeItems.Length == 0)
            {
                return;
            }

            var paragraphs = SplitIntoParagraphs();

            // 3. Generate the glyphs and lines from the paragraphs.
        }

        private readonly List<Paragraph> SplitIntoParagraphs()
        {
            var paragraphs = new List<Paragraph>();
            var current = new Paragraph { Alignment = BaseAlignment };
            var independent = true;

            foreach (var item in ShapeItems)
            {
                switch (item)
                {
                    case ShapeItem.Run run:
                        break;
                    case ShapeItem.Icon icon:
                        break;
                    case ShapeItem.Marker marker:
                        break;
                    case ShapeItem.LineBreak br:
                        break;
                    case ShapeItem.Align align:
                        break;
                }
            }
        }

        private struct Paragraph
        {
            public int Start;

            public int Length;

            public HorizontalAlignment Alignment;
        }
    }
    */
}
