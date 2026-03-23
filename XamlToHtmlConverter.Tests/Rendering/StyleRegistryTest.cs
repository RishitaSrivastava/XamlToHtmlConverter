// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    [TestFixture]
    public class StyleRegistryTest
    {
        private StyleRegistry v_Registry = null!;

        [SetUp]
        public void SetUp()
        {
            v_Registry = new StyleRegistry();
        }

        #region Tests for Register

        [Test]
        public void TestRegisterReturnsGeneratedClassNameWhenNewStyleIsProvided()
        {
            //Act
            var result = v_Registry.Register("color:red;");

            //Assert
            Assert.That(result, Is.EqualTo("c1"));
        }

        [Test]
        public void TestRegisterReturnsSameClassNameWhenIdenticalStyleIsRegisteredTwice()
        {
            //Setup
            const string c_Style = "display:flex;flex-direction:column;";

            //Act
            var first = v_Registry.Register(c_Style);
            var second = v_Registry.Register(c_Style);

            //Assert
            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void TestRegisterIncrementsClassNameCounterWhenDifferentStylesAreRegistered()
        {
            //Act
            var first = v_Registry.Register("color:red;");
            var second = v_Registry.Register("color:blue;");
            var third = v_Registry.Register("color:green;");

            //Assert
            Assert.That(first, Is.EqualTo("c1"));
            Assert.That(second, Is.EqualTo("c2"));
            Assert.That(third, Is.EqualTo("c3"));
        }

        [Test]
        public void TestRegisterReturnsEmptyStringWhenNullStyleIsProvided()
        {
            //Act
            var result = v_Registry.Register(null);

            //Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestRegisterReturnsEmptyStringWhenEmptyStringStyleIsProvided()
        {
            //Act
            var result = v_Registry.Register(string.Empty);

            //Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestRegisterReturnsEmptyStringWhenWhitespaceOnlyStyleIsProvided()
        {
            //Act
            var result = v_Registry.Register("   ");

            //Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestRegisterDoesDuplicateStyleEntryWhenSameStyleRegisteredAfterDifferentStyles()
        {
            //Setup
            const string c_SharedStyle = "font-weight:bold;";
            v_Registry.Register("color:red;");
            v_Registry.Register(c_SharedStyle);

            //Act - register the same style again
            var result = v_Registry.Register(c_SharedStyle);

            //Assert - should return c2, not c3
            Assert.That(result, Is.EqualTo("c2"));
        }

        #endregion

        #region Tests for GenerateStyleBlock

        [Test]
        public void TestGenerateStyleBlockReturnsStyleTagWhenNoStylesAreRegistered()
        {
            //Act
            var result = v_Registry.GenerateStyleBlock();

            //Assert
            Assert.That(result, Does.Contain("<style>"));
            Assert.That(result, Does.Contain("</style>"));
        }

        [Test]
        public void TestGenerateStyleBlockContainsRegisteredClassRuleWhenOneStyleIsRegistered()
        {
            //Setup
            v_Registry.Register("color:red;");

            //Act
            var result = v_Registry.GenerateStyleBlock();

            //Assert
            Assert.That(result, Does.Contain(".c1 { color:red; }"));
        }

        [Test]
        public void TestGenerateStyleBlockContainsAllRegisteredClassRulesWhenMultipleStylesAreRegistered()
        {
            //Setup
            v_Registry.Register("color:red;");
            v_Registry.Register("color:blue;");

            //Act
            var result = v_Registry.GenerateStyleBlock();

            //Assert
            Assert.That(result, Does.Contain(".c1 { color:red; }"));
            Assert.That(result, Does.Contain(".c2 { color:blue; }"));
        }

        [Test]
        public void TestGenerateStyleBlockDoesNotDuplicateClassWhenSameStyleIsRegisteredTwice()
        {
            //Setup
            v_Registry.Register("color:red;");
            v_Registry.Register("color:red;");

            //Act
            var result = v_Registry.GenerateStyleBlock();

            //Assert
            Assert.That(result.Split(".c1").Length - 1, Is.EqualTo(1),
                "Style block should contain .c1 exactly once.");
        }

        #endregion
    }
}
