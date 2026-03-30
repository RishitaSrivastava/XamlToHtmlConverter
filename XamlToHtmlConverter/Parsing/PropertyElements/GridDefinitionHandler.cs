using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements
{
    public class GridDefinitionHandler : IPropertyElementHandler
    {
        public bool CanHandle(string name)
        {
            return name == "Grid.RowDefinitions"
                || name == "Grid.ColumnDefinitions";
        }

        public void Handle(
            XElement node,
            IntermediateRepresentationElement ir,
            Func<XElement, IntermediateRepresentationElement> convert)
        {
            if (node.Name.LocalName == "Grid.RowDefinitions")
            {
                foreach (var row in node.Elements())
                {
                    var h = row.Attribute("Height")?.Value;
                    if (h != null)
                        ir.GridRowDefinitions.Add(h);
                }
            }

            if (node.Name.LocalName == "Grid.ColumnDefinitions")
            {
                foreach (var col in node.Elements())
                {
                    var w = col.Attribute("Width")?.Value;
                    if (w != null)
                        ir.GridColumnDefinitions.Add(w);
                }
            }
        }
    }
}
