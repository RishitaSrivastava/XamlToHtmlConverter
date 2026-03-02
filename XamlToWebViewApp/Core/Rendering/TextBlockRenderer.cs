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
    /// Converts TextBlock IR element into HTML span element.
    /// TextBlock represents inline text content in XAML.
    /// </summary>
    [Renderer("TextBlock")]
    public class TextBlockRenderer : IElementRenderer
    {
        public string Render(IrElement element)
        {
            string text =
                element.InnerText ??
                element.Properties.GetValueOrDefault("Text", "");

            return $"<span>{text}</span>";
        }
    }
}