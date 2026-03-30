// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class TriggerCssPropertyMapperTest
{
    #region TryGetCssPseudoClass

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndHoverWhenIsMouseOverTrue()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsMouseOver", "True", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":hover"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndActiveWhenIsPressedTrue()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsPressed", "True", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":active"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndFocusWhenIsFocusedTrue()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsFocused", "True", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":focus"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndDisabledWhenIsEnabledFalse()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsEnabled", "False", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":disabled"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndCheckedWhenIsCheckedTrue()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsChecked", "True", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":checked"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsTrueAndCheckedWhenIsSelectedTrue()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsSelected", "True", out var pseudo);

        //Assert — option:checked is the standards-compliant way to style selected <option> elements
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":checked"));
    }

    [Test]
    public void TestTryGetCssPseudoClassReturnsFalseForUnknownProperty()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("IsActive", "True", out var pseudo);

        //Assert
        Assert.That(found, Is.False);
        Assert.That(pseudo, Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestTryGetCssPseudoClassIsCaseInsensitive()
    {
        //Act
        var found = TriggerCssPropertyMapper.TryGetCssPseudoClass("ISMOUSEOVER", "TRUE", out var pseudo);

        //Assert
        Assert.That(found, Is.True);
        Assert.That(pseudo, Is.EqualTo(":hover"));
    }

    #endregion

    #region MapSetterToCss

    [Test]
    public void TestMapSetterToCssMapsBackgroundToBackgroundColor()
    {
        //Act
        var (prop, _) = TriggerCssPropertyMapper.MapSetterToCss("Background", "Blue");

        //Assert
        Assert.That(prop, Is.EqualTo("background-color"));
    }

    [Test]
    public void TestMapSetterToCssMapsNamedColorToHexValue()
    {
        //Act
        var (_, val) = TriggerCssPropertyMapper.MapSetterToCss("Background", "Blue");

        //Assert
        Assert.That(val, Is.EqualTo("#2196f3"));
    }

    [Test]
    public void TestMapSetterToCssMapsTransparentToTransparent()
    {
        //Act
        var (_, val) = TriggerCssPropertyMapper.MapSetterToCss("Background", "Transparent");

        //Assert
        Assert.That(val, Is.EqualTo("transparent"));
    }

    [Test]
    public void TestMapSetterToCssMapsForegroundToColor()
    {
        //Act
        var (prop, _) = TriggerCssPropertyMapper.MapSetterToCss("Foreground", "Red");

        //Assert
        Assert.That(prop, Is.EqualTo("color"));
    }

    [Test]
    public void TestMapSetterToCssMapsFontWeightBoldToBold()
    {
        //Act
        var (prop, val) = TriggerCssPropertyMapper.MapSetterToCss("FontWeight", "Bold");

        //Assert
        Assert.That(prop, Is.EqualTo("font-weight"));
        Assert.That(val, Is.EqualTo("bold"));
    }

    [Test]
    public void TestMapSetterToCssAppendsPxToNumericFontSize()
    {
        //Act
        var (prop, val) = TriggerCssPropertyMapper.MapSetterToCss("FontSize", "14");

        //Assert
        Assert.That(prop, Is.EqualTo("font-size"));
        Assert.That(val, Is.EqualTo("14px"));
    }

    [Test]
    public void TestMapSetterToCssReturnsLoweredPropertyNameForUnknownProperty()
    {
        //Act
        var (prop, _) = TriggerCssPropertyMapper.MapSetterToCss("CustomProp", "value");

        //Assert
        Assert.That(prop, Is.EqualTo("customprop"));
    }

    #endregion
}
