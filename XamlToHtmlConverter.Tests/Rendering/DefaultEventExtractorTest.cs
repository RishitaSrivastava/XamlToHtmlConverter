// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class DefaultEventExtractorTest
    {
        private DefaultEventExtractor v_Extractor = null!;

        [SetUp]
        public void SetUp()
        {
            v_Extractor = new DefaultEventExtractor();
        }

        #region Tests for Extract

        [Test]
        public void TestExtractReturnsClickAttributeWhenClickEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Click"] = "Save_Click";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-click"), Is.True);
            Assert.That(result["data-event-click"], Is.EqualTo("Save_Click"));
        }

        [Test]
        public void TestExtractReturnsTextChangedAttributeWhenTextChangedEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("TextBox");
            element.Properties["TextChanged"] = "OnTextChanged";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-textchanged"), Is.True);
            Assert.That(result["data-event-textchanged"], Is.EqualTo("OnTextChanged"));
        }

        [Test]
        public void TestExtractReturnsCheckedAttributeWhenCheckedEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("CheckBox");
            element.Properties["Checked"] = "OnChecked";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-checked"), Is.True);
        }

        [Test]
        public void TestExtractReturnsUncheckedAttributeWhenUncheckedEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("CheckBox");
            element.Properties["Unchecked"] = "OnUnchecked";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-unchecked"), Is.True);
        }

        [Test]
        public void TestExtractReturnsLoadedAttributeWhenLoadedEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.Properties["Loaded"] = "Grid_Loaded";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-loaded"), Is.True);
        }

        [Test]
        public void TestExtractReturnsSelectionChangedAttributeWhenSelectionChangedEventIsDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("ComboBox");
            element.Properties["SelectionChanged"] = "OnSelectionChanged";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-selectionchanged"), Is.True);
        }

        [Test]
        public void TestExtractReturnsEmptyDictionaryWhenElementHasNoKnownEvents()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Width"] = "100";
            element.Properties["Content"] = "OK";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TestExtractReturnsEmptyDictionaryWhenElementHasNoProperties()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void TestExtractDoesNotIncludeNonEventPropertiesInResultWhenMixedPropertiesExist()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Width"] = "120";
            element.Properties["Click"] = "Logout_Click";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.ContainsKey("data-event-click"), Is.True);
        }

        [Test]
        public void TestExtractReturnsMultipleAttributesWhenMultipleKnownEventsAreDefinedOnElement()
        {
            //Setup
            var element = new IntermediateRepresentationElement("TextBox");
            element.Properties["TextChanged"] = "OnChanged";
            element.Properties["Loaded"] = "OnLoaded";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsKey("data-event-textchanged"), Is.True);
            Assert.That(result.ContainsKey("data-event-loaded"), Is.True);
        }

        [Test]
        public void TestExtractUsesLowerCaseKeyNamesWhenEventNamesContainUpperCaseLetters()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");
            element.Properties["Click"] = "Handler";

            //Act
            var result = v_Extractor.Extract(element);

            //Assert
            Assert.That(result.ContainsKey("data-event-Click"), Is.False);
            Assert.That(result.ContainsKey("data-event-click"), Is.True);
        }

        #endregion
    }
}
