using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Spectrum.RichText.Parsing;

// Immutable, flattened rich-text AST. Node and attributes are stored in 1D arrays.
internal sealed class Document
{
	private readonly List<Node> _nodes = [];
	private readonly List<AttributeSpan> _attributes = [];

	public string Text { get; private set; } = string.Empty;

	// The document root is always the first element in the array.
	public ReadOnlySpan<Node> Nodes => CollectionsMarshal.AsSpan(_nodes);

	public ReadOnlySpan<AttributeSpan> Attributes => CollectionsMarshal.AsSpan(_attributes);

	public void Parse(string text)
	{
		Text = text;
		_nodes.Clear();
		_attributes.Clear();

		AddNode(new Node(NodeType.Root));

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
					});
					break;
				case TokenType.StartTag:
					int attrStart = _attributes.Count;
					int attrCount = tokenizer.Attributes.Length;

					_attributes.AddRange(tokenizer.Attributes);

					var newParent = AddNode(new Node(NodeType.Element)
					{
						ValueStart = tokenizer.StartIndex,
						ValueLength = tokenizer.ReadValue.Length,
						Parent = currentParent,
						AttributeStart = attrStart,
						AttributeCount = attrCount
					});
					
					if (!tokenizer.IsSelfClosing)
					{
						currentParent = newParent;
					}
					break;
				case TokenType.EndTag:
					int target = currentParent;

					while (target != 0)
					{
						var node = _nodes[target];
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
	}

	public override string ToString()
	{
		return DocumentPrinter.Print(this);
	}

	private int AddNode(Node node)
	{
		int newParent = _nodes.Count;
		_nodes.Add(node);

		if (node.Parent == -1)
		{
			return newParent;
		}

		var parentNode = _nodes[node.Parent];

		if (parentNode.FirstChild == -1)
		{
			_nodes[node.Parent] = parentNode with { FirstChild = newParent, LastChild = newParent };
			return newParent;
		}

		_nodes[parentNode.LastChild] = _nodes[parentNode.LastChild] with { Sibling = newParent };
		_nodes[node.Parent] = parentNode with { LastChild = newParent };

		return newParent;
	}
}
