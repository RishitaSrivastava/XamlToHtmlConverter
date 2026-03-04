using System.Text;
using XamlToWebViewApp.Core.Common;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Converts StackPanel into HTML flex container.
    /// </summary>
    
    //[Renderer("StackPanel")]
    [Renderer(XamlElementConstants.StackPanel)]
    public class StackPanelRenderer : IElementRenderer
    {
        public string Render(IrElement element)
        {
            var html = new StringBuilder();

            //html.Append("<div style='display:flex;flex-direction:column;'>");
            html.Append(
    "<div style='display:flex;flex-direction:column;align-items:flex-start;'>");

            foreach (var child in element.Children)
            {
                var renderer =
                    RendererFactory.GetRenderer(child.Type);

                html.Append(renderer.Render(child));
            }

            html.Append("</div>");

            return html.ToString();
        }
    }
}