using Xunit;
using XamlToWebViewApp.Core.Generators;
using XamlToWebViewApp.Core.IR;

namespace XamlToWebViewApp.Tests.Generators
{
    /// <summary>
    /// Unit tests for HtmlGenerator.
    /// Ensures IR elements are converted into
    /// expected HTML markup.
    /// </summary>
    public class HtmlGeneratorTests
    {
        [Fact]
        public void Generate_Should_CreateButtonHtml()
        {
            // Arrange
            var ir = new IrElement
            {
                Type = "Button"
            };

            ir.Properties["Content"] = "Click Me";

            var generator = new HtmlGenerator();

            // Act
            string html = generator.Generate(ir);

            // Assert
            Assert.Contains("<button>", html);
            Assert.Contains("Click Me", html);
        }
    }
}