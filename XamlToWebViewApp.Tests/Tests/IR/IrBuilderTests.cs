using Xunit;
using XamlToWebViewApp.Core.IR;
using System.Xml.Linq;

namespace XamlToWebViewApp.Tests.IR
{
    /// <summary>
    /// Tests IR building logic which converts XElement
    /// structures into Intermediate Representation objects.
    /// </summary>
    public class IrBuilderTests
    {
        [Fact]
        public void Build_Should_CreateCorrectHierarchy()
        {
            // Arrange
            XElement element =
                XElement.Parse(
                    "<StackPanel><Button Content='Click'/></StackPanel>");

            var builder = new IrBuilder();

            // Act
            var ir = builder.Build(element);

            // Assert
            Assert.Equal("StackPanel", ir.Type);
            Assert.Single(ir.Children);
            Assert.Equal("Button", ir.Children[0].Type);
        }
    }
}