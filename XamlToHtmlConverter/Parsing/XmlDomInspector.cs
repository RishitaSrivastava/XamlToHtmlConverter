// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Provides helper methods for inspecting and printing XML DOM structure.
/// Primarily used for debugging and structure visualization.
/// </summary>
public static class XmlDomInspector
{
    #region Public Methods

    /// <summary>
    /// Recursively prints an XML element, its non-namespace attributes,
    /// and child nodes to the console with indentation representing hierarchy depth.
    /// </summary>
    /// <param name="element">The XML element to inspect and print.</param>
    /// <param name="indent">The number of leading spaces used for the current level of indentation.</param>
    public static void Print(XElement element, int indent = 0)
    {
        var indentation = new string(' ', indent);

        foreach (var attr in element.Attributes())
        {
            if (attr.IsNamespaceDeclaration)
                continue;

            Console.WriteLine($"{indentation}  @{attr.Name.LocalName}={attr.Value}");
        }

        foreach (var node in element.Nodes())
            if (node is XElement childElement)
            {
                Print(childElement, indent + 2);
            }
            else if (node is XText textNode)
            {
                var text = textNode.Value.Trim();
                if (!string.IsNullOrEmpty(text)) Console.WriteLine($"{indentation}  text: {text}");
            }
    }

    #endregion
}