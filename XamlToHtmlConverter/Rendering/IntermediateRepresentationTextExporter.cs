// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides functionality to export an IR element tree
    /// as a readable text format suitable for viewing in text editors.
    /// Preserves the hierarchical structure with indentation and formatting.
    /// </summary>
    public static class IntermediateRepresentationTextExporter
    {
        #region Public Methods

        /// <summary>
        /// Converts the root IR element and its subtree into a formatted text representation
        /// showing the complete tree structure with properties, bindings, templates, and layouts.
        /// </summary>
        /// <param name="root">The root IR element to export.</param>
        /// <returns>A formatted string representing the IR tree structure.</returns>
        public static string Export(IntermediateRepresentationElement root)
        {
            // Capacity optimized for full IR tree export (medium-large documents)
            var sb = new StringBuilder(1023);
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine("                   IR TREE STRUCTURE                        ");
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine();
            
            ConvertToText(root, 0, sb);
            
            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("═══════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        /// <summary>
        /// Saves the IR tree structure to a text file.
        /// </summary>
        /// <param name="root">The root IR element to export.</param>
        /// <param name="filePath">The file path where the text will be saved.</param>
        public static void SaveToFile(IntermediateRepresentationElement root, string filePath)
        {
            var content = Export(root);
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively converts an IR element into formatted text representation,
        /// showing element type, properties, bindings, templates, and child hierarchy.
        /// </summary>
        /// <param name="element">The IR element to convert.</param>
        /// <param name="indent">The current indentation level in spaces.</param>
        /// <param name="sb">The StringBuilder to append text to.</param>
        private static void ConvertToText(IntermediateRepresentationElement element, int indent, StringBuilder sb)
        {
            var space = new string(' ', indent);
            var elementLine = $"{space}[{element.Type}]";
            sb.AppendLine(elementLine);

            // Add DataContext if present
            if (!string.IsNullOrWhiteSpace(element.DataContext))
                sb.AppendLine($"{space}  DataContext: {element.DataContext}");

            // Add regular properties
            foreach (var prop in element.Properties)
                sb.AppendLine($"{space}  prop: {prop.Key}={prop.Value}");

            // Add attached properties
            foreach (var attached in element.AttachedProperties)
                sb.AppendLine($"{space}  Attached: {attached.Key}={attached.Value}");

            // Add inner text
            if (!string.IsNullOrWhiteSpace(element.InnerText))
                sb.AppendLine($"{space}  Text: {element.InnerText}");

            // Add Grid definitions if present
            if (element.GridRowDefinitions.Count > 0)
            {
                sb.AppendLine($"{space}  GridRowDefinitions:");
                foreach (var rowDef in element.GridRowDefinitions)
                    sb.AppendLine($"{space}    - Height: {rowDef}");
            }

            if (element.GridColumnDefinitions.Count > 0)
            {
                sb.AppendLine($"{space}  GridColumnDefinitions:");
                foreach (var colDef in element.GridColumnDefinitions)
                    sb.AppendLine($"{space}    - Width: {colDef}");
            }

            // Add bindings if present
            if (element.Bindings.Count > 0)
            {
                sb.AppendLine($"{space}  Bindings:");
                foreach (var binding in element.Bindings)
                {
                    var bindingInfo = $"{space}    - Property: {binding.Key}";
                    if (!string.IsNullOrWhiteSpace(binding.Value.Path))
                        bindingInfo += $", Path: {binding.Value.Path}";
                    if (!string.IsNullOrWhiteSpace(binding.Value.Mode))
                        bindingInfo += $", Mode: {binding.Value.Mode}";
                    if (!string.IsNullOrWhiteSpace(binding.Value.ElementName))
                        bindingInfo += $", ElementName: {binding.Value.ElementName}";
                    if (!string.IsNullOrWhiteSpace(binding.Value.RelativeSource))
                        bindingInfo += $", RelativeSource: {binding.Value.RelativeSource}";
                    sb.AppendLine(bindingInfo);
                }
            }

            // Add resources if present
            if (element.Resources.Count > 0)
            {
                sb.AppendLine($"{space}  Resources:");
                foreach (var resource in element.Resources)
                {
                    sb.AppendLine($"{space}    - Style Key: {resource.Key}");
                    if (!string.IsNullOrWhiteSpace(resource.Value.TargetType))
                        sb.AppendLine($"{space}      TargetType: {resource.Value.TargetType}");
                    if (!string.IsNullOrWhiteSpace(resource.Value.BasedOn))
                        sb.AppendLine($"{space}      BasedOn: {resource.Value.BasedOn}");
                    foreach (var setter in resource.Value.Setters)
                        sb.AppendLine($"{space}      Setter: {setter.Key}={setter.Value}");
                }
            }

            // Add triggers if present
            if (element.Triggers.Count > 0)
            {
                sb.AppendLine($"{space}  Triggers:");
                foreach (var trigger in element.Triggers)
                {
                    sb.AppendLine($"{space}    - Trigger: Property={trigger.Property}, Value={trigger.Value}");
                    foreach (var setter in trigger.Setters)
                        sb.AppendLine($"{space}      Setter: {setter.Key}={setter.Value}");
                }
            }

            // Add multi-triggers if present
            if (element.MultiTriggers.Count > 0)
            {
                sb.AppendLine($"{space}  MultiTriggers:");
                foreach (var multiTrigger in element.MultiTriggers)
                {
                    sb.AppendLine($"{space}    - MultiTrigger:");
                    foreach (var condition in multiTrigger.Conditions)
                        sb.AppendLine($"{space}      Condition: {condition.Property}={condition.Value}");
                    foreach (var setter in multiTrigger.Setters)
                        sb.AppendLine($"{space}      Setter: {setter.Key}={setter.Value}");
                }
            }

            // Add template if present
            if (element.Template != null)
            {
                sb.AppendLine($"{space}  [Template]");
                ConvertToText(element.Template, indent + 4, sb);
            }

            // Add item template if present
            if (element.ItemTemplate != null)
            {
                sb.AppendLine($"{space}  [ItemTemplate]");
                ConvertToText(element.ItemTemplate, indent + 4, sb);
            }

            // Add child elements (recursively preserve tree structure)
            foreach (var child in element.Children)
                ConvertToText(child, indent + 2, sb);
        }

        #endregion
    }
}
