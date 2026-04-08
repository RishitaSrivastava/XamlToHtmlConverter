// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class MultiDataTriggerHandlerTest
{
    private MultiDataTriggerHandler v_Handler = null!;

    [SetUp]
    public void SetUp()
    {
        v_Handler = new MultiDataTriggerHandler();
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenMultiDataTriggerPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var trigger = new IntermediateRepresentationMultiDataTrigger();
        trigger.Conditions.Add(("IsAdmin", "True"));
        trigger.Conditions.Add(("IsLoggedIn", "True"));
        trigger.Setters["Background"] = "Gold";
        element.MultiDataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#brd", output);

        //Assert — MultiDataTriggers require JS data-binding; graceful degradation emits nothing
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWithMultipleConditions()
    {
        //Setup
        var element = new IntermediateRepresentationElement("StackPanel");
        var trigger = new IntermediateRepresentationMultiDataTrigger();
        trigger.Conditions.Add(("StatusA", "Ready"));
        trigger.Conditions.Add(("StatusB", "Active"));
        element.MultiDataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#sp", output);

        //Assert — graceful degradation: no output regardless of condition count
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessDoesNotSetRequiresJsRuntimeForMultiDataTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Grid");
        var trigger = new IntermediateRepresentationMultiDataTrigger();
        trigger.Conditions.Add(("X", "1"));
        element.MultiDataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#grid", output);

        //Assert — graceful degradation: no JS dependency flag
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessProducesEmptyOutputForMultipleMultiDataTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("StackPanel");

        var t1 = new IntermediateRepresentationMultiDataTrigger();
        t1.Conditions.Add(("A", "1"));

        var t2 = new IntermediateRepresentationMultiDataTrigger();
        t2.Conditions.Add(("B", "2"));

        element.MultiDataTriggers.Add(t1);
        element.MultiDataTriggers.Add(t2);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#sp", output);

        //Assert — graceful degradation: no output for any number of MultiDataTriggers
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenNoMultiDataTriggersPresent()
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
