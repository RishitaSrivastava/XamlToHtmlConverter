// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class WrapPanelLayoutRendererTest
    {
        private WrapPanelLayoutRenderer v_Renderer = null!;

        [SetUp]
        public void SetUp()
        {
            v_Renderer = new WrapPanelLayoutRenderer();
        }

        #region Tests for CanHandle

        [Test]
        public void TestCanHandleReturnsTrueWhenElementTypeIsWrapPanel()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");

            //Act
            var result = v_Renderer.CanHandle(element);

            //Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestCanHandleReturnsFalseWhenElementTypeIsStackPanel()
        {
            //Setup
            var element = new IntermediateRepresentationElement("StackPanel");

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

        #endregion

        #region Tests for ApplyLayout

        [Test]
        public void TestApplyLayoutAppendsFlexDisplayWhenCalled()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("display:flex;"));
        }

        [Test]
        public void TestApplyLayoutAppendsFlexWrapWhenCalled()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-wrap:wrap;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionByDefaultWhenOrientationIsNotSet()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenOrientationIsSetToHorizontal()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            element.Properties["Orientation"] = "Horizontal";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenOrientationIsSetToVertical()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            element.Properties["Orientation"] = "Vertical";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenOrientationIsSetToVerticalInLowerCase()
        {
            //Setup
            var element = new IntermediateRepresentationElement("WrapPanel");
            element.Properties["Orientation"] = "vertical";
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        #endregion
    }
}
