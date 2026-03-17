using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public class StyleHandler : IPropertyElementHandler
{
    public bool CanHandle(string elementName)
    {
        return elementName.EndsWith(".Style");
    }

    public void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        var styleNode = node.Elements().FirstOrDefault();

        if (styleNode == null)
            return;

        foreach (var child in styleNode.Elements())
        {
            var name = child.Name.LocalName;

            if (name == "Style.Triggers")
            {
                ParseTriggers(child, ir);
            }
        }
    }

    private void ParseTriggers(
        XElement triggersNode,
        IntermediateRepresentationElement ir)
    {
        foreach (var triggerNode in triggersNode.Elements())
        {
            var nodeName = triggerNode.Name.LocalName;

            // NORMAL TRIGGER
            if (nodeName == "Trigger")
            {
                var trigger = new IntermediateRepresentationTrigger();

                trigger.Property =
                    triggerNode.Attribute("Property")?.Value ?? "";

                trigger.Value =
                    triggerNode.Attribute("Value")?.Value ?? "";

                foreach (var setter in triggerNode.Elements())
                {
                    if (setter.Name.LocalName != "Setter")
                        continue;

                    var prop = setter.Attribute("Property")?.Value;
                    var val = setter.Attribute("Value")?.Value;

                    if (prop != null && val != null)
                        trigger.Setters[prop] = val;
                }

                ir.Triggers.Add(trigger);
            }

            // MULTI TRIGGER
            else if (nodeName == "MultiTrigger")
            {
                var multi = new IntermediateRepresentationMultiTrigger();

                var conditionsNode =
                    triggerNode.Elements()
                    .FirstOrDefault(x => x.Name.LocalName == "MultiTrigger.Conditions");

                if (conditionsNode != null)
                {
                    foreach (var cond in conditionsNode.Elements())
                    {
                        var prop = cond.Attribute("Property")?.Value;
                        var val = cond.Attribute("Value")?.Value;

                        if (prop != null && val != null)
                            multi.Conditions.Add((prop, val));
                    }
                }

                foreach (var setter in triggerNode.Elements())
                {
                    if (setter.Name.LocalName != "Setter")
                        continue;

                    var prop = setter.Attribute("Property")?.Value;
                    var val = setter.Attribute("Value")?.Value;

                    if (prop != null && val != null)
                        multi.Setters[prop] = val;
                }

                ir.MultiTriggers.Add(multi);
            }
        }
    }
}