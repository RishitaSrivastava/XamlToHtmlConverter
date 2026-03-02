using XamlToWebViewApp.Core.IR;
using XamlToWebViewApp.Core.Rendering;

namespace XamlToWebViewApp.Core.Generators
{
    /// <summary>
    /// Generates final HTML document from IR tree.
    /// Delegates element rendering to registered renderers.
    /// </summary>
    public class HtmlGenerator
    {
        /// <summary>
        /// Generates complete HTML page from IR root element.
        /// </summary>
        public string Generate(IrElement root)
        {
            var renderer =
                RendererFactory.GetRenderer(root.Type);

            string body = renderer.Render(root);

            return WrapHtml(body);
        }

        /// <summary>
        /// Wraps generated HTML body inside full HTML document.
        /// </summary>
        private string WrapHtml(string body)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'/>
<title>XAML Preview</title>
</head>
<body>
{body}
</body>
</html>";
        }
    }
}