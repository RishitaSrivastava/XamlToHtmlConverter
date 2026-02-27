using System.Collections.Generic;

namespace XamlToWebViewApp.Core.IR
{
    /// <summary>
    /// Represents an intermediate representation (IR)
    /// node created from a XAML element.
    /// Stores element type, properties and children.
    /// </summary>
    public class IrElement
    {
        // Type of control (Button, TextBlock, StackPanel, etc.)
        public string Type { get; set; }

        // Stores attributes like Text, Content, Width, etc.
        public Dictionary<string, string> Properties { get; set; }
            = new Dictionary<string, string>();

        // Child elements
        public List<IrElement> Children { get; set; }
            = new List<IrElement>();
    }
}