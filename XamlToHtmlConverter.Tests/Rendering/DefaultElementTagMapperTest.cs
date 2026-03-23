// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class DefaultElementTagMapperTest
    {
        private DefaultElementTagMapper v_Mapper = null!;

        [SetUp]
        public void SetUp()
        {
            v_Mapper = new DefaultElementTagMapper();
        }

        #region Tests for Map

        [Test]
        public void TestMapReturnsButtonTagWhenButtonTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("Button");

            //Assert
            Assert.That(result, Is.EqualTo("button"));
        }

        [Test]
        public void TestMapReturnsDivTagWhenGridTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("Grid");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsDivTagWhenStackPanelTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("StackPanel");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsSpanTagWhenTextBlockTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("TextBlock");

            //Assert
            Assert.That(result, Is.EqualTo("span"));
        }

        [Test]
        public void TestMapReturnsDivTagWhenBorderTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("Border");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsInputTagWhenCheckBoxTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("CheckBox");

            //Assert
            Assert.That(result, Is.EqualTo("input"));
        }

        [Test]
        public void TestMapReturnsInputTagWhenRadioButtonTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("RadioButton");

            //Assert
            Assert.That(result, Is.EqualTo("input"));
        }

        [Test]
        public void TestMapReturnsImgTagWhenImageTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("Image");

            //Assert
            Assert.That(result, Is.EqualTo("img"));
        }

        [Test]
        public void TestMapReturnsSelectTagWhenComboBoxTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("ComboBox");

            //Assert
            Assert.That(result, Is.EqualTo("select"));
        }

        [Test]
        public void TestMapReturnsSelectTagWhenListBoxTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("ListBox");

            //Assert
            Assert.That(result, Is.EqualTo("select"));
        }

        [Test]
        public void TestMapReturnsOptionTagWhenComboBoxItemTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("ComboBoxItem");

            //Assert
            Assert.That(result, Is.EqualTo("option"));
        }

        [Test]
        public void TestMapReturnsOptionTagWhenListBoxItemTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("ListBoxItem");

            //Assert
            Assert.That(result, Is.EqualTo("option"));
        }

        [Test]
        public void TestMapReturnsDivTagWhenContentControlTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("ContentControl");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsInputTagWhenTextBoxTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("TextBox");

            //Assert
            Assert.That(result, Is.EqualTo("input"));
        }

        [Test]
        public void TestMapReturnsDivTagWhenWrapPanelTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("WrapPanel");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsDivFallbackWhenUnknownTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("UnknownControl");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsDivFallbackWhenEmptyStringTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map(string.Empty);

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        [Test]
        public void TestMapReturnsDivFallbackWhenWindowTypeIsProvided()
        {
            //Act
            var result = v_Mapper.Map("Window");

            //Assert
            Assert.That(result, Is.EqualTo("div"));
        }

        #endregion
    }
}
