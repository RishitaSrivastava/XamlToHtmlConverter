using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public class ResourceHandler : IPropertyElementHandler
{
    public bool CanHandle(string elementName)
    {
        return elementName.EndsWith(".Resources");
    }

    public void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        foreach (var resource in node.Elements())
        {
            if (resource.Name.LocalName != "Style")
                continue;

            var style = new IntermediateRepresentationStyle();

            // x:Key
            var keyAttr = resource.Attributes()
                .FirstOrDefault(a => a.Name.LocalName == "Key");

            if (keyAttr != null)
                style.Key = keyAttr.Value;

            // TargetType
            var targetTypeAttr = resource.Attribute("TargetType");
            if (targetTypeAttr != null)
                style.TargetType = targetTypeAttr.Value;

            // BasedOn
            var basedOnAttr = resource.Attribute("BasedOn");
            if (basedOnAttr != null)
                style.BasedOn = ExtractStaticResourceKey(basedOnAttr.Value);

            // Parse setters
            foreach (var setter in resource.Elements())
            {
                if (setter.Name.LocalName != "Setter")
                    continue;

                var prop = setter.Attribute("Property")?.Value;
                var val = setter.Attribute("Value")?.Value;

                if (prop != null && val != null)
                    style.Setters[prop] = val;
            }

            // Store resource
            if (style.Key != null)
                ir.Resources[style.Key] = style;
            else if (style.TargetType != null)
                ir.Resources["__implicit__" + style.TargetType] = style;
        }
    }

    private string? ExtractStaticResourceKey(string value)
    {
        if (!value.StartsWith("{StaticResource"))
            return null;

        return value
            .Replace("{StaticResource", "")
            .Replace("}", "")
            .Trim();
    }
}