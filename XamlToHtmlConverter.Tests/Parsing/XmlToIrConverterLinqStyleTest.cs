// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using NUnit.Framework;
using XamlToHtmlConverter.Parsing;

namespace XamlToHtmlConverter.Tests.Parsing
{
    [TestFixture]
    public class XmlToIrConverterLinqStyleTest
    {
        private XmlToIrConverterLinqStyle v_Converter = null!;

        private const string c_XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        [SetUp]
        public void SetUp()
        {
            v_Converter = new XmlToIrConverterLinqStyle();
        }

        #region Tests for Convert

        [Test]
        public void TestConvertReturnsElementWithCorrectTypeWhenSimpleElementIsProvided()
        {
            //Setup
            var xml = new XElement(XName.Get("Button", c_XamlNamespace));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Type, Is.EqualTo("Button"));
        }

        [Test]
        public void TestConvertMapsAttributesToPropertiesWhenElementHasAttributes()
        {
            //Setup
            var xml = new XElement(XName.Get("Button", c_XamlNamespace),
                new XAttribute("Content", "Click Me"),
                new XAttribute("Width", "120"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Properties["Content"], Is.EqualTo("Click Me"));
            Assert.That(result.Properties["Width"], Is.EqualTo("120"));
        }

        [Test]
        public void TestConvertMapsAttachedPropertyToAttachedPropertiesWhenDotAttributeIsPresent()
        {
            //Setup
            var xml = new XElement(XName.Get("Border", c_XamlNamespace),
                new XAttribute("Grid.Row", "1"),
                new XAttribute("Grid.Column", "2"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.AttachedProperties["Grid.Row"], Is.EqualTo("1"));
            Assert.That(result.AttachedProperties["Grid.Column"], Is.EqualTo("2"));
        }

        [Test]
        public void TestConvertConvertsChildElementsToChildrenWhenXmlHasNestedElements()
        {
            //Setup
            var xml = new XElement(XName.Get("StackPanel", c_XamlNamespace),
                new XElement(XName.Get("Button", c_XamlNamespace), new XAttribute("Content", "OK")),
                new XElement(XName.Get("Button", c_XamlNamespace), new XAttribute("Content", "Cancel")));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Children.Count, Is.EqualTo(2));
            Assert.That(result.Children[0].Type, Is.EqualTo("Button"));
            Assert.That(result.Children[0].Properties["Content"], Is.EqualTo("OK"));
            Assert.That(result.Children[1].Properties["Content"], Is.EqualTo("Cancel"));
        }

        [Test]
        public void TestConvertReturnsEmptyChildrenListWhenElementHasNoChildren()
        {
            //Setup
            var xml = new XElement(XName.Get("Button", c_XamlNamespace),
                new XAttribute("Content", "OK"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Children, Is.Empty);
        }

        [Test]
        public void TestConvertSetsInnerTextWhenElementContainsTextContent()
        {
            //Setup
            var xml = new XElement(XName.Get("TextBlock", c_XamlNamespace), "Hello World");

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.InnerText, Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestConvertExtractsRowDefinitionsWhenGridContainsRowDefinitionElements()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.RowDefinitions",
                    new XElement(ns + "RowDefinition", new XAttribute("Height", "Auto")),
                    new XElement(ns + "RowDefinition", new XAttribute("Height", "*"))));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.GridRowDefinitions.Count, Is.EqualTo(2));
            Assert.That(result.GridRowDefinitions[0], Is.EqualTo("Auto"));
            Assert.That(result.GridRowDefinitions[1], Is.EqualTo("*"));
        }

        [Test]
        public void TestConvertExtractsColumnDefinitionsWhenGridContainsColumnDefinitionElements()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.ColumnDefinitions",
                    new XElement(ns + "ColumnDefinition", new XAttribute("Width", "2*")),
                    new XElement(ns + "ColumnDefinition", new XAttribute("Width", "1*"))));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.GridColumnDefinitions.Count, Is.EqualTo(2));
            Assert.That(result.GridColumnDefinitions[0], Is.EqualTo("2*"));
            Assert.That(result.GridColumnDefinitions[1], Is.EqualTo("1*"));
        }

        #endregion
    }
}
