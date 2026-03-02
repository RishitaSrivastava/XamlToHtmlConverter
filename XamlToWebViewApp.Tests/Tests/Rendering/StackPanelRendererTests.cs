using Xunit;
using XamlToWebViewApp.Core.Rendering;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Tests.Rendering
{
    /// <summary>
    /// Tests StackPanel layout rendering.
    /// </summary>
    public class StackPanelRendererTests
    {
        [Fact]
        public void Render_ShouldRenderChildren()
        {
            var stackPanel = new IrElement
            {
                Type = "StackPanel"
            };

            stackPanel.Children.Add(new IrElement
            {
                Type = "TextBlock",
                InnerText = "Child Text"
            });

            var renderer = new StackPanelRenderer();

            string html = renderer.Render(stackPanel);

            Assert.Contains("display:flex", html);
            Assert.Contains("Child Text", html);
        }
    }
}