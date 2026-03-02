using Xunit;
using XamlToWebViewApp.Core.Rendering;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Tests.Rendering
{
    /// <summary>
    /// Tests TextBlock HTML rendering.
    /// </summary>
    public class TextBlockRendererTests
    {
        [Fact]
        public void Render_ShouldGenerateSpanHtml()
        {
            var renderer = new TextBlockRenderer();

            var element = new IrElement
            {
                Type = "TextBlock",
                InnerText = "Hello World"
            };

            string html = renderer.Render(element);

            Assert.Contains("<span>", html);
            Assert.Contains("Hello World", html);
        }
    }
}