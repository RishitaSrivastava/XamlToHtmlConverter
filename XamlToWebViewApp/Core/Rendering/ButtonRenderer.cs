using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Generates HTML markup for the provided IR element.
    /// </summary>
    /// <param name="Button">Intermediate representation node.</param>
    /// <returns>Generated HTML string.</returns>
    [Renderer("Button")]
    public class ButtonRenderer : IElementRenderer
    {
        public string Render(IrElement element)
        {
            string content =
                element.Properties.ContainsKey("Content")
                ? element.Properties["Content"]
                : "Button";

            return $"<button>{content}</button>";
        }
    }
}