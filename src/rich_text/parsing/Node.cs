using System.Text;

namespace Spectrum.RichText.Parsing;

// The nodes from the Document struct. Node relationships are described using indexes from the Document.Nodes list.
internal readonly struct Node(NodeType type)
{
	public NodeType Type { get; } = type;

	// Depends on the node type:
	// * For text nodes, returns the start of the text content.
	// * For element nodes, returns the start of the tag name.
	// * For root nodes, simply returns 0.
	public int ValueStart { get; init; }

	public int ValueLength { get; init; }

	public Rune CharacterEntity { get; init; }

	// A value of -1 represents that the node is the root of the hierarchy.
	public int Parent { get; init; } = -1;

	// A value of -1 represents that the node is a leaf.
	public int FirstChild { get; init; } = -1;

	// For nodes with one child, FirstChild and LastChild are equal.
	public int LastChild { get; init; } = -1;

	// A value of -1 represents that the node is the last child in its hierarchy.
	public int Sibling { get; init; } = -1;

	// A value of -1 represents that the node does not have any attributes associated with.
	public int AttributeStart { get; init; } = -1;

	public int AttributeCount { get; init; }
}
