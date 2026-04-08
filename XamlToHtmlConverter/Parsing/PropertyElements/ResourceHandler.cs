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

            // x:Key - Find attribute without LINQ allocation
            XAttribute? keyAttr = null;
            foreach (var attr in resource.Attributes())
            {
                if (attr.Name.LocalName == "Key")
                {
                    keyAttr = attr;
                    break;
                }
            }

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
        const string startMarker = "{StaticResource";
        
        if (!value.StartsWith(startMarker))
            return null;

        // Find end marker position
        int endIndex = value.LastIndexOf('}');
        
        // Validate: end marker must be after start marker
        if (endIndex <= startMarker.Length)
            return null;
        
        // Extract substring between markers and trim whitespace
        return value.Substring(startMarker.Length, endIndex - startMarker.Length).Trim();
    }
}