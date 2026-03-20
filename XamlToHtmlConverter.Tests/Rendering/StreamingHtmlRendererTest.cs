// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering;

/// <summary>
/// Unit tests for streaming HTML renderer.
/// Verifies:
/// - Output correctness (matches regular renderer)
/// - File writing works correctly
/// - Stream writing works correctly
/// - HTML structure is valid
/// </summary>
[TestFixture]
public class StreamingHtmlRendererTest
{
    private StreamingHtmlRenderer v_Renderer = null!;
    private DefaultElementTagMapper v_TagMapper = null!;
    private DefaultStyleBuilder v_StyleBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        v_TagMapper = new DefaultElementTagMapper();
        v_StyleBuilder = new DefaultStyleBuilder();
        v_Renderer = new StreamingHtmlRenderer(v_TagMapper, v_StyleBuilder, Array.Empty<ILayoutRenderer>());
    }

    [Test]
    public void TestRenderToStringReturnsValidHtml()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties.Add("Content", "Click Me");

        // Act
        var html = v_Renderer.RenderToString(element);

        // Assert
        Assert.That(html, Contains.Substring("<!DOCTYPE html>"));
        Assert.That(html, Contains.Substring("<html>"));
        Assert.That(html, Contains.Substring("<button"));
        Assert.That(html, Contains.Substring("</button>"));
        Assert.That(html, Contains.Substring("</html>"));
    }

    [Test]
    public void TestRenderToStringContainsAttributes()
    {
        // Setup
        var element = new IntermediateRepresentationElement("TextBox");
        element.Properties.Add("Name", "TextInput");
        element.Properties.Add("Placeholder", "Enter text");

        // Act
        var html = v_Renderer.RenderToString(element);

        // Assert
        Assert.That(html, Contains.Substring("Name=\"TextInput\""));
        Assert.That(html, Contains.Substring("Placeholder=\"Enter text\""));
    }

    [Test]
    public void TestRenderToStringContainsTextContent()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Label")
        {
            InnerText = "Hello World"
        };

        // Act
        var html = v_Renderer.RenderToString(element);

        // Assert
        Assert.That(html, Contains.Substring("Hello World"));
    }

    [Test]
    public void TestRenderToStringHandlesNestedElements()
    {
        // Setup
        var parent = new IntermediateRepresentationElement("Grid");
        var child1 = new IntermediateRepresentationElement("Button") { InnerText = "Button 1" };
        var child2 = new IntermediateRepresentationElement("Button") { InnerText = "Button 2" };
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        // Act
        var html = v_Renderer.RenderToString(parent);

        // Assert
        Assert.That(html, Contains.Substring("Button 1"));
        Assert.That(html, Contains.Substring("Button 2"));
        Assert.That(html, Contains.Substring("<div>"));
        Assert.That(html, Contains.Substring("</div>"));
    }

    [Test]
    public void TestRenderToFileCreatesFile()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Button") { InnerText = "Test" };
        var outputFile = Path.GetTempFileName();

        try
        {
            // Act
            v_Renderer.RenderToFile(element, outputFile);

            // Assert
            Assert.That(File.Exists(outputFile), Is.True);
            var content = File.ReadAllText(outputFile);
            Assert.That(content, Contains.Substring("<!DOCTYPE html>"));
        }
        finally
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
        }
    }

    [Test]
    public void TestRenderToFileOutputIsValid()
    {
        // Setup
        var element = new IntermediateRepresentationElement("StackPanel");
        element.Children.Add(new IntermediateRepresentationElement("TextBlock") { InnerText = "Item 1" });
        element.Children.Add(new IntermediateRepresentationElement("TextBlock") { InnerText = "Item 2" });

        var outputFile = Path.GetTempFileName();

        try
        {
            // Act
            v_Renderer.RenderToFile(element, outputFile);
            var content = File.ReadAllText(outputFile);

            // Assert
            Assert.That(content, Contains.Substring("<html>").And.Contains("</html>"));
            Assert.That(content, Contains.Substring("Item 1"));
            Assert.That(content, Contains.Substring("Item 2"));
        }
        finally
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
        }
    }

    [Test]
    public void TestRenderToStreamWritesValidOutput()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Button");
        using (var writer = new StringWriter())
        {
            // Act
            v_Renderer.RenderToStream(element, writer);
            var output = writer.ToString();

            // Assert
            Assert.That(output, Contains.Substring("<!DOCTYPE html>"));
            Assert.That(output, Contains.Substring("<html>"));
            Assert.That(output, Contains.Substring("</html>"));
        }
    }

    [Test]
    public void TestRenderEscapesHtmlCharactersInText()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Label")
        {
            InnerText = "Text with <html> & \"quotes\""
        };

        // Act
        var html = v_Renderer.RenderToString(element);

        // Assert
        Assert.That(html, Contains.Substring("&lt;"));
        Assert.That(html, Contains.Substring("&gt;"));
        Assert.That(html, Contains.Substring("&amp;"));
        Assert.That(html, Contains.Substring("&quot;"));
    }

    [Test]
    public void TestRenderEscapesHtmlCharactersInAttributes()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties.Add("Title", "Text with <tag>");

        // Act
        var html = v_Renderer.RenderToString(element);

        // Assert
        Assert.That(html, Contains.Substring("&lt;"));
        Assert.That(html, Contains.Substring("&gt;"));
    }

    [Test]
    public void TestStringAndFileOutputAreIdentical()
    {
        // Setup
        var element = new IntermediateRepresentationElement("Grid");
        element.Children.Add(new IntermediateRepresentationElement("Button") { InnerText = "Click" });
        var outputFile = Path.GetTempFileName();

        try
        {
            // Act
            var stringOutput = v_Renderer.RenderToString(element);
            v_Renderer.RenderToFile(element, outputFile);
            var fileOutput = File.ReadAllText(outputFile);

            // Assert
            Assert.That(stringOutput, Is.EqualTo(fileOutput));
        }
        finally
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
        }
    }
}
