using System.Text;

namespace Espejismo.Core.RichText.Parsing;

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

	public Rune Entity { get; init; }

	public int ParentIndex { get; init; } = -1;

	public int FirstChildIndex { get; init; } = -1;

	public int LastChildIndex { get; init; } = -1;

	public int SiblingIndex { get; init; } = -1;

	public int AttributeStart { get; init; } = -1;

	public int AttributeCount { get; init; }
}
