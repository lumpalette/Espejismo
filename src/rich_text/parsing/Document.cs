using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Spectrum.RichText.Parsing;

// Immutable, flattened rich-text AST. Node and attributes are stored in plain spans.
internal readonly ref struct Document
{
	private Document(string text, List<Node> nodes, List<AttributeSpan> attributes)
	{
		Text = text;
		Nodes = CollectionsMarshal.AsSpan(nodes);
		Attributes = CollectionsMarshal.AsSpan(attributes);
	}

	public string Text { get; }

	public ReadOnlySpan<Node> Nodes { get; }

	public ReadOnlySpan<AttributeSpan> Attributes { get; }

	public override string ToString()
	{
		if (Nodes.Length == 0)
		{
			return "Document\n└── (empty)";
		}

		var sb = new StringBuilder("Document");
		PrintChildren(0, sb, "");

		return sb.ToString();
	}

	private void PrintChildren(int parent, StringBuilder sb, string preffix)
	{
		int child = Nodes[parent].FirstChild;

		// mientras el node tenga ñiñes:
		while (child != -1)
		{
			var childNode = Nodes[child];
			bool isLast = childNode.Sibling == -1;

			sb.Append('\n');
			sb.Append(preffix);
			sb.Append(isLast ? "└── " : "├── ");
			
			if (childNode.Type == NodeType.Element)
			{
				sb.Append("Element(");
				sb.Append(Text, childNode.ValueStart, childNode.ValueLength);
				sb.Append(")\n");

				sb.Append(preffix);
				sb.Append(isLast ? "    " : "│   ");
				sb.Append("├── Attributes:");

				if (childNode.AttributeCount > 0)
				{
					for (int i = 0; i < childNode.AttributeCount; i++)
					{
						var attribute = Attributes[i + childNode.AttributeStart];

						sb.Append('\n');
						sb.Append(preffix);
						sb.Append(isLast ? "    " : "│   ");
						sb.Append(i + 1 < childNode.AttributeCount ? "│   ├── " : "│   └── ");

						sb.Append('"');
						sb.Append(Text, attribute.NameStart, attribute.NameLength);
						
						if (attribute.ValueLength > 0)
						{
							sb.Append("\" = \"");
							sb.Append(Text, attribute.ValueStart, attribute.ValueLength);
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

				if (childNode.FirstChild != -1)
				{
					string newPreffix = preffix + (isLast ? "        " : "│       ");
					PrintChildren(child, sb, newPreffix);
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
				
				for (int i = 0; i < childNode.ValueLength; i++)
				{
					char c = Text[childNode.ValueStart + i];

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

			child = childNode.Sibling;
		}
	}

	public static Document Parse(string text)
	{
		var nodes = new List<Node>();
		var attributes = new List<AttributeSpan>();

		AddNode(new Node(NodeType.Root), nodes);

		int currentParent = 0;
		var tokenizer = new Tokenizer(text);

		while (tokenizer.Read())
		{
			switch (tokenizer.TokenType)
			{
				case TokenType.Text:
					AddNode(new Node(NodeType.Text)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						Parent = currentParent,
					}, nodes);
					break;
				case TokenType.StartTag:
					var attributeStart = attributes.Count;
					var attributeCount = tokenizer.Attributes.Length;

					attributes.AddRange(tokenizer.Attributes);

					int newParent = AddNode(new Node(NodeType.Element)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						Parent = currentParent,
						AttributeStart = attributeStart,
						AttributeCount = attributeCount
					}, nodes);
					
					if (!tokenizer.IsSelfClosing)
					{
						currentParent = newParent;
					}
					break;
				case TokenType.EndTag:
					int target = currentParent;

					while (target != 0)
					{
						var node = nodes[target];
						var name = text.AsSpan(node.ValueStart, node.ValueLength);

						if (name.SequenceEqual(tokenizer.ReadValue))
						{
							currentParent = node.Parent;
							break;
						}

						target = node.Parent;
					}
					break;
			}
		}

		return new Document(text, nodes, attributes);
	}

	private static int AddNode(Node node, List<Node> nodes)
	{
		int newParent = nodes.Count;
		nodes.Add(node);

		if (node.Parent == -1)
		{
			return newParent;
		}

		var parentNode = nodes[node.Parent];

		if (parentNode.FirstChild == -1)
		{
			nodes[node.Parent] = parentNode with { FirstChild = newParent, LastChild = newParent };
			return newParent;
		}

		nodes[parentNode.LastChild] = nodes[parentNode.LastChild] with { Sibling = newParent };
		nodes[node.Parent] = parentNode with { LastChild = newParent };

		return newParent;
	}
}
