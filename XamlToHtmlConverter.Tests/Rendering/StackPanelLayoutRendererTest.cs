// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class StackPanelLayoutRendererTest
    {
        private StackPanelLayoutRenderer v_Renderer = null!;

        [SetUp]
        public void SetUp()
        {
            v_Renderer = new StackPanelLayoutRenderer();
        }

        #region Tests for CanHandle

        [Test]
        public void TestCanHandleReturnsTrueWhenElementTypeIsStackPanel()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");

            //Act
            var result = v_Renderer.CanHandle(element);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestCanHandleReturnsFalseWhenElementTypeIsGrid()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");

            //Act
            var result = v_Renderer.CanHandle(element);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestCanHandleReturnsFalseWhenElementTypeIsButton()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Button");

            //Act
            var result = v_Renderer.CanHandle(element);

            //Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestCanHandleReturnsFalseWhenElementTypeIsEmptyString()
        {
            //Setup
            var element = new IntermediateRepresentationElement(string.Empty);

            //Act
            var result = v_Renderer.CanHandle(element);

            //Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Tests for ApplyLayout

        [Test]
        public void TestApplyLayoutAppendsFlexDisplayWhenCalled()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("display:flex;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenOrientationIsNotSet()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenOrientationIsSetToVertical()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");
            element.Properties["Orientation"] = "Vertical";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenOrientationIsSetToHorizontal()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");
            element.Properties["Orientation"] = "Horizontal";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenOrientationIsSetToHorizontalInLowerCase()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");
            element.Properties["Orientation"] = "horizontal";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        #endregion
    }
}
