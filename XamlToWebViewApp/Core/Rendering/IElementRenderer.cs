using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Defines contract for converting an IR element into HTML.
    /// Each XAML component implements its own renderer.
    /// </summary>
    public interface IElementRenderer
    {
        string Render(IrElement element);
    }
}