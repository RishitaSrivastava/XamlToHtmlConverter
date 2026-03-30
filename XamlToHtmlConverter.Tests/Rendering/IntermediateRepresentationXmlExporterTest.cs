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

        #region Tests for Enhanced Export Features

        [Test]
        public void TestExportIncludesGridRowDefinitionsWhenPresent()
        {
            //Setup
            var grid = new IntermediateRepresentationElement("Grid");
            grid.GridRowDefinitions.Add("Auto");
            grid.GridRowDefinitions.Add("*");

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(grid);

            //Assert
            var rowDefsElement = doc.Root!.Element("GridRowDefinitions");
            Assert.That(rowDefsElement, Is.Not.Null);
            var rowDefs = rowDefsElement!.Elements("RowDefinition").ToList();
            Assert.That(rowDefs.Count, Is.EqualTo(2));
            Assert.That(rowDefs[0].Attribute("Height")?.Value, Is.EqualTo("Auto"));
            Assert.That(rowDefs[1].Attribute("Height")?.Value, Is.EqualTo("*"));
        }

        [Test]
        public void TestExportIncludesGridColumnDefinitionsWhenPresent()
        {
            //Setup
            var grid = new IntermediateRepresentationElement("Grid");
            grid.GridColumnDefinitions.Add("2*");
            grid.GridColumnDefinitions.Add("1*");

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(grid);

            //Assert
            var colDefsElement = doc.Root!.Element("GridColumnDefinitions");
            Assert.That(colDefsElement, Is.Not.Null);
            var colDefs = colDefsElement!.Elements("ColumnDefinition").ToList();
            Assert.That(colDefs.Count, Is.EqualTo(2));
            Assert.That(colDefs[0].Attribute("Width")?.Value, Is.EqualTo("2*"));
            Assert.That(colDefs[1].Attribute("Width")?.Value, Is.EqualTo("1*"));
        }

        [Test]
        public void TestExportIncludesDataContextWhenPresent()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Window");
            element.DataContext = "ViewModelName";

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            Assert.That(doc.Root!.Attribute("DataContext")?.Value, Is.EqualTo("ViewModelName"));
        }

        [Test]
        public void TestExportIncludesBindingsWhenPresent()
        {
            //Setup
            var button = new IntermediateRepresentationElement("Button");
            var binding = new IntermediateRepresentationBinding
            {
                Path = "SaveCommand",
                Mode = "OneWay"
            };
            button.Bindings["Command"] = binding;

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(button);

            //Assert
            var bindingsElement = doc.Root!.Element("Bindings");
            Assert.That(bindingsElement, Is.Not.Null);
            var bindElement = bindingsElement!.Element("Binding");
            Assert.That(bindElement, Is.Not.Null);
            Assert.That(bindElement!.Attribute("Property")?.Value, Is.EqualTo("Command"));
            Assert.That(bindElement.Attribute("Path")?.Value, Is.EqualTo("SaveCommand"));
            Assert.That(bindElement.Attribute("Mode")?.Value, Is.EqualTo("OneWay"));
        }

        [Test]
        public void TestExportIncludesBindingElementNameAndRelativeSourceWhenPresent()
        {
            //Setup
            var element = new IntermediateRepresentationElement("TextBox");
            var binding = new IntermediateRepresentationBinding
            {
                Path = "SelectedItem",
                ElementName = "MyListBox",
                RelativeSource = "Self"
            };
            element.Bindings["Text"] = binding;

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            var bindElement = doc.Root!.Element("Bindings")!.Element("Binding");
            Assert.That(bindElement!.Attribute("ElementName")?.Value, Is.EqualTo("MyListBox"));
            Assert.That(bindElement.Attribute("RelativeSource")?.Value, Is.EqualTo("Self"));
        }

        [Test]
        public void TestExportIncludesTemplateWhenPresent()
        {
            //Setup
            var listView = new IntermediateRepresentationElement("ListView");
            var itemTemplate = new IntermediateRepresentationElement("Border");
            itemTemplate.Properties["Padding"] = "5";
            var textBlock = new IntermediateRepresentationElement("TextBlock");
            itemTemplate.Children.Add(textBlock);
            listView.ItemTemplate = itemTemplate;

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(listView);

            //Assert
            var templateElement = doc.Root!.Element("ItemTemplate");
            Assert.That(templateElement, Is.Not.Null);
            var borderElement = templateElement!.Element("Border");
            Assert.That(borderElement, Is.Not.Null);
            Assert.That(borderElement!.Attribute("Padding")?.Value, Is.EqualTo("5"));
            var textBlockElement = borderElement.Element("TextBlock");
            Assert.That(textBlockElement, Is.Not.Null);
        }

        [Test]
        public void TestExportIncludesResourcesWhenPresent()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Window");
            var style = new IntermediateRepresentationStyle
            {
                Key = "MyButtonStyle",
                TargetType = "Button",
                BasedOn = "DefaultButtonStyle"
            };
            style.Setters["Background"] = "Blue";
            style.Setters["Foreground"] = "White";
            element.Resources["MyButtonStyle"] = style;

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(element);

            //Assert
            var resourcesElement = doc.Root!.Element("Resources");
            Assert.That(resourcesElement, Is.Not.Null);
            var styleElement = resourcesElement!.Element("Style");
            Assert.That(styleElement, Is.Not.Null);
            Assert.That(styleElement!.Attribute("Key")?.Value, Is.EqualTo("MyButtonStyle"));
            Assert.That(styleElement.Attribute("TargetType")?.Value, Is.EqualTo("Button"));
            Assert.That(styleElement.Attribute("BasedOn")?.Value, Is.EqualTo("DefaultButtonStyle"));
            var setters = styleElement.Elements("Setter").ToList();
            Assert.That(setters.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestExportIncludesTriggersWhenPresent()
        {
            //Setup
            var button = new IntermediateRepresentationElement("Button");
            var trigger = new IntermediateRepresentationTrigger
            {
                Property = "IsMouseOver",
                Value = "True"
            };
            trigger.Setters["Background"] = "Red";
            trigger.Setters["Cursor"] = "Hand";
            button.Triggers.Add(trigger);

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(button);

            //Assert
            var triggersElement = doc.Root!.Element("Triggers");
            Assert.That(triggersElement, Is.Not.Null);
            var triggerElement = triggersElement!.Element("Trigger");
            Assert.That(triggerElement, Is.Not.Null);
            Assert.That(triggerElement!.Attribute("Property")?.Value, Is.EqualTo("IsMouseOver"));
            Assert.That(triggerElement.Attribute("Value")?.Value, Is.EqualTo("True"));
            var setters = triggerElement.Elements("Setter").ToList();
            Assert.That(setters.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestExportIncludesMultiTriggersWhenPresent()
        {
            //Setup
            var button = new IntermediateRepresentationElement("Button");
            var multiTrigger = new IntermediateRepresentationMultiTrigger();
            multiTrigger.Conditions.Add(("IsEnabled", "True"));
            multiTrigger.Conditions.Add(("IsMouseOver", "True"));
            multiTrigger.Setters["Background"] = "Gold";
            button.MultiTriggers.Add(multiTrigger);

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(button);

            //Assert
            var multiTriggersElement = doc.Root!.Element("MultiTriggers");
            Assert.That(multiTriggersElement, Is.Not.Null);
            var multiTriggerElement = multiTriggersElement!.Element("MultiTrigger");
            Assert.That(multiTriggerElement, Is.Not.Null);
            var conditions = multiTriggerElement!.Elements("Condition").ToList();
            Assert.That(conditions.Count, Is.EqualTo(2));
            Assert.That(conditions[0].Attribute("Property")?.Value, Is.EqualTo("IsEnabled"));
            Assert.That(conditions[1].Attribute("Property")?.Value, Is.EqualTo("IsMouseOver"));
            var setters = multiTriggerElement.Elements("Setter").ToList();
            Assert.That(setters.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestExportPreservesCompleteTreeStructureWithMultipleFeatures()
        {
            //Setup
            var grid = new IntermediateRepresentationElement("Grid");
            grid.DataContext = "MyViewModel";
            grid.GridRowDefinitions.Add("Auto");
            grid.GridColumnDefinitions.Add("*");

            var button = new IntermediateRepresentationElement("Button");
            button.Properties["Content"] = "Save";
            var binding = new IntermediateRepresentationBinding { Path = "SaveCommand" };
            button.Bindings["Command"] = binding;
            grid.Children.Add(button);

            //Act
            var doc = IntermediateRepresentationXmlExporter.Export(grid);

            //Assert
            Assert.That(doc.Root!.Attribute("DataContext")?.Value, Is.EqualTo("MyViewModel"));
            Assert.That(doc.Root.Element("GridRowDefinitions"), Is.Not.Null);
            Assert.That(doc.Root.Element("GridColumnDefinitions"), Is.Not.Null);
            var buttonElement = doc.Root.Element("Button");
            Assert.That(buttonElement, Is.Not.Null);
            Assert.That(buttonElement!.Attribute("Content")?.Value, Is.EqualTo("Save"));
            Assert.That(buttonElement.Element("Bindings"), Is.Not.Null);
        }

        #endregion
    }
}
