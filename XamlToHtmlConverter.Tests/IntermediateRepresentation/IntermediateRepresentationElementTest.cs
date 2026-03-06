// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Tests.IntermediateRepresentation
{
    [TestFixture]
    public class IntermediateRepresentationElementTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructorSetsTypeCorrectlyWhenValidTypeIsProvided()
        {
            //Act
            var element = new IntermediateRepresentationElement("Button");

            //Assert
            Assert.That(element.Type, Is.EqualTo("Button"));
        }

        [Test]
        public void TestConstructorInitializesPropertiesAsCaseInsensitiveDictionaryWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Grid");
            element.Properties["Width"] = "100";

            //Assert
            Assert.That(element.Properties["width"], Is.EqualTo("100"));
            Assert.That(element.Properties["WIDTH"], Is.EqualTo("100"));
        }

        [Test]
        public void TestConstructorInitializesAttachedPropertiesAsCaseInsensitiveDictionaryWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Border");
            element.AttachedProperties["Grid.Row"] = "1";

            //Assert
            Assert.That(element.AttachedProperties["grid.row"], Is.EqualTo("1"));
        }

        [Test]
        public void TestConstructorInitializesChildrenAsEmptyListWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("StackPanel");

            //Assert
            Assert.That(element.Children, Is.Not.Null);
            Assert.That(element.Children, Is.Empty);
        }

        [Test]
        public void TestConstructorInitializesGridRowDefinitionsAsEmptyListWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Grid");

            //Assert
            Assert.That(element.GridRowDefinitions, Is.Not.Null);
            Assert.That(element.GridRowDefinitions, Is.Empty);
        }

        [Test]
        public void TestConstructorInitializesGridColumnDefinitionsAsEmptyListWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Grid");

            //Assert
            Assert.That(element.GridColumnDefinitions, Is.Not.Null);
            Assert.That(element.GridColumnDefinitions, Is.Empty);
        }

        [Test]
        public void TestConstructorInitializesResourcesAsEmptyDictionaryWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Window");

            //Assert
            Assert.That(element.Resources, Is.Not.Null);
            Assert.That(element.Resources, Is.Empty);
        }

        [Test]
        public void TestConstructorSetsInnerTextToNullWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("TextBlock");

            //Assert
            Assert.That(element.InnerText, Is.Null);
        }

        [Test]
        public void TestConstructorSetsParentToNullWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Button");

            //Assert
            Assert.That(element.Parent, Is.Null);
        }

        [Test]
        public void TestConstructorSetsTemplateToNullWhenCreated()
        {
            //Act
            var element = new IntermediateRepresentationElement("Button");

            //Assert
            Assert.That(element.Template, Is.Null);
        }

        [Test]
        public void TestConstructorAcceptsEmptyStringTypeWhenProvided()
        {
            //Act
            var element = new IntermediateRepresentationElement(string.Empty);

            //Assert
            Assert.That(element.Type, Is.EqualTo(string.Empty));
        }

        #endregion

        #region Tests for Properties

        [Test]
        public void TestPropertiesStoresAndRetrievesValueCorrectlyWhenSet()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");

            //Act
            element.Properties["Content"] = "Click Me";

            //Assert
            Assert.That(element.Properties["Content"], Is.EqualTo("Click Me"));
        }

        [Test]
        public void TestInnerTextIsSettableAndRetrievableWhenAssigned()
        {
            //Setup
            var element = new IntermediateRepresentationElement("TextBlock");

            //Act
            element.InnerText = "Hello World";

            //Assert
            Assert.That(element.InnerText, Is.EqualTo("Hello World"));
        }

        [Test]
        public void TestParentIsSettableWhenAssignedAnotherElement()
        {
            //Setup
            var parent = new IntermediateRepresentationElement("Grid");
            var child = new IntermediateRepresentationElement("Button");

            //Act
            child.Parent = parent;

            //Assert
            Assert.That(child.Parent, Is.SameAs(parent));
        }

        [Test]
        public void TestTemplateIsSettableWhenAssignedAnotherElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            var template = new IntermediateRepresentationElement("ControlTemplate");

            //Act
            element.Template = template;

            //Assert
            Assert.That(element.Template, Is.SameAs(template));
        }

        [Test]
        public void TestChildrenIsReassignableWhenSetToNewList()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var newChildren = new List<IntermediateRepresentationElement>
            {
                new IntermediateRepresentationElement("Button"),
                new IntermediateRepresentationElement("TextBlock")
            };

            //Act
            element.Children = newChildren;

            //Assert
            Assert.That(element.Children.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestGridRowDefinitionsAcceptsMultipleEntriesWhenAdded()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");

            //Act
            element.GridRowDefinitions.Add("Auto");
            element.GridRowDefinitions.Add("*");
            element.GridRowDefinitions.Add("2*");

            //Assert
            Assert.That(element.GridRowDefinitions.Count, Is.EqualTo(3));
            Assert.That(element.GridRowDefinitions[0], Is.EqualTo("Auto"));
            Assert.That(element.GridRowDefinitions[1], Is.EqualTo("*"));
            Assert.That(element.GridRowDefinitions[2], Is.EqualTo("2*"));
        }

        [Test]
        public void TestResourcesStoresStyleByKeyWhenAdded()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Window");
            var style = new IntermediateRepresentationStyle { Key = "PrimaryButton", TargetType = "Button" };

            //Act
            element.Resources["PrimaryButton"] = style;

            //Assert
            Assert.That(element.Resources["PrimaryButton"], Is.SameAs(style));
        }

        #endregion
    }
}
