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

            // DATA TRIGGER
            else if (nodeName == "DataTrigger")
            {
                var dataTrigger = new IntermediateRepresentationDataTrigger();

                dataTrigger.BindingPath = ExtractBindingPath(
                    triggerNode.Attribute("Binding")?.Value ?? "");

                dataTrigger.Value =
                    triggerNode.Attribute("Value")?.Value ?? "";

                foreach (var setter in triggerNode.Elements())
                {
                    if (setter.Name.LocalName != "Setter")
                        continue;

                    var prop = setter.Attribute("Property")?.Value;
                    var val = setter.Attribute("Value")?.Value;

                    if (prop != null && val != null)
                        dataTrigger.Setters[prop] = val;
                }

                ir.DataTriggers.Add(dataTrigger);
            }

            // MULTI DATA TRIGGER
            else if (nodeName == "MultiDataTrigger")
            {
                var multiData = new IntermediateRepresentationMultiDataTrigger();

                var conditionsNode =
                    triggerNode.Elements()
                    .FirstOrDefault(x => x.Name.LocalName == "MultiDataTrigger.Conditions");

                if (conditionsNode != null)
                {
                    foreach (var cond in conditionsNode.Elements())
                    {
                        var bindingPath = ExtractBindingPath(
                            cond.Attribute("Binding")?.Value ?? "");
                        var val = cond.Attribute("Value")?.Value;

                        if (!string.IsNullOrWhiteSpace(bindingPath) && val != null)
                            multiData.Conditions.Add((bindingPath, val));
                    }
                }

                foreach (var setter in triggerNode.Elements())
                {
                    if (setter.Name.LocalName != "Setter")
                        continue;

                    var prop = setter.Attribute("Property")?.Value;
                    var val = setter.Attribute("Value")?.Value;

                    if (prop != null && val != null)
                        multiData.Setters[prop] = val;
                }

                ir.MultiDataTriggers.Add(multiData);
            }

            // EVENT TRIGGER
            else if (nodeName == "EventTrigger")
            {
                var eventTrigger = new IntermediateRepresentationEventTrigger();

                eventTrigger.RoutedEvent =
                    triggerNode.Attribute("RoutedEvent")?.Value ?? "";

                foreach (var actionNode in triggerNode.Elements())
                {
                    var action = new IntermediateRepresentationTriggerAction
                    {
                        ActionType = actionNode.Name.LocalName
                    };

                    foreach (var attr in actionNode.Attributes())
                        action.Properties[attr.Name.LocalName] = attr.Value;

                    eventTrigger.Actions.Add(action);
                }

                ir.EventTriggers.Add(eventTrigger);
            }
        }
    }

    /// <summary>
    /// Extracts the binding path from a XAML binding markup extension string.
    /// For example, "{Binding IsActive}" returns "IsActive".
    /// Handles simple paths, Path= syntax, and plain property names.
    /// </summary>
    private static string ExtractBindingPath(string bindingExpression)
    {
        if (string.IsNullOrWhiteSpace(bindingExpression))
            return "";

        // Strip braces: {Binding ...} or {x:Binding ...}
        var expr = bindingExpression.Trim();
        if (expr.StartsWith('{') && expr.EndsWith('}'))
            expr = expr[1..^1].Trim();

        // Remove leading "Binding" keyword.
        if (expr.StartsWith("Binding", StringComparison.OrdinalIgnoreCase))
            expr = expr[7..].Trim();

        if (string.IsNullOrWhiteSpace(expr))
            return "";

        // Handle "Path=SomePath, Converter=..." — extract the Path value.
        var parts = expr.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                return trimmed[5..].Trim();

            // If no key=value, the first unnamed token is the path.
            if (!trimmed.Contains('='))
                return trimmed;
        }

        return expr;
    }
}