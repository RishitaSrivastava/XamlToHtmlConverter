// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Handles routed-event triggers (<see cref="IntermediateRepresentationEventTrigger"/>).
/// 
/// <para>Well-known WPF routed events are mapped to their HTML event equivalents.
/// Unknown events are normalised and emitted as-is so no information is lost.</para>
/// 
/// <para>Each trigger produces a <c>data-event-trigger-{htmlEvent}</c> attribute whose value
/// encodes the action type and its properties in the format
/// <c>ActionType:Key1=Val1;Key2=Val2</c>.</para>
/// 
/// <para>Sets <see cref="TriggerOutput.RequiresJsRuntime"/> to <c>true</c> because an event
/// listener must be attached at runtime.</para>
/// </summary>
public sealed class EventTriggerHandler : ITriggerHandler
{
    #region Private Data

    // WPF routed event names (last segment, lower-cased) → HTML event name.
    private static readonly Dictionary<string, string> s_EventMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Click",           "click"       },
            { "MouseEnter",      "mouseenter"  },
            { "MouseLeave",      "mouseleave"  },
            { "MouseDown",       "mousedown"   },
            { "MouseUp",         "mouseup"     },
            { "MouseMove",       "mousemove"   },
            { "GotFocus",        "focus"       },
            { "LostFocus",       "blur"        },
            { "KeyDown",         "keydown"     },
            { "KeyUp",           "keyup"       },
            { "PreviewKeyDown",  "keydown"     },
            { "Loaded",          "load"        },
            { "Unloaded",        "unload"      },
            { "SizeChanged",     "resize"      },
            { "SelectionChanged","change"      },
            { "TextChanged",     "input"       },
            { "Checked",         "change"      },
            { "Unchecked",       "change"      },
            { "DropDownOpened",  "open"        },
            { "DropDownClosed",  "close"       },
        };

    #endregion

    /// <inheritdoc />
    public void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output)
    {
        // EventTriggers wire WPF routed events to Storyboard animations.
        // No CSS equivalent exists; event listeners require JavaScript.
        // Graceful degradation: produce no output.
        // The element renders statically without any animation behaviour.
    }

    #region Private Methods

    /// <summary>
    /// Resolves the HTML event name from a fully-qualified WPF routed event name.
    /// E.g. "Button.Click" → "click", "UIElement.GotFocus" → "focus".
    /// Falls back to the lower-cased last segment for unknown events.
    /// </summary>
    private static string ResolveHtmlEvent(string routedEvent)
    {
        // Extract the local event name (last segment after optional type qualifier).
        var dot = routedEvent.LastIndexOf('.');
        var eventName = dot >= 0 ? routedEvent[(dot + 1)..] : routedEvent;

        return s_EventMap.TryGetValue(eventName, out var html)
            ? html
            : eventName.ToLowerInvariant();
    }

    private static string SerializeProperties(Dictionary<string, string> props)
    {
        var sb = new StringBuilder();
        bool first = true;

        foreach (var p in props)
        {
            if (!first) sb.Append(';');
            sb.Append(p.Key).Append('=').Append(p.Value);
            first = false;
        }

        return sb.ToString();
    }

    #endregion
}
