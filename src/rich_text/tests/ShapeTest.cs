using Godot;
using Spectrum.RichText.BuiltinTags;

namespace Spectrum.RichText.Tests;

[Tool]
internal sealed partial class ShapeTest : Control
{
	private readonly TextParser _parser;

	private Text? _text;

	public ShapeTest()
	{
		_parser = new TextParser();

		_parser.RegisterTag(new ColorTag());
		_parser.RegisterTag(new AlignmentTag(HorizontalAlignment.Left));
		_parser.RegisterTag(new AlignmentTag(HorizontalAlignment.Right));
		_parser.RegisterTag(new AlignmentTag(HorizontalAlignment.Center));
	}

	[Export(PropertyHint.MultilineText)]
	public string Text
	{
		get;
		set
		{
			value ??= string.Empty;

			if (field != value)
			{
				field = value;
				_text = null;
				QueueRedraw();
			}
		}
	} = string.Empty;

	[Export]
	public TextStyle? Style
	{
		get;
		set
		{
			if (field != value)
			{
				field?.Disconnect(Resource.SignalName.Changed, new Callable(this, CanvasItem.MethodName.QueueRedraw));

				field = value;
				
				field?.Connect(Resource.SignalName.Changed, new Callable(this, CanvasItem.MethodName.QueueRedraw));

				QueueRedraw();
			}
		}
	}

	[Export]
	public HorizontalAlignment HorizontalAlignment
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				QueueRedraw();
			}
		}
	}

	[Export]
	public VerticalAlignment VerticalAlignment
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				QueueRedraw();
			}
		}
	}

	public override void _Draw()
	{
		if (Style is null || Style.Font is null)
		{
			return;
		}

		_text ??= _parser.Parse(Text, Style);

		_text.Size = Size;
		_text.HorizontalAlignment = HorizontalAlignment;
		_text.VerticalAlignment = VerticalAlignment;

		_text.Draw(GetCanvasItem());
	}

	public override void _Notification(int what)
	{
		if (what == NotificationResized)
		{
			QueueRedraw();
		}
	}
}
