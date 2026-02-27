using Xunit;
using XamlToWebViewApp.Core.Parsers;
using System.Xml.Linq;
using System.IO;

namespace XamlToWebViewApp.Tests.Parsers
{
    /// <summary>
    /// Unit tests for XamlParser.
    /// Verifies that XAML/XML files are correctly loaded
    /// and root elements are parsed using LINQ to XML.
    /// </summary>
    public class XamlParserTests
    {
        [Fact]
        public void Parse_Should_ReturnRootElement()
        {
            // Arrange
            var parser = new XamlParser();

            string xml =
                "<StackPanel><TextBlock Text='Hello'/></StackPanel>";

            File.WriteAllText("test.xml", xml);

            // Act
            XElement result = parser.Parse("test.xml");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("StackPanel", result.Name.LocalName);
        }
    }
}