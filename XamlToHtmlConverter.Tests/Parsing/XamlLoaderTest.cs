// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.Parsing;

namespace XamlToHtmlConverter.Tests.Parsing
{
    [TestFixture]
    public class XamlLoaderTest
    {
        private XamlLoader v_Loader = null!;
        private string v_TempFilePath = null!;

        [SetUp]
        public void SetUp()
        {
            v_Loader = new XamlLoader();
            v_TempFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "test_input.xaml");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(v_TempFilePath))
                File.Delete(v_TempFilePath);
        }

        #region Tests for Load

        [Test]
        public void TestLoadReturnsXDocumentWithRootElementWhenValidXamlFileIsProvided()
        {
            //Setup
            File.WriteAllText(v_TempFilePath,
                "<Window xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Grid/></Window>");

            //Act
            var doc = v_Loader.Load(v_TempFilePath);

            //Assert
            Assert.That(doc, Is.Not.Null);
            Assert.That(doc.Root, Is.Not.Null);
            Assert.That(doc.Root!.Name.LocalName, Is.EqualTo("Window"));
        }

        [Test]
        public void TestLoadParsesChildElementCorrectlyWhenXamlContainsNestedElements()
        {
            //Setup
            File.WriteAllText(v_TempFilePath,
                "<Window xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Grid><Button Content=\"OK\"/></Grid></Window>");

            //Act
            var doc = v_Loader.Load(v_TempFilePath);

            //Assert
            Assert.That(doc.Root!.Name.LocalName, Is.EqualTo("Window"));
        }

        [Test]
        public void TestLoadThrowsArgumentExceptionWhenPathIsNull()
        {
            //Act & Assert
            Assert.Throws(
                Is.TypeOf<ArgumentException>(),
                () => v_Loader.Load(null!));
        }

        [Test]
        public void TestLoadThrowsArgumentExceptionWhenPathIsEmptyString()
        {
            //Act & Assert
            Assert.Throws(
                Is.TypeOf<ArgumentException>(),
                () => v_Loader.Load(string.Empty));
        }

        [Test]
        public void TestLoadThrowsArgumentExceptionWhenPathIsWhitespaceString()
        {
            //Act & Assert
            Assert.Throws(
                Is.TypeOf<ArgumentException>(),
                () => v_Loader.Load("   "));
        }

        [Test]
        public void TestLoadThrowsFileNotFoundExceptionWhenFileDoesNotExist()
        {
            //Setup
            const string c_NonExistentPath = "C:\\NonExistent\\path\\to\\file.xaml";

            //Act & Assert
            Assert.Throws(
                Is.TypeOf<FileNotFoundException>(),
                () => v_Loader.Load(c_NonExistentPath));
        }

        [Test]
        public void TestLoadParsesAttributesCorrectlyWhenRootElementHasAttributes()
        {
            //Setup
            File.WriteAllText(v_TempFilePath,
                "<Window xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Title=\"Test\" Width=\"800\"/>");

            //Act
            var doc = v_Loader.Load(v_TempFilePath);

            //Assert
            Assert.That(doc.Root!.Attribute("Title")?.Value, Is.EqualTo("Test"));
            Assert.That(doc.Root!.Attribute("Width")?.Value, Is.EqualTo("800"));
        }

        #endregion
    }
}
