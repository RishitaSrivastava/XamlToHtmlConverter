// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class PropertyTriggerHandlerTest
{
    private PropertyTriggerHandler v_Handler = null!;

    [SetUp]
    public void SetUp()
    {
        v_Handler = new PropertyTriggerHandler();
    }

    [Test]
    public void TestProcessEmitsCssRuleWhenIsMouseOverTriggerPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties["Name"] = "myBtn";
        var trigger = new IntermediateRepresentationTrigger
        {
            Property = "IsMouseOver",
            Value = "True"
        };
        trigger.Setters["Background"] = "Blue";
        element.Triggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#myBtn", output);

        //Assert
        Assert.That(output.CssRules, Has.Count.EqualTo(1));
        Assert.That(output.CssRules[0], Does.Contain("#myBtn:hover"));
        Assert.That(output.CssRules[0], Does.Contain("background-color:#2196f3"));
    }

    [Test]
    public void TestProcessEmitsCssRuleWhenIsFocusedTriggerPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("TextBox");
        var trigger = new IntermediateRepresentationTrigger
        {
            Property = "IsFocused",
            Value = "True"
        };
        trigger.Setters["BorderBrush"] = "Blue";
        element.Triggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#myText", output);

        //Assert
        Assert.That(output.CssRules, Has.Count.EqualTo(1));
        Assert.That(output.CssRules[0], Does.Contain(":focus"));
        Assert.That(output.CssRules[0], Does.Contain("border-color"));
    }

    [Test]
    public void TestProcessEmitsCssRuleWhenIsEnabledFalseTriggerPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationTrigger
        {
            Property = "IsEnabled",
            Value = "False"
        };
        trigger.Setters["Foreground"] = "Gray";
        element.Triggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.CssRules[0], Does.Contain(":disabled"));
    }

    [Test]
    public void TestProcessDropsTriggerSilentlyWhenPropertyHasNoCssMapping()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationTrigger
        {
            Property = "IsActive",
            Value = "True"
        };
        trigger.Setters["Background"] = "Red";
        element.Triggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — graceful degradation: no CSS equivalent exists, trigger is silently dropped
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessEmitsMultipleCssRulesWhenMultipleTriggersPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");

        var t1 = new IntermediateRepresentationTrigger { Property = "IsMouseOver", Value = "True" };
        t1.Setters["Background"] = "Blue";

        var t2 = new IntermediateRepresentationTrigger { Property = "IsPressed", Value = "True" };
        t2.Setters["Background"] = "Navy";

        element.Triggers.Add(t1);
        element.Triggers.Add(t2);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.CssRules, Has.Count.EqualTo(2));
        Assert.That(output.CssRules[0], Does.Contain(":hover"));
        Assert.That(output.CssRules[1], Does.Contain(":active"));
    }

    [Test]
    public void TestProcessDoesNotSetRequiresJsRuntimeForCssMappableTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationTrigger { Property = "IsMouseOver", Value = "True" };
        trigger.Setters["Background"] = "Blue";
        element.Triggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenNoTriggersPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.DataAttributes, Is.Empty);
    }
}
