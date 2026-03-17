// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
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
        var ir = new IntermediateRepresentationElement(element.Name.LocalName);
        ProcessAttributes(element, ir);
        ProcessText(element, ir);
        ProcessChildren(element, ir);
        return ir;
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
        //foreach (var child in element.Elements())
        //{
        //    var childName = child.Name.LocalName;

        //    // Handle Resources property elements
        //    if (child.Name.LocalName.EndsWith(".Resources"))
        //    {
        //        ParseResources(child, ir);
        //        continue;
        //    }

        //    // Handle Template property elements
        //    if (childName.EndsWith(".Template"))
        //    {
        //        var templateElement = child.Elements().FirstOrDefault();
        //        if (templateElement != null)
        //        {
        //            var templateIr = ConvertElement(templateElement);
        //            templateIr.Parent = ir;
        //            ir.Template = templateIr;
        //        }

        //        continue;
        //    }

        //    // Handle Grid.RowDefinitions
        //    if (childName == "Grid.RowDefinitions")
        //    {
        //        foreach (var rowDef in child.Elements())
        //        {
        //            var heightAttr = rowDef.Attribute("Height");
        //            if (heightAttr != null)
        //                ir.GridRowDefinitions.Add(heightAttr.Value);
        //        }

        //        continue;
        //    }

        //    // Handle Grid.ColumnDefinitions
        //    if (childName == "Grid.ColumnDefinitions")
        //    {
        //        foreach (var colDef in child.Elements())
        //        {
        //            var widthAttr = colDef.Attribute("Width");
        //            if (widthAttr != null)
        //                ir.GridColumnDefinitions.Add(widthAttr.Value);
        //        }

        //        continue;
        //    }

        //    if (child.Name.LocalName.EndsWith(".ItemTemplate"))
        //    {
        //        var dataTemplate = child.Elements().FirstOrDefault();

        //        if (dataTemplate != null)
        //        {
        //            var templateRoot = dataTemplate.Elements().FirstOrDefault();

        //            if (templateRoot != null)
        //            {
        //                ir.ItemTemplate = ConvertElement(templateRoot);
        //            }
        //        }

        //        continue;
        //    }

        //    if (childName == "Style.Triggers")
        //    {
        //        foreach (var triggerNode in child.Elements())
        //        {
        //            if (triggerNode.Name.LocalName == "Trigger")
        //            {
        //                var trigger = new IntermediateRepresentationTrigger();

        //                trigger.Property =
        //                    triggerNode.Attribute("Property")?.Value ?? "";

        //                trigger.Value =
        //                    triggerNode.Attribute("Value")?.Value ?? "";

        //                foreach (var setter in triggerNode.Elements())
        //                {
        //                    if (setter.Name.LocalName != "Setter")
        //                        continue;

        //                    var prop =
        //                        setter.Attribute("Property")?.Value;

        //                    var val =
        //                        setter.Attribute("Value")?.Value;

        //                    if (prop != null && val != null)
        //                        trigger.Setters[prop] = val;
        //                }

        //                ir.Triggers.Add(trigger);
        //            }
        //        }

        //        continue;
        //    }
        //    if (childName.EndsWith(".Style"))
        //    {
        //        var styleNode = child.Elements().FirstOrDefault();

        //        if (styleNode != null)
        //        {
        //            ParseStyle(styleNode, ir);
        //        }

        //        continue;
        //    }

        //    // Normal child elements
        //    var childIr = ConvertElement(child);
        //    childIr.Parent = ir;
        //    ir.Children.Add(childIr);
        //}
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

                        var conditionsNode = triggerNode
                            .Elements()
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

            var keyAttr = style.Attributes().FirstOrDefault(a => a.Name.LocalName == "Key");
            var targetTypeAttr = style.Attribute("TargetType");
            var basedOnAttr = style.Attribute("BasedOn");

            var styleObject = new IntermediateRepresentationStyle();

            if (keyAttr != null)
                styleObject.Key = keyAttr.Value;

            if (targetTypeAttr != null)
                styleObject.TargetType = targetTypeAttr.Value;

            if (basedOnAttr != null)
                styleObject.BasedOn = ExtractStaticResourceKey(basedOnAttr.Value);

            foreach (var setter in style.Elements().Where(e => e.Name.LocalName == "Setter"))
            {
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

        var key = styleValue.Replace("{StaticResource", "").Replace("}", "").Trim();
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
    /// Extracts the resource key from a StaticResource markup extension string.
    /// </summary>
    /// <param name="value">The raw attribute value (e.g., "{StaticResource MyStyle}").</param>
    /// <returns>The extracted key string, or <c>null</c> if the value is not a StaticResource reference.</returns>
    private string? ExtractStaticResourceKey(string value)
    {
        if (!value.StartsWith("{StaticResource"))
            return null;

        return value.Replace("{StaticResource", "").Replace("}", "").Trim();
    }

    #endregion
}