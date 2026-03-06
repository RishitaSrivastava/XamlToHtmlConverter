// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class IntermediateRepresentationXmlExporterTest
    {
        #region Tests for Export

        [Test]
        public void TestExportReturnsXDocumentWithRootElementWhenSimpleElementIsExported()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Root, Is.Not.Null);
            Assert.That(doc.Root!.Name.LocalName, Is.EqualTo("Button"));
        }

        [Test]
        public void TestExportIncludesPropertyAsAttributeWhenElementHasProperties()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Content"] = "Click Me";

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Attribute("Content")?.Value, Is.EqualTo("Click Me"));
        }

        [Test]
        public void TestExportIncludesMultiplePropertiesAsAttributesWhenElementHasSeveralProperties()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Width"] = "140";
            element.Properties["Height"] = "38";

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Attribute("Width")?.Value, Is.EqualTo("140"));
            Assert.That(doc.Root!.Attribute("Height")?.Value, Is.EqualTo("38"));
        }

        [Test]
        public void TestExportIncludesAttachedPropertyAsAttributeWithDotNotationWhenPresent()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Border");
            element.AttachedProperties["Grid.Row"] = "1";

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Attribute("Grid.Row")?.Value, Is.EqualTo("1"));
        }

        [Test]
        public void TestExportIncludesInnerTextAsTextNodeWhenElementHasInnerText()
        {
            //Setup
            var element = new IntermediateRepresentationElement("TextBlock");
            element.InnerText = "Hello World";

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Value, Does.Contain("Hello World"));
        }

        [Test]
        public void TestExportIncludesChildElementsAsNestedXmlWhenElementHasChildren()
        {
            //Setup
            var parent = new IntermediateRepresentationElement("StackPanel");
            var child = new IntermediateRepresentationElement("Button");
            child.Properties["Content"] = "OK";
            parent.Children.Add(child);

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(parent);

            //Assert
            Assert.That(doc.Root!.Elements().Count(), Is.EqualTo(1));
            Assert.That(doc.Root.Element("Button"), Is.Not.Null);
            Assert.That(doc.Root.Element("Button")!.Attribute("Content")?.Value, Is.EqualTo("OK"));
        }

        [Test]
        public void TestExportIncludesDeepNestedChildrenWhenHierarchyIsMultipleLevels()
        {
            //Setup
            var grid = new IntermediateRepresentationElement("Grid");
            var border = new IntermediateRepresentationElement("Border");
            var button = new IntermediateRepresentationElement("Button");
            button.Properties["Content"] = "Save";
            border.Children.Add(button);
            grid.Children.Add(border);

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(grid);

            //Assert
            var borderNode = doc.Root!.Element("Border");
            Assert.That(borderNode, Is.Not.Null);
            var buttonNode = borderNode!.Element("Button");
            Assert.That(buttonNode, Is.Not.Null);
            Assert.That(buttonNode!.Attribute("Content")?.Value, Is.EqualTo("Save"));
        }

        [Test]
        public void TestExportReturnsElementWithNoAttributesWhenElementHasNoPropertiesOrAttachedProperties()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Attributes().Count(), Is.EqualTo(0));
        }

        #endregion
    }
}
