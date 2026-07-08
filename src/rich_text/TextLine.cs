using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Alignment = Godot.HorizontalAlignment;

namespace Spectrum.RichText;

public readonly struct TextLine
{
	internal TextLine(int start, int length, float ascent, float descent, float width, Alignment alignment)
	{
		Start = start;
		Length = length;

		Ascent = ascent;
		Descent = descent;
		Width = width;
		Alignment = alignment;
	}

	public int Start { get; }

	public int Length { get; }

	public float Ascent { get; }

	public float Descent { get; }

	public float Width { get; }

	public float Height => Ascent + Descent;

	public Alignment Alignment { get; }

	public float GetAlignmentOffset(float containerWidth)
	{
		if (Alignment == Alignment.Center)
		{
			return (containerWidth - Width) / 2f;
		}

		if (Alignment == Alignment.Right)
		{
			return containerWidth - Width;
		}

		return 0f;
	}
}
