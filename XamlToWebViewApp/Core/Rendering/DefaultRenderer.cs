/// <summary>
/// Responsible for converting a specific XAML IR element
/// into its equivalent HTML representation.
/// 
/// Part of the XAML → IR → HTML rendering pipeline.
/// </summary>

using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Fallback renderer used when no specific renderer exists.
    /// Ensures conversion pipeline never fails.
    /// </summary>
    public class DefaultRenderer : IElementRenderer
    {
        public string Render(IrElement element)
        {
            return $"<div data-unknown='{element.Type}'></div>";
        }
    }
}