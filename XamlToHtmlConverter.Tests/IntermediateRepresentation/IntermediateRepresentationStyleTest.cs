// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Tests.IntermediateRepresentation
{
    [TestFixture]
    public class IntermediateRepresentationStyleTest
    {
        #region Tests for Properties

        [Test]
        public void TestKeyIsNullByDefaultWhenStyleIsCreated()
        {
            //Act
            var style = new IntermediateRepresentationStyle();

            //Assert
            Assert.That(style.Key, Is.Null);
        }

        [Test]
        public void TestTargetTypeIsNullByDefaultWhenStyleIsCreated()
        {
            //Act
            var style = new IntermediateRepresentationStyle();

            //Assert
            Assert.That(style.TargetType, Is.Null);
        }

        [Test]
        public void TestBasedOnIsNullByDefaultWhenStyleIsCreated()
        {
            //Act
            var style = new IntermediateRepresentationStyle();

            //Assert
            Assert.That(style.BasedOn, Is.Null);
        }

        [Test]
        public void TestSettersIsEmptyByDefaultWhenStyleIsCreated()
        {
            //Act
            var style = new IntermediateRepresentationStyle();

            //Assert
            Assert.That(style.Setters, Is.Not.Null);
            Assert.That(style.Setters, Is.Empty);
        }

        [Test]
        public void TestKeyIsSettableAndRetrievableWhenAssigned()
        {
            //Setup
            var style = new IntermediateRepresentationStyle();

            //Act
            style.Key = "PrimaryButton";

            //Assert
            Assert.That(style.Key, Is.EqualTo("PrimaryButton"));
        }

        [Test]
        public void TestTargetTypeIsSettableAndRetrievableWhenAssigned()
        {
            //Setup
            var style = new IntermediateRepresentationStyle();

            //Act
            style.TargetType = "Button";

            //Assert
            Assert.That(style.TargetType, Is.EqualTo("Button"));
        }

        [Test]
        public void TestBasedOnIsSettableAndRetrievableWhenAssigned()
        {
            //Setup
            var style = new IntermediateRepresentationStyle();

            //Act
            style.BasedOn = "BaseStyle";

            //Assert
            Assert.That(style.BasedOn, Is.EqualTo("BaseStyle"));
        }

        [Test]
        public void TestSettersStoresPropertyValuePairWhenAdded()
        {
            //Setup
            var style = new IntermediateRepresentationStyle();

            //Act
            style.Setters["Background"] = "LightSteelBlue";
            style.Setters["Width"] = "140";

            //Assert
            Assert.That(style.Setters["Background"], Is.EqualTo("LightSteelBlue"));
            Assert.That(style.Setters["Width"], Is.EqualTo("140"));
            Assert.That(style.Setters.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestSettersAllowsOverwritingExistingKeyWhenValueIsUpdated()
        {
            //Setup
            var style = new IntermediateRepresentationStyle();
            style.Setters["Margin"] = "4";

            //Act
            style.Setters["Margin"] = "8";

            //Assert
            Assert.That(style.Setters["Margin"], Is.EqualTo("8"));
        }

        #endregion
    }
}
