// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class DataTriggerHandlerTest
{
    private DataTriggerHandler v_Handler = null!;

    [SetUp]
    public void SetUp()
    {
        v_Handler = new DataTriggerHandler();
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenDataTriggerPresent()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var trigger = new IntermediateRepresentationDataTrigger
        {
            BindingPath = "IsActive",
            Value = "True"
        };
        trigger.Setters["Background"] = "Green";
        element.DataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#border1", output);

        //Assert — DataTriggers require JS data-binding; graceful degradation emits nothing
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessDoesNotSetRequiresJsRuntimeForDataTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var trigger = new IntermediateRepresentationDataTrigger
        {
            BindingPath = "IsActive",
            Value = "True"
        };
        element.DataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#border1", output);

        //Assert — graceful degradation: DataTriggers are silently dropped, no JS dependency
        Assert.That(output.RequiresJsRuntime, Is.False);
    }

    [Test]
    public void TestProcessProducesEmptyOutputForMultipleDataTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("TextBlock");

        var t1 = new IntermediateRepresentationDataTrigger { BindingPath = "State", Value = "Active" };
        t1.Setters["Foreground"] = "Green";

        var t2 = new IntermediateRepresentationDataTrigger { BindingPath = "State", Value = "Error" };
        t2.Setters["Foreground"] = "Red";

        element.DataTriggers.Add(t1);
        element.DataTriggers.Add(t2);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#txt", output);

        //Assert — graceful degradation: no output even for multiple data triggers
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenNoDataTriggersPresent()
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

    [Test]
    public void TestProcessNeverEmitsCssRulesForDataTriggers()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Border");
        var trigger = new IntermediateRepresentationDataTrigger
        {
            BindingPath = "IsVisible",
            Value = "False"
        };
        trigger.Setters["Opacity"] = "0";
        element.DataTriggers.Add(trigger);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#brd", output);

        //Assert
        Assert.That(output.CssRules, Is.Empty);
    }
}
