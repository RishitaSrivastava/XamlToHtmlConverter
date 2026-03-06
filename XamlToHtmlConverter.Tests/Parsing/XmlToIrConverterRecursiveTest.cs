// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using NUnit.Framework;
using XamlToHtmlConverter.Parsing;

namespace XamlToHtmlConverter.Tests.Parsing
{
    [TestFixture]
    public class XmlToIrConverterRecursiveTest
    {
        private XmlToIrConverterRecursive v_Converter = null!;

        private const string c_XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string c_XamlXNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        [SetUp]
        public void SetUp()
        {
            v_Converter = new XmlToIrConverterRecursive();
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
        public void TestConvertMapsRegularAttributesToPropertiesWhenAttributesArePresent()
        {
            //Setup
            var xml = new XElement(XName.Get("Button", c_XamlNamespace),
                new XAttribute("Content", "Save"),
                new XAttribute("Width", "100"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Properties["Content"], Is.EqualTo("Save"));
            Assert.That(result.Properties["Width"], Is.EqualTo("100"));
        }

        [Test]
        public void TestConvertMapsAttachedPropertiesToAttachedPropertiesDictionaryWhenDotNotationIsUsed()
        {
            //Setup
            var xml = new XElement(XName.Get("Border", c_XamlNamespace),
                new XAttribute("Grid.Row", "2"),
                new XAttribute("Grid.ColumnSpan", "3"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.AttachedProperties["Grid.Row"], Is.EqualTo("2"));
            Assert.That(result.AttachedProperties["Grid.ColumnSpan"], Is.EqualTo("3"));
        }

        [Test]
        public void TestConvertBuildsChildHierarchyWhenElementHasNestedChildren()
        {
            //Setup
            var xml = new XElement(XName.Get("StackPanel", c_XamlNamespace),
                new XElement(XName.Get("TextBlock", c_XamlNamespace), new XAttribute("Text", "Label")),
                new XElement(XName.Get("Button", c_XamlNamespace), new XAttribute("Content", "OK")));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Children.Count, Is.EqualTo(2));
            Assert.That(result.Children[0].Type, Is.EqualTo("TextBlock"));
            Assert.That(result.Children[1].Type, Is.EqualTo("Button"));
        }

        [Test]
        public void TestConvertSetsParentReferenceOnChildWhenConvertingHierarchy()
        {
            //Setup
            var xml = new XElement(XName.Get("Grid", c_XamlNamespace),
                new XElement(XName.Get("Button", c_XamlNamespace), new XAttribute("Content", "OK")));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Children[0].Parent, Is.SameAs(result));
        }

        [Test]
        public void TestConvertSetsInnerTextWhenElementHasDirectTextContent()
        {
            //Setup
            var xml = new XElement(XName.Get("TextBlock", c_XamlNamespace), "Hello World");

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.InnerText, Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestConvertExtractsRowDefinitionsWhenGridHasExplicitRowDefinitions()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.RowDefinitions",
                    new XElement(ns + "RowDefinition", new XAttribute("Height", "Auto")),
                    new XElement(ns + "RowDefinition", new XAttribute("Height", "*")),
                    new XElement(ns + "RowDefinition", new XAttribute("Height", "Auto"))));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.GridRowDefinitions.Count, Is.EqualTo(3));
            Assert.That(result.GridRowDefinitions[0], Is.EqualTo("Auto"));
            Assert.That(result.GridRowDefinitions[1], Is.EqualTo("*"));
        }

        [Test]
        public void TestConvertExtractsColumnDefinitionsWhenGridHasExplicitColumnDefinitions()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.ColumnDefinitions",
                    new XElement(ns + "ColumnDefinition", new XAttribute("Width", "2*")),
                    new XElement(ns + "ColumnDefinition", new XAttribute("Width", "3*")),
                    new XElement(ns + "ColumnDefinition", new XAttribute("Width", "1*"))));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.GridColumnDefinitions.Count, Is.EqualTo(3));
            Assert.That(result.GridColumnDefinitions[0], Is.EqualTo("2*"));
            Assert.That(result.GridColumnDefinitions[1], Is.EqualTo("3*"));
            Assert.That(result.GridColumnDefinitions[2], Is.EqualTo("1*"));
        }

        [Test]
        public void TestConvertAppliesExplicitStyleSettersWhenStaticResourceIsReferenced()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xns = XNamespace.Get(c_XamlXNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.Resources",
                    new XElement(ns + "Style",
                        new XAttribute(xns + "Key", "PrimaryButton"),
                        new XAttribute("TargetType", "Button"),
                        new XElement(ns + "Setter",
                            new XAttribute("Property", "Width"),
                            new XAttribute("Value", "140")))),
                new XElement(ns + "Button",
                    new XAttribute("Style", "{StaticResource PrimaryButton}")));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert - style setters should be applied to the button
            Assert.That(result.Children[0].Properties.ContainsKey("Width"), Is.True);
            Assert.That(result.Children[0].Properties["Width"], Is.EqualTo("140"));
        }

        [Test]
        public void TestConvertAppliesImplicitStyleWhenStyleTargetTypeMatchesElementType()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Grid.Resources",
                    new XElement(ns + "Style",
                        new XAttribute("TargetType", "TextBlock"),
                        new XElement(ns + "Setter",
                            new XAttribute("Property", "Margin"),
                            new XAttribute("Value", "4")))),
                new XElement(ns + "TextBlock",
                    new XAttribute("Text", "Hello")));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert - implicit style should be applied
            Assert.That(result.Children[0].Properties.ContainsKey("Margin"), Is.True);
            Assert.That(result.Children[0].Properties["Margin"], Is.EqualTo("4"));
        }

        [Test]
        public void TestConvertReturnsEmptyChildrenListWhenLeafElementHasNoChildren()
        {
            //Setup
            var xml = new XElement(XName.Get("Button", c_XamlNamespace),
                new XAttribute("Content", "Submit"));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            Assert.That(result.Children, Is.Empty);
        }

        [Test]
        public void TestConvertHandlesDeepNestingCorrectlyWhenMultipleLevelsArePresent()
        {
            //Setup
            var ns = XNamespace.Get(c_XamlNamespace);
            var xml = new XElement(ns + "Grid",
                new XElement(ns + "Border",
                    new XElement(ns + "StackPanel",
                        new XElement(ns + "Button", new XAttribute("Content", "Deep")))));

            //Act
            var result = v_Converter.Convert(xml);

            //Assert
            var border = result.Children[0];
            var stack = border.Children[0];
            var button = stack.Children[0];
            Assert.That(button.Type, Is.EqualTo("Button"));
            Assert.That(button.Properties["Content"], Is.EqualTo("Deep"));
        }

        #endregion
    }
}
