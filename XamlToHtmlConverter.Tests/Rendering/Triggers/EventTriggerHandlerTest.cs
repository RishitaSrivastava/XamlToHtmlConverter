// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class EventTriggerHandlerTest
{
    private EventTriggerHandler v_Handler = null!;

    [SetUp]
    public void SetUp()
    {
        v_Handler = new EventTriggerHandler();
    }

    [Test]
    public void TestProcessProducesEmptyOutputForButtonClickEvent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "Button.Click"
        };
        trigger.Actions.Add(new IntermediateRepresentationTriggerAction
        {
            ActionType = "BeginStoryboard"
        });
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — EventTriggers wire WPF animations; no CSS equivalent; graceful degradation emits nothing
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputForMouseEnterEvent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "Mouse.MouseEnter"
        };
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#brd", output);

        //Assert — graceful degradation: no data attributes emitted
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputForGotFocusEvent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("TextBox");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "UIElement.GotFocus"
        };
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#tb", output);

        //Assert — graceful degradation: no data attributes emitted
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputForUnknownEvent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "MyControl.CustomEvent"
        };
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — graceful degradation: unknown events produce no output
        Assert.That(output.DataAttributes, Is.Empty);
    }

    [Test]
    public void TestProcessDoesNotSetRequiresJsRuntimeForEventTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "Button.Click"
        };
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — graceful degradation: no JS dependency flag
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessProducesEmptyOutputRegardlessOfActionType()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "Button.Click"
        };
        trigger.Actions.Add(new IntermediateRepresentationTriggerAction
        {
            ActionType = "BeginStoryboard"
        });
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — action type is irrelevant; graceful degradation emits nothing
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessNeverEmitsCssRulesForEventTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var trigger = new IntermediateRepresentationEventTrigger
        {
            RoutedEvent = "Button.Click"
        };
        element.EventTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenNoEventTriggersPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.RequiresJsRuntime, Is.False);
    }
}
