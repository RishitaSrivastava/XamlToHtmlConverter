// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Converts XML elements into IR elements using a structured recursive processing approach.
/// Separates attribute, text, and child handling into dedicated methods for clarity.
/// Performs a second pass to resolve StaticResource references and propagate DataContext.
/// </summary>
public class XmlToIrConverterRecursive : IXmlToIrConverter
{
    #region Public Methods

    /// <summary>
    /// Validates input, starts recursive conversion of the XML element tree,
    /// and resolves StaticResource references in a second pass.
    /// </summary>
    /// <param name="element">The root XML element to convert.</param>
    /// <returns>The root <see cref="IntermediateRepresentationElement"/> of the converted IR tree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public IntermediateRepresentationElement Convert(XElement element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        var root = ConvertElement(element);

        // Second pass: resolve StaticResource references and propagate DataContext
        ResolveStaticResources(root);

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
                ir.Properties[name] = value;
        }
    }

    /// <summary>
    /// Extracts and assigns combined non-empty text nodes
    /// as the IR element's inner text.
    /// </summary>
    /// <param name="element">The source XML element.</param>
    /// <param name="ir">The target IR element to populate.</param>
    private void ProcessText(XElement element, IntermediateRepresentationElement ir)
    {
        var text = string.Join(" ", element.Nodes().OfType<XText>()
            .Select(t => t.Value.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t)));

        if (!string.IsNullOrWhiteSpace(text))
            ir.InnerText = text;
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
            var childName = child.Name.LocalName;

            // Handle Resources property elements
            if (child.Name.LocalName.EndsWith(".Resources"))
            {
                ParseResources(child, ir);
                continue;
            }

            // Handle Template property elements
            if (childName.EndsWith(".Template"))
            {
                var templateElement = child.Elements().FirstOrDefault();
                if (templateElement != null)
                {
                    var templateIr = ConvertElement(templateElement);
                    templateIr.Parent = ir;
                    ir.Template = templateIr;
                }

                continue;
            }

            // Handle Grid.RowDefinitions
            if (childName == "Grid.RowDefinitions")
            {
                foreach (var rowDef in child.Elements())
                {
                    var heightAttr = rowDef.Attribute("Height");
                    if (heightAttr != null)
                        ir.GridRowDefinitions.Add(heightAttr.Value);
                }

                continue;
            }

            // Handle Grid.ColumnDefinitions
            if (childName == "Grid.ColumnDefinitions")
            {
                foreach (var colDef in child.Elements())
                {
                    var widthAttr = colDef.Attribute("Width");
                    if (widthAttr != null)
                        ir.GridColumnDefinitions.Add(widthAttr.Value);
                }

                continue;
            }

            // Normal child elements
            var childIr = ConvertElement(child);
            childIr.Parent = ir;
            ir.Children.Add(childIr);
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
    /// <returns>The matching <see cref="IntermediateRepresentationStyle"/> if found; otherwise, <c>null</c>.</returns>
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
    /// Performs the second-pass resolution: applies implicit and explicit styles,
    /// propagates DataContext to children, and recurses into the subtree.
    /// </summary>
    /// <param name="element">The IR element to resolve resources for.</param>
    private void ResolveStaticResources(IntermediateRepresentationElement element)
    {
        ApplyImplicitStyle(element);
        ApplyStaticResource(element);

        foreach (var child in element.Children)
        {
            PropagateDataContext(element, child);
            ResolveStaticResources(child);
        }
    }

    /// <summary>
    /// Propagates the parent's DataContext to the child element
    /// if the child does not already have its own DataContext defined.
    /// </summary>
    /// <param name="parent">The parent IR element that may carry a DataContext.</param>
    /// <param name="child">The child IR element to inherit the DataContext.</param>
    private void PropagateDataContext(IntermediateRepresentationElement parent, IntermediateRepresentationElement child)
    {
        if (child.Properties.ContainsKey("DataContext"))
            return;

        if (parent.Properties.TryGetValue("DataContext", out var dataContext))
            child.Properties["DataContext"] = dataContext;
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