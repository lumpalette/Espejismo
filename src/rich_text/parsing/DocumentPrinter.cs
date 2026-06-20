using System.Text;

namespace Spectrum.RichText.Parsing;

internal static class DocumentPrinter
{
	public static string Print(Document document)
	{
		if (document.Nodes.Length == 0)
		{
			return "Document\n└── (empty)";
		}

		var sb = new StringBuilder("Document");
		AppendChildren(0, document, sb, "");

		return sb.ToString();
	}

	private static void AppendChildren(int parent, Document document, StringBuilder sb, string preffix)
	{
		int child = document.Nodes[parent].FirstChild;

		// mientras el node tenga ñiñes:
		while (child != -1)
		{
			var childNode = document.Nodes[child];
			bool isLast = childNode.Sibling == -1;

			sb.Append('\n');
			sb.Append(preffix);
			sb.Append(isLast ? "└── " : "├── ");
			
			if (childNode.Type == NodeType.Element)
			{
				sb.Append("Element(");
				sb.Append(document.Text, childNode.ValueStart, childNode.ValueLength);
				sb.Append(")\n");

				sb.Append(preffix);
				sb.Append(isLast ? "    " : "│   ");
				sb.Append("├── Attributes:");

				if (childNode.AttributeCount > 0)
				{
					for (int i = 0; i < childNode.AttributeCount; i++)
					{
						var attribute = document.Attributes[i + childNode.AttributeStart];

						sb.Append('\n');
						sb.Append(preffix);
						sb.Append(isLast ? "    " : "│   ");
						sb.Append(i + 1 < childNode.AttributeCount ? "│   ├── " : "│   └── ");

						sb.Append('"');
						sb.Append(document.Text, attribute.NameStart, attribute.NameLength);
						
						if (attribute.ValueLength > 0)
						{
							sb.Append("\" = \"");
							sb.Append(document.Text, attribute.ValueStart, attribute.ValueLength);
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
					AppendChildren(child, document, sb, preffix + (isLast ? "        " : "│       "));
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
					char c = document.Text[childNode.ValueStart + i];

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
}
