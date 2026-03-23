// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class DockPanelLayoutRendererTest
    {
        private DockPanelLayoutRenderer v_Renderer = null!;

        [SetUp]
        public void SetUp()
        {
            v_Renderer = new DockPanelLayoutRenderer();
        }

        #region Tests for CanHandle

        [Test]
        public void TestCanHandleReturnsTrueWhenElementTypeIsDockPanel()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");

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
        public void TestCanHandleReturnsFalseWhenElementTypeIsGrid()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");

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
            var element = new IntermediateRepresentationElement("DockPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("display:flex;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenChildIsDockedToTop()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var child = new IntermediateRepresentationElement("TextBlock");
            child.AttachedProperties["DockPanel.Dock"] = "Top";
            element.Children.Add(child);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenChildIsDockedToBottom()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var child = new IntermediateRepresentationElement("Border");
            child.AttachedProperties["DockPanel.Dock"] = "Bottom";
            element.Children.Add(child);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenChildIsDockedToLeft()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var child = new IntermediateRepresentationElement("Button");
            child.AttachedProperties["DockPanel.Dock"] = "Left";
            element.Children.Add(child);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenChildIsDockedToRight()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var child = new IntermediateRepresentationElement("Button");
            child.AttachedProperties["DockPanel.Dock"] = "Right";
            element.Children.Add(child);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsRowDirectionWhenElementHasNoChildren()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:row;"));
        }

        [Test]
        public void TestApplyLayoutAppendsColumnDirectionWhenFirstChildIsDockTopAndSecondIsDockLeft()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");
            var topChild = new IntermediateRepresentationElement("TextBlock");
            topChild.AttachedProperties["DockPanel.Dock"] = "Top";
            var leftChild = new IntermediateRepresentationElement("Button");
            leftChild.AttachedProperties["DockPanel.Dock"] = "Left";
            element.Children.Add(topChild);
            element.Children.Add(leftChild);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("flex-direction:column;"));
        }

        #endregion
    }
}
