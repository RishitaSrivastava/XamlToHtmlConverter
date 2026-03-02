using Xunit;
using XamlToWebViewApp.Core.Rendering;

namespace XamlToWebViewApp.Tests.Rendering
{
    /// <summary>
    /// Tests automatic renderer discovery.
    /// </summary>
    public class RendererFactoryTests
    {
        [Fact]
        public void GetRenderer_ShouldReturnCorrectRenderer()
        {
            var renderer = RendererFactory.GetRenderer("Button");

            Assert.IsType<ButtonRenderer>(renderer);
        }
    }
}