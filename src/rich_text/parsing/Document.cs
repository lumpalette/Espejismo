using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Spectrum.RichText.Parsing;

// Immutable, flattened rich-text AST. Node and attributes are stored in plain spans.
internal readonly struct Document
{
	private readonly List<Node> _nodes;
	private readonly List<AttributeSpan> _attributes;

	private Document(string source, List<Node> nodes, List<AttributeSpan> attributes)
	{
		Source = source;
		_nodes = nodes;
		_attributes = attributes;
	}

	public string Source { get; }

	public ReadOnlySpan<Node> Nodes => CollectionsMarshal.AsSpan(_nodes);

	public ReadOnlySpan<AttributeSpan> Attributes => CollectionsMarshal.AsSpan(_attributes);

	public static Document Parse(string source)
	{
		var nodes = new List<Node>();
		var attributes = new List<AttributeSpan>();

		AddNode(new Node(NodeType.Root), nodes);

		var parentIndex = 0;
		var tokenizer = new Tokenizer(source);

		while (tokenizer.Read())
		{
			switch (tokenizer.TokenType)
			{
				case TokenType.Text:
					AddNode(new Node(NodeType.Text)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						ParentIndex = parentIndex
					}, nodes);
					break;
				case TokenType.StartTag:
					var attributeStart = attributes.Count;
					var attributeCount = tokenizer.Attributes.Length;

					attributes.AddRange(tokenizer.Attributes);

					var newParentIndex = AddNode(new Node(NodeType.Element)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						ParentIndex = parentIndex,
						AttributeStart = attributeStart,
						AttributeCount = attributeCount
					}, nodes);

					if (!tokenizer.IsSelfClosing)
					{
						parentIndex = newParentIndex;
					}
					break;
				case TokenType.EndTag:
					var index = parentIndex;

					while (index != 0)
					{
						var current = nodes[index];
						var name = source.AsSpan(current.ValueStart, current.ValueLength);

						if (name.SequenceEqual(tokenizer.ReadValue))
						{
							parentIndex = current.ParentIndex;
							break;
						}

						index = current.ParentIndex;
					}
					break;
				case TokenType.CharacterEntity:
					AddNode(new Node(NodeType.CharacterEntity)
					{
						CharacterEntity = tokenizer.CharacterEntity,
						ParentIndex = parentIndex
					}, nodes);
					break;
			}
		}

		return new Document(source, nodes, attributes);
	}

	private static int AddNode(Node node, List<Node> nodes)
	{
		var newParentIndex = nodes.Count;
		nodes.Add(node);

		// Root node.
		if (node.ParentIndex == -1)
		{
			return newParentIndex;
		}

		var parent = nodes[node.ParentIndex];

		// Add child to node with no children.
		if (parent.FirstChildIndex == -1)
		{
			nodes[node.ParentIndex] = parent with
			{
				FirstChildIndex = newParentIndex,
				LastChildIndex = newParentIndex
			};

			return newParentIndex;
		}

		// Add sibling to last child and update the parent.
		nodes[parent.LastChildIndex] = nodes[parent.LastChildIndex] with
		{
			SiblingIndex = newParentIndex
		};

		nodes[node.ParentIndex] = parent with
		{
			LastChildIndex = newParentIndex
		};

		return newParentIndex;
	}

	public override string ToString()
	{
		if (Nodes.Length == 0)
		{
			return "Document\n└── (empty)";
		}

		var sb = new StringBuilder("Document");
		PrintChildren(0, sb, string.Empty);

		return sb.ToString();
	}

	private void PrintChildren(int parent, StringBuilder sb, string preffix)
	{
		var childIndex = Nodes[parent].FirstChildIndex;

		// mientras el node tenga ñiñes:
		while (childIndex != -1)
		{
			var child = Nodes[childIndex];
			var isLast = child.SiblingIndex == -1;

			sb.Append('\n');
			sb.Append(preffix);
			sb.Append(isLast ? "└── " : "├── ");

			if (child.Type == NodeType.Element)
			{
				sb.Append("Element(");
				sb.Append(Source, child.ValueStart, child.ValueLength);
				sb.Append(")\n");

				sb.Append(preffix);
				sb.Append(isLast ? "    " : "│   ");
				sb.Append("├── Attributes:");

				if (child.AttributeCount > 0)
				{
					for (var i = 0; i < child.AttributeCount; i++)
					{
						var attribute = Attributes[i + child.AttributeStart];

						sb.Append('\n');
						sb.Append(preffix);
						sb.Append(isLast ? "    " : "│   ");
						sb.Append(i + 1 < child.AttributeCount ? "│   ├── " : "│   └── ");

						sb.Append('"');
						sb.Append(Source, attribute.NameStart, attribute.NameLength);

						if (attribute.ValueLength > 0)
						{
							sb.Append("\" = \"");
							sb.Append(Source, attribute.ValueStart, attribute.ValueLength);
						}

						sb.Append('"');
					}

					sb.Append('\n');
				}
				else
				{
					sb.Append('\n');
					sb.Append(preffix);
					sb.Append(isLast ? "    " : "│   ");
					sb.Append("│   └── (empty)\n");
				}

				sb.Append(preffix);
				sb.Append(isLast ? "    " : "│   ");
				sb.Append("└── Children:");

				if (child.FirstChildIndex != -1)
				{
					var newPreffix = preffix + (isLast ? "        " : "│       ");
					PrintChildren(childIndex, sb, newPreffix);
				}
				else
				{
					sb.Append('\n');
					sb.Append(preffix);
					sb.Append(isLast ? "    " : "│   ");
					sb.Append("    └── (empty)");
				}
			}
			else
			{
				sb.Append("Text(\"");

				for (var i = 0; i < child.ValueLength; i++)
				{
					var c = Source[child.ValueStart + i];

					// Line feeds break everything so we must append them as plain text.
					if (c == '\n')
					{
						sb.Append("\\n");
					}
					else
					{
						sb.Append(c);
					}
				}

				sb.Append("\")");
			}

			childIndex = child.SiblingIndex;
		}
	}
}
