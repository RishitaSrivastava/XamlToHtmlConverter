// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides functionality to export an IR element tree
    /// back into an XML document structure for inspection or persistence.
    /// </summary>
    public static class IntermediateRepresentationXmlExporter
    {
        #region Public Methods

        /// <summary>
        /// Converts the root IR element and its subtree into an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="root">The root IR element to export.</param>
        /// <returns>An <see cref="XDocument"/> representing the full IR tree.</returns>
        public static XDocument Export(IntermediateRepresentationElement root)
        {
            var rootElement = ConvertToXElement(root);
            return new XDocument(rootElement);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively converts an IR element into an <see cref="XElement"/>,
        /// preserving the complete tree structure including properties, attached properties,
        /// inner text, templates, resources, bindings, triggers, and child elements.
        /// </summary>
        /// <param name="element">The IR element to convert.</param>
        /// <returns>The resulting <see cref="XElement"/>.</returns>
        private static XElement ConvertToXElement(IntermediateRepresentationElement element)
        {
            var xElement = new XElement(element.Type);

            // Add regular properties as attributes
            foreach (var prop in element.Properties)
                xElement.Add(new XAttribute(prop.Key, prop.Value));

            // Add attached properties as attributes
            foreach (var attached in element.AttachedProperties)
                xElement.Add(new XAttribute(attached.Key, attached.Value));

            // Add inner text content
            if (!string.IsNullOrWhiteSpace(element.InnerText))
                xElement.Add(new XText(element.InnerText));

            // Add DataContext if present
            if (!string.IsNullOrWhiteSpace(element.DataContext))
                xElement.Add(new XAttribute("DataContext", element.DataContext));

            // Add Grid row definitions if present
            if (element.GridRowDefinitions.Count > 0)
            {
                var rowDefsElement = new XElement("GridRowDefinitions");
                foreach (var rowDef in element.GridRowDefinitions)
                {
                    rowDefsElement.Add(new XElement("RowDefinition", 
                        new XAttribute("Height", rowDef)));
                }
                xElement.Add(rowDefsElement);
            }

            // Add Grid column definitions if present
            if (element.GridColumnDefinitions.Count > 0)
            {
                var colDefsElement = new XElement("GridColumnDefinitions");
                foreach (var colDef in element.GridColumnDefinitions)
                {
                    colDefsElement.Add(new XElement("ColumnDefinition", 
                        new XAttribute("Width", colDef)));
                }
                xElement.Add(colDefsElement);
            }

            // Add template if present
            if (element.Template != null)
            {
                var templateElement = new XElement("Template");
                templateElement.Add(ConvertToXElement(element.Template));
                xElement.Add(templateElement);
            }

            // Add item template if present
            if (element.ItemTemplate != null)
            {
                var itemTemplateElement = new XElement("ItemTemplate");
                itemTemplateElement.Add(ConvertToXElement(element.ItemTemplate));
                xElement.Add(itemTemplateElement);
            }

            // Add resources if present
            if (element.Resources.Count > 0)
            {
                var resourcesElement = new XElement("Resources");
                foreach (var resource in element.Resources)
                {
                    var resourceElement = new XElement("Style",
                        new XAttribute("Key", resource.Key));
                    
                    if (!string.IsNullOrWhiteSpace(resource.Value.TargetType))
                        resourceElement.Add(new XAttribute("TargetType", resource.Value.TargetType));
                    if (!string.IsNullOrWhiteSpace(resource.Value.BasedOn))
                        resourceElement.Add(new XAttribute("BasedOn", resource.Value.BasedOn));
                    
                    // Add style properties as child elements
                    foreach (var prop in resource.Value.Setters)
                    {
                        resourceElement.Add(new XElement("Setter",
                            new XAttribute("Property", prop.Key),
                            new XAttribute("Value", prop.Value)));
                    }
                    resourcesElement.Add(resourceElement);
                }
                xElement.Add(resourcesElement);
            }

            // Add bindings if present
            if (element.Bindings.Count > 0)
            {
                var bindingsElement = new XElement("Bindings");
                foreach (var binding in element.Bindings)
                {
                    var bindElement = new XElement("Binding",
                        new XAttribute("Property", binding.Key));
                    
                    if (!string.IsNullOrWhiteSpace(binding.Value.Path))
                        bindElement.Add(new XAttribute("Path", binding.Value.Path));
                    if (!string.IsNullOrWhiteSpace(binding.Value.Mode))
                        bindElement.Add(new XAttribute("Mode", binding.Value.Mode));
                    if (!string.IsNullOrWhiteSpace(binding.Value.ElementName))
                        bindElement.Add(new XAttribute("ElementName", binding.Value.ElementName));
                    if (!string.IsNullOrWhiteSpace(binding.Value.RelativeSource))
                        bindElement.Add(new XAttribute("RelativeSource", binding.Value.RelativeSource));
                    
                    bindingsElement.Add(bindElement);
                }
                xElement.Add(bindingsElement);
            }

            // Add triggers if present
            if (element.Triggers.Count > 0)
            {
                var triggersElement = new XElement("Triggers");
                foreach (var trigger in element.Triggers)
                {
                    var triggerElement = new XElement("Trigger",
                        new XAttribute("Property", trigger.Property ?? string.Empty),
                        new XAttribute("Value", trigger.Value ?? string.Empty));
                    // Add trigger actions as child elements
                    foreach (var setter in trigger.Setters)
                    {
                        triggerElement.Add(new XElement("Setter",
                            new XAttribute("Property", setter.Key),
                            new XAttribute("Value", setter.Value)));
                    }
                    triggersElement.Add(triggerElement);
                }
                xElement.Add(triggersElement);
            }

            // Add multi-triggers if present
            if (element.MultiTriggers.Count > 0)
            {
                var multiTriggersElement = new XElement("MultiTriggers");
                foreach (var multiTrigger in element.MultiTriggers)
                {
                    var multiTriggerElement = new XElement("MultiTrigger");
                    // Add conditions
                    foreach (var condition in multiTrigger.Conditions)
                    {
                        multiTriggerElement.Add(new XElement("Condition",
                            new XAttribute("Property", condition.Property),
                            new XAttribute("Value", condition.Value)));
                    }
                    // Add setters
                    foreach (var setter in multiTrigger.Setters)
                    {
                        multiTriggerElement.Add(new XElement("Setter",
                            new XAttribute("Property", setter.Key),
                            new XAttribute("Value", setter.Value)));
                    }
                    multiTriggersElement.Add(multiTriggerElement);
                }
                xElement.Add(multiTriggersElement);
            }

            // Add child elements (recursively preserves tree structure)
            foreach (var child in element.Children)
                xElement.Add(ConvertToXElement(child));

            return xElement;
        }

        #endregion
    }
}
