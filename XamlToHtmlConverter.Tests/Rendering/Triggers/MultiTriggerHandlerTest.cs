// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using NUnit.Framework;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Tests.Rendering.Triggers;

[TestFixture]
public class MultiTriggerHandlerTest
{
    private MultiTriggerHandler v_Handler = null!;

    [SetUp]
    public void SetUp()
    {
        v_Handler = new MultiTriggerHandler();
    }

    [Test]
    public void TestProcessDropsTriggerSilentlyWhenOneConditionIsNotCssMappable()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var multi = new IntermediateRepresentationMultiTrigger();
        multi.Conditions.Add(("IsMouseOver", "True"));
        multi.Conditions.Add(("IsEnabled", "True"));  // IsEnabled=True has no CSS pseudo-class
        multi.Setters["Background"] = "Gold";
        element.MultiTriggers.Add(multi);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — partial CSS-mapping means the whole trigger is dropped silently
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessEmitsCssRuleWhenBothConditionsMapToKnownPseudoClasses()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var multi = new IntermediateRepresentationMultiTrigger();
        multi.Conditions.Add(("IsMouseOver", "True"));
        multi.Conditions.Add(("IsPressed", "True"));
        multi.Setters["Background"] = "Orange";
        element.MultiTriggers.Add(multi);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert
        Assert.That(output.CssRules, Has.Count.EqualTo(1));
        Assert.That(output.CssRules[0], Does.Contain(":hover:active"));
    }

    [Test]
    public void TestProcessDropsAllTriggersWhenConditionsAreNotCssMappable()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");

        // First multi-trigger (dropped: IsEnabled=True is not CSS-mappable)
        var m1 = new IntermediateRepresentationMultiTrigger();
        m1.Conditions.Add(("IsMouseOver", "True"));
        m1.Conditions.Add(("IsEnabled", "True"));
        m1.Setters["Background"] = "Red";

        // Second multi-trigger (also dropped)
        var m2 = new IntermediateRepresentationMultiTrigger();
        m2.Conditions.Add(("IsMouseOver", "True"));
        m2.Conditions.Add(("IsEnabled", "True"));
        m2.Setters["Foreground"] = "White";

        element.MultiTriggers.Add(m1);
        element.MultiTriggers.Add(m2);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — all non-CSS triggers silently dropped; no data attributes emitted
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessDropsTriggerWithoutOutputWhenNotAllConditionsMapToCss()
    {
        //Setup
        var element = new IntermediateRepresentationElement("Button");
        var multi = new IntermediateRepresentationMultiTrigger();
        multi.Conditions.Add(("IsMouseOver", "True"));
        multi.Conditions.Add(("IsEnabled", "True"));  // IsEnabled=True has no CSS pseudo-class
        multi.Setters["Background"] = "Blue";
        element.MultiTriggers.Add(multi);

        var output = new TriggerOutput();

        //Act
        v_Handler.Process(element, "#btn", output);

        //Assert — graceful degradation: trigger silently dropped, no data attributes
        Assert.That(output.DataAttributes, Is.Empty);
        Assert.That(output.CssRules, Is.Empty);
    }

    [Test]
    public void TestProcessProducesEmptyOutputWhenNoMultiTriggersPresent()
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
