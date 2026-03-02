using System;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Attribute used to associate a renderer
    /// with a specific XAML element type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RendererAttribute : Attribute
    {
        public string ElementType { get; }

        public RendererAttribute(string elementType)
        {
            ElementType = elementType;
        }
    }
}