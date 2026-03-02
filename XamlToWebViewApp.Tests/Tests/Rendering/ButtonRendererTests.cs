using Xunit;
using XamlToWebViewApp.Core.Rendering;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Tests.Rendering
{
    /// <summary>
    /// Tests HTML generation for ButtonRenderer.
    /// </summary>
    public class ButtonRendererTests
    {
        [Fact]
        public void Render_ShouldGenerateButtonHtml()
        {
            // Arrange
            var renderer = new ButtonRenderer();

            var element = new IrElement
            {
                Type = "Button"
            };

            element.Properties["Content"] = "Click Me";

            // Act
            string html = renderer.Render(element);

            // Assert
            Assert.Contains("<button>", html);
            Assert.Contains("Click Me", html);
        }
    }
}