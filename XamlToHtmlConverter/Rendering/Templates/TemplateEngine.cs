using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Templates;

public class TemplateEngine
{
    public void ExpandTemplates(IntermediateRepresentationElement element)
    {
        for (int i = 0; i < element.Children.Count; i++)
        {
            var child = element.Children[i];

            // ControlTemplate
            if (child.Type.EndsWith(".Template"))
            {
                ExpandControlTemplate(element, child);
                element.Children.RemoveAt(i);
                i--;
                continue;
            }

            // ItemTemplate
            if (child.Type == "ItemsControl.ItemTemplate")
            {
                ExpandItemTemplate(element, child);
                element.Children.RemoveAt(i);
                i--;
                continue;
            }

            ExpandTemplates(child);
        }
    }
    private void ExpandControlTemplate(
    IntermediateRepresentationElement parent,
    IntermediateRepresentationElement templateNode)
    {
        foreach (var templateChild in templateNode.Children)
        {
            if (templateChild.Type == "ControlTemplate")
            {
                foreach (var visual in templateChild.Children)
                {
                    visual.Parent = parent;
                    parent.Children.Add(visual);
                }
            }
        }
    }
    private void ExpandItemTemplate(
    IntermediateRepresentationElement parent,
    IntermediateRepresentationElement templateNode)
    {
        foreach (var templateChild in templateNode.Children)
        {
            if (templateChild.Type == "DataTemplate")
            {
                foreach (var visual in templateChild.Children)
                {
                    visual.Parent = parent;
                    parent.Children.Add(visual);
                }
            }
        }
    }
}