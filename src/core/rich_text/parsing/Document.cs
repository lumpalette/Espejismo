using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Espejismo.Core.RichText.Parsing;

// Immutable, flattened rich-text AST. Node and attributes are stored in plain spans.
internal readonly struct Document
{
	private readonly List<Node> _nodes = [];
	private readonly List<AttributeSpan> _attributes = [];

	public Document(string source)
	{
		Source = source;
		Write();
	}

	public string Source { get; }

	public ReadOnlySpan<Node> Nodes => (_nodes is not null) ? CollectionsMarshal.AsSpan(_nodes) : [];

	public ReadOnlySpan<AttributeSpan> Attributes
		=> (_attributes is not null) ? CollectionsMarshal.AsSpan(_attributes) : [];

	private void Write()
	{
		AddNode(new Node(NodeType.Root));

		var parentIndex = 0;
		var tokenizer = new Tokenizer(Source);

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
					});
					break;
				case TokenType.StartTag:
					var attributeStart = _attributes.Count;
					var attributeCount = tokenizer.Attributes.Length;

					_attributes.AddRange(tokenizer.Attributes);

					var newParentIndex = AddNode(new Node(NodeType.Element)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						ParentIndex = parentIndex,
						AttributeStart = attributeStart,
						AttributeCount = attributeCount
					});

					if (!tokenizer.IsSelfClosing)
					{
						parentIndex = newParentIndex;
					}
					break;
				case TokenType.EndTag:
					var index = parentIndex;

					while (index != 0)
					{
						var current = _nodes[index];
						var name = Source.AsSpan(current.ValueStart, current.ValueLength);

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
						Entity = tokenizer.CharacterEntity,
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						ParentIndex = parentIndex
					});
					break;
			}
		}
	}

	private int AddNode(Node node)
	{
		var newNodeIndex = _nodes.Count;
		_nodes.Add(node);

		// Root node.
		if (node.ParentIndex == -1)
		{
			return newNodeIndex;
		}

		var parent = _nodes[node.ParentIndex];

		// Add child to node with no children.
		if (parent.ChildIndex == -1)
		{
			_nodes[node.ParentIndex] = parent with { ChildIndex = newNodeIndex };
			return newNodeIndex;
		}

		// Add sibling to the last child of the parent.
		var lastChildIndex = parent.ChildIndex;

		while (lastChildIndex != -1)
		{
			var siblingIndex = _nodes[lastChildIndex].SiblingIndex;

			if (siblingIndex == -1)
			{
				break;
			}

			lastChildIndex = siblingIndex;
		}

		_nodes[lastChildIndex] = _nodes[lastChildIndex] with { SiblingIndex = newNodeIndex };

		return newNodeIndex;
	}

	public override string ToString()
	{
		// Nothing but the root itself.
		if (Nodes.Length <= 1)
		{
			return "Document\n└── (empty)";
		}

		var sb = new StringBuilder("Document");
		PrintChildren(0, sb, string.Empty);

		return sb.ToString();
	}

	private void PrintChildren(int parent, StringBuilder sb, string preffix)
	{
		var childIndex = _nodes[parent].ChildIndex;

		// mientras el node tenga ñiñes:
		while (childIndex != -1)
		{
			var child = _nodes[childIndex];
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

				if (child.ChildIndex != -1)
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
			else if (child.Type == NodeType.CharacterEntity)
			{
				sb.Append("Entity(\"");
				sb.Append(Source, child.ValueStart, child.ValueLength);
				sb.Append("\" = '");
				sb.Append(child.Entity);
				sb.Append("')");
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
