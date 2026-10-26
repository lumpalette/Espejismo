using Godot;
using System.Runtime.InteropServices;

namespace Espejismo.Core.RichText;

/// <summary>
/// Represents the visual shape of a text character or icon.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct Glyph
{
    private readonly int _itemIndex;

    public int Start { get; private init; }

    public byte Count { get; private init; }

    public byte Repeat { get; private init; }

    public ushort Flags { get; private init; }

    public Vector2 Bearing { get; private init; }

    public float Advance { get; private init; }

    public Rid FontRid { get; private init; }

    public ushort FontSize { get; private init; }

    public int Index { get; private init; }
}
