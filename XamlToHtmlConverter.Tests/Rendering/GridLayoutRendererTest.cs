// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class GridLayoutRendererTest
    {
        private GridLayoutRenderer v_Renderer = null!;

        [SetUp]
        public void SetUp()
        {
            v_Renderer = new GridLayoutRenderer();
        }

        #region Tests for CanHandle

        [Test]
        public void TestCanHandleReturnsTrueWhenElementTypeIsGrid()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");

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
        public void TestCanHandleReturnsFalseWhenElementTypeIsDockPanel()
        {
            //Setup
            var element = new IntermediateRepresentationElement("DockPanel");

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
        public void TestApplyLayoutAppendsDisplayGridWhenCalled()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("display:grid;"));
        }

        [Test]
        public void TestApplyLayoutUsesExplicitRowDefinitionsWhenProvided()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.GridRowDefinitions.Add("Auto");
            element.GridRowDefinitions.Add("*");
            element.GridRowDefinitions.Add("Auto");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("grid-template-rows:auto minmax(0,1fr) auto;"));
        }

        [Test]
        public void TestApplyLayoutUsesExplicitColumnDefinitionsWhenProvided()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.GridColumnDefinitions.Add("2*");
            element.GridColumnDefinitions.Add("3*");
            element.GridColumnDefinitions.Add("1*");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("grid-template-columns:"));
        }

        [Test]
        public void TestApplyLayoutInfersRowCountFromChildAttachedPropertiesWhenNoExplicitDefinitionsExist()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var child0 = new IntermediateRepresentationElement("Border");
            child0.AttachedProperties["Grid.Row"] = "0";
            var child1 = new IntermediateRepresentationElement("Border");
            child1.AttachedProperties["Grid.Row"] = "2";
            element.Children.Add(child0);
            element.Children.Add(child1);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            // Row 0, 1, 2 inferred → 3 auto rows
            Assert.That(sb.ToString(), Does.Contain("grid-template-rows:auto auto auto;"));
        }

        [Test]
        public void TestApplyLayoutInfersColumnCountFromChildAttachedPropertiesWhenNoExplicitDefinitionsExist()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var child0 = new IntermediateRepresentationElement("Border");
            child0.AttachedProperties["Grid.Column"] = "0";
            var child1 = new IntermediateRepresentationElement("Border");
            child1.AttachedProperties["Grid.Column"] = "1";
            element.Children.Add(child0);
            element.Children.Add(child1);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("grid-template-columns:auto auto;"));
        }

        [Test]
        public void TestApplyLayoutConvertsStarToFrUnitWhenGridLengthIsStarValue()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.GridRowDefinitions.Add("*");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("1fr"));
        }

        [Test]
        public void TestApplyLayoutConvertsAutoToAutoWhenGridLengthIsAutoValue()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.GridRowDefinitions.Add("Auto");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("auto"));
        }

        [Test]
        public void TestApplyLayoutConvertsPixelValueToPixelUnitWhenGridLengthIsFixed()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            element.GridRowDefinitions.Add("200");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Contain("200px"));
        }

        [Test]
        public void TestApplyLayoutDoesNotAppendRowTemplateWhenGridHasNoChildrenAndNoDefinitions()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            Assert.That(sb.ToString(), Does.Not.Contain("grid-template-rows"));
        }

        [Test]
        public void TestApplyLayoutAccountsForRowSpanWhenInferringRowCount()
        {
            //Setup
            var element = new IntermediateRepresentationElement("Grid");
            var child = new IntermediateRepresentationElement("Border");
            child.AttachedProperties["Grid.Row"] = "1";
            child.AttachedProperties["Grid.RowSpan"] = "2";
            element.Children.Add(child);
            var sb = new StringBuilder();

            //Act
            v_Renderer.ApplyLayout(element, sb);

            //Assert
            // Row 1 + span 2 - 1 = last row index 2 → 3 auto rows
            Assert.That(sb.ToString(), Does.Contain("grid-template-rows:auto auto auto;"));
        }

        #endregion
    }
}
