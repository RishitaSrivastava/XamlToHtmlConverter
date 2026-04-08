// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using System.Xml;
using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing.PropertyElements;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Converts XML elements into IR elements using a structured recursive processing approach.
/// Separates attribute, text, and child handling into dedicated methods for clarity.
/// Performs a second pass to resolve StaticResource references and propagate DataContext.
/// </summary>
public class XmlToIrConverterRecursive : IXmlToIrConverter
{

    private readonly PropertyElementHandlerEngine propertyHandlers;

    public XmlToIrConverterRecursive()
    {
        propertyHandlers = new PropertyElementHandlerEngine(new IPropertyElementHandler[]
        {
        new GridDefinitionHandler(),
        new StyleHandler(),
        new ItemTemplateHandler(),
        new ResourceHandler(),
        new TemplateHandler(),
        });
    }
    #region Public Methods

    /// <summary>
    /// Validates input, starts recursive conversion of the XML element tree,
    /// and performs resource resolution and context propagation in a SINGLE pass.
    /// Performance optimization: Eliminates 3 separate tree traversals by combining all
    /// processing into one recursive walk.
    /// </summary>
    /// <param name="element">The root XML element to convert.</param>
    /// <returns>The root <see cref="IntermediateRepresentationElement"/> of the converted IR tree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public IntermediateRepresentationElement Convert(XElement element)
    {
        var root = ConvertElement(element);

        // Single pass combines: resource resolution + context propagation
        // Replaces calling ResolveStaticResources() and DataContextPropagator.Propagate() separately
        ConsolidateTreePass(root, parentDataContext: null);

        return root;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Converts a single <see cref="XElement"/> into an <see cref="IntermediateRepresentationElement"/>
    /// by delegating attribute, text, and child processing to specialized methods.
    /// </summary>
    /// <param name="element">The XML element to convert.</param>
    /// <returns>The resulting <see cref="IntermediateRepresentationElement"/>.</returns>
    private IntermediateRepresentationElement ConvertElement(XElement element)
    {
        try
        {
        var ir = new IntermediateRepresentationElement(element.Name.LocalName);
        ProcessAttributes(element, ir);
        ProcessText(element, ir);
        ProcessChildren(element, ir);
        return ir;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
        // Retrieve line info if the XElement was loaded with line tracking
        var lineInfo = (element as IXmlLineInfo)?.HasLineInfo() == true
        ? $" (line {((IXmlLineInfo)element).LineNumber})"
        : string.Empty;
        throw new InvalidOperationException(
        $"Failed to convert XAML element '<{element.Name.LocalName}>'{lineInfo}.",
        ex);
        }

    }

    /// <summary>
    /// Maps XML attributes to standard or attached IR properties,
    /// skipping namespace declaration attributes.
    /// </summary>
    /// <param name="element">The source XML element.</param>
    /// <param name="ir">The target IR element to populate.</param>
    private void ProcessAttributes(XElement element, IntermediateRepresentationElement ir)
    {
        foreach (var attr in element.Attributes())
        {
            if (attr.IsNamespaceDeclaration)
                continue;

            var name = attr.Name.LocalName;
            var value = attr.Value;

            if (IsAttachedProperty(name))
                ir.AttachedProperties[name] = value;
            else
            {
                var binding = BindingParser.Parse(value);

                if (binding != null)
                {
                    ir.Bindings[name] = binding;

                    if (name == "DataContext")
                    {
                        ir.DataContext = binding.Path;
                    }
                }
                else
                {
                    ir.Properties[name] = value;
                }
            }
        }
    }

    /// <summary>
    /// Extracts and assigns combined non-empty text nodes as the IR element's inner text.
    /// Performance optimization: Direct loop instead of LINQ to eliminate allocations.
    /// Removes: OfType, Select, Where state machines and string.Join array allocation.
    /// </summary>
    /// <param name="element">The source XML element.</param>
    /// <param name="ir">The target IR element to populate.</param>
    private void ProcessText(XElement element, IntermediateRepresentationElement ir)
    {
        var sb = new StringBuilder();
        bool isFirstText = true;

        // Direct loop - zero allocations
        foreach (var node in element.Nodes())
        {
            // Check if node is text (no OfType allocation)
            if (node is XText xtext)
            {
                var trimmed = xtext.Value.Trim();

                // Skip empty text nodes
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Add space between text nodes
                if (!isFirstText)
                    sb.Append(' ');

                sb.Append(trimmed);
                isFirstText = false;
            }
        }

        // Only set InnerText if we found any text
        if (sb.Length > 0)
        {
            ir.InnerText = sb.ToString();
        }
    }

    /// <summary>
    /// Iterates over child XML elements and handles resources, template property elements,
    /// grid row/column definitions, and normal child elements recursively.
    /// </summary>
    /// <param name="element">The source XML element.</param>
    /// <param name="ir">The target IR element to populate.</param>
    private void ProcessChildren(XElement element, IntermediateRepresentationElement ir)
    {
        foreach (var child in element.Elements())
        {
            if (propertyHandlers.TryHandle(child, ir, ConvertElement))
                continue;

            var childIr = ConvertElement(child);
            childIr.Parent = ir;
            ir.Children.Add(childIr);
        }
        
    }

    private void ParseStyle(
    XElement styleNode,
    IntermediateRepresentationElement ir)
    {
        foreach (var child in styleNode.Elements())
        {
            var name = child.Name.LocalName;

            if (name == "Style.Triggers")
            {
                foreach (var triggerNode in child.Elements())
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

                        // Find conditions node without LINQ allocation
                        XElement? conditionsNode = null;
                        foreach (var elem in triggerNode.Elements())
                        {
                            if (elem.Name.LocalName == "MultiTrigger.Conditions")
                            {
                                conditionsNode = elem;
                                break;
                            }
                        }

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

                        // Find conditions node without LINQ allocation
                        XElement? conditionsNode = null;
                        foreach (var elem in triggerNode.Elements())
                        {
                            if (elem.Name.LocalName == "MultiDataTrigger.Conditions")
                            {
                                conditionsNode = elem;
                                break;
                            }
                        }

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
            
        }
    }

    /// <summary>
    /// Extracts the binding path from a XAML binding markup extension string.
    /// For example, "{Binding IsActive}" returns "IsActive".
    /// </summary>
    private static string ExtractBindingPath(string bindingExpression)
    {
        if (string.IsNullOrWhiteSpace(bindingExpression))
            return "";

        var expr = bindingExpression.Trim();
        if (expr.StartsWith('{') && expr.EndsWith('}'))
            expr = expr[1..^1].Trim();

        if (expr.StartsWith("Binding", StringComparison.OrdinalIgnoreCase))
            expr = expr[7..].Trim();

        if (string.IsNullOrWhiteSpace(expr))
            return "";

        var parts = expr.Split(',');
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                return trimmed[5..].Trim();

            if (!trimmed.Contains('='))
                return trimmed;
        }

        return expr;
    }

    /// <summary>
    /// Determines whether an attribute name represents an attached property
    /// based on the dotted naming convention (e.g., Grid.Row).
    /// </summary>
    /// <param name="name">The attribute name to evaluate.</param>
    /// <returns><c>true</c> if the name contains a dot; otherwise, <c>false</c>.</returns>
    private bool IsAttachedProperty(string name)
    {
        return name.Contains(".");
    }

    /// <summary>
    /// Parses a Resources property element and extracts all Style definitions,
    /// storing them in the owner element's resource dictionary by key or implicit type key.
    /// </summary>
    /// <param name="resourcesNode">The XML resources element.</param>
    /// <param name="ir">The IR element that will own the parsed style resources.</param>
    private void ParseResources(XElement resourcesNode, IntermediateRepresentationElement ir)
    {
        foreach (var style in resourcesNode.Elements())
        {
            if (style.Name.LocalName != "Style")
                continue;

            // Find Key attribute without LINQ allocation
            XAttribute? keyAttr = null;
            foreach (var attr in style.Attributes())
            {
                if (attr.Name.LocalName == "Key")
                {
                    keyAttr = attr;
                    break;
                }
            }
            
            var targetTypeAttr = style.Attribute("TargetType");
            var basedOnAttr = style.Attribute("BasedOn");

            var styleObject = new IntermediateRepresentationStyle();

            if (keyAttr != null)
                styleObject.Key = keyAttr.Value;

            if (targetTypeAttr != null)
                styleObject.TargetType = targetTypeAttr.Value;

            if (basedOnAttr != null)
                styleObject.BasedOn = ExtractStaticResourceKey(basedOnAttr.Value);

            // Find and process setters without LINQ allocation
            foreach (var setter in style.Elements())
            {
                if (setter.Name.LocalName != "Setter")
                    continue;

                var propAttr = setter.Attribute("Property");
                var valueAttr = setter.Attribute("Value");

                if (propAttr != null && valueAttr != null)
                    styleObject.Setters[propAttr.Value] = valueAttr.Value;
            }

            if (styleObject.Key != null)
                ir.Resources[styleObject.Key] = styleObject;
            else if (styleObject.TargetType != null)
                ir.Resources["__implicit__" + styleObject.TargetType] = styleObject;
        }
    }

    /// <summary>
    /// Applies an explicit StaticResource style reference to the specified element
    /// by resolving the style key from the ancestor resource chain.
    /// </summary>
    /// <param name="element">The IR element that may reference a StaticResource style.</param>
    private void ApplyStaticResource(IntermediateRepresentationElement element)
    {
        if (!element.Properties.TryGetValue("Style", out var styleValue))
            return;

        if (!styleValue.StartsWith("{StaticResource"))
            return;

        var key = ExtractStaticResourceKey(styleValue);
        if (key == null)
            return;
            
        var style = FindResource(element.Parent, key);
        if (style == null)
            return;

        ApplyStyleWithInheritance(element, style);
    }

    /// <summary>
    /// Applies an implicit style (keyed by element type) to the specified element
    /// if one is found in the ancestor resource chain, without overriding explicit properties.
    /// </summary>
    /// <param name="element">The IR element to apply the implicit style to.</param>
    private void ApplyImplicitStyle(IntermediateRepresentationElement element)
    {
        var implicitKey = "__implicit__" + element.Type;
        var style = FindResource(element.Parent, implicitKey);
        if (style == null)
            return;

        foreach (var setter in style.Setters)
            if (!element.Properties.ContainsKey(setter.Key))
                element.Properties[setter.Key] = setter.Value;
    }

    /// <summary>
    /// Recursively applies a style's setters to the element, first applying the base style
    /// if one is specified, then overlaying this style's own setters without overriding
    /// explicitly set properties.
    /// </summary>
    /// <param name="element">The IR element to apply the style to.</param>
    /// <param name="style">The style whose setters are to be applied.</param>
    private void ApplyStyleWithInheritance(IntermediateRepresentationElement element,
        IntermediateRepresentationStyle style)
    {
        // Apply parent style first
        if (!string.IsNullOrWhiteSpace(style.BasedOn))
        {
            var parentStyle = FindResource(element.Parent, style.BasedOn);
            if (parentStyle != null)
                ApplyStyleWithInheritance(element, parentStyle);
        }

        // Apply this style's setters
        foreach (var setter in style.Setters)
            if (!element.Properties.ContainsKey(setter.Key))
                element.Properties[setter.Key] = setter.Value;
    }

    /// <summary>
    /// Walks up the parent chain to find a resource with the specified key.
    /// </summary>
    /// <param name="element">The starting element for the upward resource search.</param>
    /// <param name="key">The resource key to locate.</param>
    /// <returns>The matching <see cref="IntermediateRepresentationStyle"/> if found; otherwise, null.</returns>
    private IntermediateRepresentationStyle? FindResource(IntermediateRepresentationElement? element, string key)
    {
        while (element != null)
        {
            if (element.Resources.TryGetValue(key, out var style))
                return style;

            element = element.Parent;
        }

        return null;
    }

    /// <summary>
    /// Performs resource resolution and context propagation in a SINGLE tree walk.
    /// Performance optimization (Phase 2, Problem #5):
    /// Combines:
    ///  - ApplyImplicitStyle()
    ///  - ApplyStaticResource()
    ///  - DataContext propagation
    /// Into one recursive pass, eliminating 2 separate full tree traversals.
    /// </summary>
    /// <param name="element">The IR element to process.</param>
    /// <param name="parentDataContext">DataContext from parent to propagate if child doesn't have one.</param>
    private void ConsolidateTreePass(IntermediateRepresentationElement element, string? parentDataContext)
    {
        // Apply implicit style (matches element type)
        ApplyImplicitStyle(element);

        // Apply explicit StaticResource style reference
        ApplyStaticResource(element);

        // Propagate DataContext from parent if not explicitly set
        if (element.DataContext == null && parentDataContext != null)
        {
            element.DataContext = parentDataContext;
        }

        // Determine context for children
        string? contextForChildren = element.DataContext ?? parentDataContext;

        // Single recursive call processes all children in one pass
        foreach (var child in element.Children)
        {
            ConsolidateTreePass(child, contextForChildren);
        }
    }

    /// <summary>
    /// Extracts the resource key from a StaticResource markup extension string using index-based extraction.
    /// Optimized to avoid string allocations from multiple Replace() calls.
    /// </summary>
    /// <param name="value">The raw attribute value (e.g., "{StaticResource MyStyle}").</param>
    /// <returns>The extracted key string, or <c>null</c> if the value is not a StaticResource reference.</returns>
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

    /// <summary>
    /// Applies style setters to an element without inheritance handling.
    /// </summary>
    /// <param name="element">The IR element to apply setters to.</param>
    /// <param name="style">The style containing the setters.</param>
    private void ApplyStyleSetters(
        IntermediateRepresentationElement element,
        IntermediateRepresentationStyle style)
    {
        if (style?.Setters == null)
            return;

        foreach (var setter in style.Setters)
        {
            if (!element.Properties.ContainsKey(setter.Key))
                element.Properties[setter.Key] = setter.Value;
        }
    }

    private void ResolveStaticResource(
        IntermediateRepresentationElement element,
        string propertyName,
        string resourceKey)
        {
        // Walk up the tree to find the resource
        var owner = element;
        while (owner != null)
        {
        if (owner.Resources.TryGetValue(resourceKey, out var style))
        {
        ApplyStyleSetters(element, style);
        return;
        }
        owner = owner.Parent;
        }
        // Resource not found — do NOT silently ignore it
        // Option A: log a warning (preferred for a converter tool)
        Console.Error.WriteLine(
            $"[Warning] StaticResource '{resourceKey}' referenced by '{element.Type}.{propertyName}' could not be resolved.");
        }

    #endregion
}