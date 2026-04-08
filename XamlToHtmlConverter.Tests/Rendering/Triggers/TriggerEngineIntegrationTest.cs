// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class TriggerEngineIntegrationTest
{
    [Test]
    public void TestProcessAllEmitsCssRuleForCssMappableTrigger()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties["Name"] = "saveBtn";
        var trigger = new IntermediateRepresentationTrigger
        {
            Property = "IsMouseOver",
            Value = "True"
        };
        trigger.Setters["Background"] = "Blue";
        element.Triggers.Add(trigger);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert
        Assert.That(output.CssRules, Has.Count.EqualTo(1));
        Assert.That(output.CssRules[0], Does.Contain("#saveBtn:hover"));
        Assert.That(output.DataAttributes, Does.Not.ContainKey("data-trigger-ismouseover"));
    }

    [Test]
    public void TestProcessAllDropsNonCssMappableTriggerSilently()
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

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert — graceful degradation: non-CSS trigger silently dropped, no data attributes
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessAllProducesEmptyOutputForDataTrigger()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var dt = new IntermediateRepresentationDataTrigger
        {
            BindingPath = "IsVisible",
            Value = "False"
        };
        dt.Setters["Opacity"] = "0";
        element.DataTriggers.Add(dt);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert — DataTriggers require JS; graceful degradation emits nothing
        Assert.That(output.RequiresJsRuntime, Is.False);
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessAllProducesEmptyOutputForEventTrigger()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var et = new IntermediateRepresentationEventTrigger { RoutedEvent = "Button.Click" };
        element.EventTriggers.Add(et);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert — EventTriggers require JS; graceful degradation emits nothing
        Assert.That(output.RequiresJsRuntime, Is.False);
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessAllCombinesAllFiveTriggerTypesInSingleOutput()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties["Name"] = "complexBtn";

        // Trigger (CSS-mappable: IsMouseOver → :hover)
        var t = new IntermediateRepresentationTrigger { Property = "IsMouseOver", Value = "True" };
        t.Setters["Background"] = "Blue";
        element.Triggers.Add(t);

        // MultiTrigger (both CSS-mappable: IsMouseOver+IsPressed → :hover:active)
        var mt = new IntermediateRepresentationMultiTrigger();
        mt.Conditions.Add(("IsMouseOver", "True"));
        mt.Conditions.Add(("IsPressed", "True"));
        mt.Setters["Background"] = "DarkBlue";
        element.MultiTriggers.Add(mt);

        // DataTrigger (no CSS equivalent — silently dropped)
        var dt = new IntermediateRepresentationDataTrigger { BindingPath = "IsEnabled", Value = "False" };
        dt.Setters["Opacity"] = "0.5";
        element.DataTriggers.Add(dt);

        // MultiDataTrigger (no CSS equivalent — silently dropped)
        var mdt = new IntermediateRepresentationMultiDataTrigger();
        mdt.Conditions.Add(("A", "1"));
        mdt.Conditions.Add(("B", "2"));
        mdt.Setters["Visibility"] = "Hidden";
        element.MultiDataTriggers.Add(mdt);

        // EventTrigger (no CSS equivalent — silently dropped)
        var et = new IntermediateRepresentationEventTrigger { RoutedEvent = "Button.Click" };
        et.Actions.Add(new IntermediateRepresentationTriggerAction { ActionType = "BeginStoryboard" });
        element.EventTriggers.Add(et);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert — only CSS-mappable triggers produce output; all others silently dropped
        Assert.That(output.CssRules, Has.Count.GreaterThanOrEqualTo(2));  // Trigger + MultiTrigger CSS
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessAllGeneratesDataXtIdAttributeWhenElementHasNoName()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationTrigger { Property = "IsMouseOver", Value = "True" };
        trigger.Setters["Background"] = "Blue";
        element.Triggers.Add(trigger);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert — unnamed element gets data-xt-id and selector-based CSS rule
        Assert.That(output.DataAttributes, Contains.Key("data-xt-id"));
        Assert.That(output.CssRules[0], Does.Contain("data-xt-id"));
    }

    [Test]
    public void TestProcessAllUsesNamedSelectorWhenElementHasNameProperty()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        element.Properties["Name"] = "okButton";
        var trigger = new IntermediateRepresentationTrigger { Property = "IsMouseOver", Value = "True" };
        trigger.Setters["Foreground"] = "White";
        element.Triggers.Add(trigger);

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert
        Assert.That(output.CssRules[0], Does.StartWith("#okButton"));
        Assert.That(output.DataAttributes, Does.Not.ContainKey("data-xt-id"));
    }

    [Test]
    public void TestProcessAllReturnsEmptyOutputForElementWithNoTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("TextBlock");

        //Act
        var output = TriggerEngine.ProcessAll(element);

        //Assert
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestGenerateJsRuntimeReturnsNonEmptyScriptBlock()
    {
        //Act
        var script = TriggerEngine.GenerateJsRuntime();

        //Assert
        Assert.That(script, Does.Contain("<script>"));
        Assert.That(script, Does.Contain("</script>"));
        Assert.That(script, Does.Contain("xamlSetContext"));
    }
}
