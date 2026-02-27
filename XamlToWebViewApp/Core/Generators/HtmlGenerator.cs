using System.Text;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Core.Generators
{
    /// <summary>
    /// Converts IR elements into HTML markup.
    /// Supports recursive rendering.
    /// </summary>
    public class HtmlGenerator
    {
        /// <summary>
        /// Generates complete HTML from IR
        /// </summary>
        public string Generate(IrElement root)
        {
            var sb = new StringBuilder();

            BuildHtml(root, sb);

            return WrapHtml(sb.ToString());
        }

        // Recursively build HTML
        private void BuildHtml(IrElement element, StringBuilder sb)
        {
            switch (element.Type)
            {
                case "Button":
                    sb.Append($"<button>{GetText(element)}</button>");
                    break;

                case "TextBlock":
                    sb.Append($"<span>{GetText(element)}</span>");
                    break;

                case "StackPanel":
                    sb.Append("<div style='display:flex;flex-direction:column;'>");
                    break;

                default:
                    sb.Append("<div>");
                    break;
            }

            // Render children
            foreach (var child in element.Children)
            {
                BuildHtml(child, sb);
            }

            // Close tag
            sb.Append("</div>");
        }

        // Get display text
        private string GetText(IrElement element)
        {
            if (element.Properties.TryGetValue("Content", out var content))
                return content;

            if (element.Properties.TryGetValue("Text", out var text))
                return text;

            return string.Empty;
        }

        // Wrap in full HTML document
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