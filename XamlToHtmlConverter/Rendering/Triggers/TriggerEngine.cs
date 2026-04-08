// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Runtime.CompilerServices;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Central entry-point for all XAML trigger processing.
/// Orchestrates five specialised <see cref="ITriggerHandler"/> implementations that cover
/// every major XAML trigger type:
/// <list type="bullet">
///   <item><description><b>Trigger</b> — single property/value condition → CSS pseudo-class or data attribute</description></item>
///   <item><description><b>MultiTrigger</b> — multiple property/value conditions → combined CSS selector or data attribute</description></item>
///   <item><description><b>DataTrigger</b> — binding-based condition → indexed data-datatrigger-* attribute + JS runtime</description></item>
///   <item><description><b>MultiDataTrigger</b> — multiple binding conditions → indexed data-multidatatrigger-* attribute + JS runtime</description></item>
///   <item><description><b>EventTrigger</b> — routed event → data-event-trigger-{event} attribute + JS runtime</description></item>
/// </list>
/// </summary>
public static class TriggerEngine
{
    #region Private Data

    private static readonly IReadOnlyList<ITriggerHandler> s_Handlers =
    [
        new PropertyTriggerHandler(),
        new MultiTriggerHandler(),
        new DataTriggerHandler(),
        new MultiDataTriggerHandler(),
        new EventTriggerHandler(),
    ];

    #endregion

    #region Public Methods — Primary API

    /// <summary>
    /// Processes all trigger types on the given element and returns a
    /// <see cref="TriggerOutput"/> containing CSS rules, data attributes,
    /// and a flag indicating whether the inline JS runtime is required.
    /// </summary>
    /// <param name="element">The IR element to process.</param>
    /// <returns>
    /// A <see cref="TriggerOutput"/> with:
    /// <list type="bullet">
    ///   <item><see cref="TriggerOutput.CssRules"/> — full CSS rule strings for CSS-mappable triggers.</item>
    ///   <item><see cref="TriggerOutput.DataAttributes"/> — data-* attributes for runtime-evaluated triggers.</item>
    ///   <item><see cref="TriggerOutput.RequiresJsRuntime"/> — true when the JS runtime block must be emitted.</item>
    /// </list>
    /// </returns>
    public static TriggerOutput ProcessAll(IntermediateRepresentationElement element)
    {
        var output = new TriggerOutput();
        var (selector, generatedId) = ResolveElementSelector(element);

        foreach (var handler in s_Handlers)
            handler.Process(element, selector, output);

        // Only emit the generated data-xt-id attribute when CSS rules were actually produced
        // using this selector. If there are no CSS rules, no selector is needed in the DOM.
        if (generatedId != null && output.CssRules.Count > 0)
            output.DataAttributes["data-xt-id"] = generatedId;

        return output;
    }

    /// <summary>
    /// Generates the compact inline JavaScript runtime block that reads
    /// <c>data-datatrigger-*</c>, <c>data-multidatatrigger-*</c>, and
    /// <c>data-event-trigger-*</c> attributes and applies their effects at runtime.
    /// </summary>
    /// <returns>
    /// A self-contained <c>&lt;script&gt;</c> block that should be emitted once per HTML document
    /// when any trigger with <see cref="TriggerOutput.RequiresJsRuntime"/> set to true is present.
    /// </returns>
    public static string GenerateJsRuntime()
    {
        return """
            <script>
            (function(){
              "use strict";
              // --- XAML Trigger Runtime ---

              // Helper: apply WPF-style setter string "Prop1:Val1;Prop2:Val2" as CSS.
              function applySetters(el, setterStr) {
                if (!setterStr) return;
                setterStr.split(';').forEach(function(pair) {
                  var idx = pair.indexOf(':');
                  if (idx < 0) return;
                  var prop = pair.substring(0, idx).trim();
                  var val  = pair.substring(idx + 1).trim();
                  // Map common WPF property names to CSS.
                  var cssMap = {
                    'Background':'background-color','Foreground':'color',
                    'BorderBrush':'border-color','BorderThickness':'border-width',
                    'FontWeight':'font-weight','FontSize':'font-size',
                    'Opacity':'opacity','Visibility':'visibility','Cursor':'cursor',
                    'Width':'width','Height':'height'
                  };
                  el.style[cssMap[prop] || prop.toLowerCase()] = val;
                });
              }

              // Helper: restore WPF-style setters (set to empty to revert).
              function clearSetters(el, setterStr) {
                if (!setterStr) return;
                setterStr.split(';').forEach(function(pair) {
                  var idx = pair.indexOf(':');
                  if (idx < 0) return;
                  var prop = pair.substring(0, idx).trim();
                  var cssMap = {
                    'Background':'background-color','Foreground':'color',
                    'BorderBrush':'border-color','BorderThickness':'border-width',
                    'FontWeight':'font-weight','FontSize':'font-size',
                    'Opacity':'opacity','Visibility':'visibility','Cursor':'cursor',
                    'Width':'width','Height':'height'
                  };
                  el.style[cssMap[prop] || prop.toLowerCase()] = '';
                });
              }

              // --- Property triggers (non-CSS-mappable fallback) ---
              document.querySelectorAll('[data-trigger-isenabled]').forEach(function(el) {
                var raw = el.getAttribute('data-trigger-isenabled');
                if (!raw) return;
                var colon = raw.indexOf(':');
                var targetVal = raw.substring(0, colon);
                var setterStr = raw.substring(colon + 1);
                if (targetVal === 'False' && el.disabled) applySetters(el, setterStr);
              });

              // --- DataTrigger evaluation (runs when data-context changes) ---
              function evalDataTriggers(el) {
                var i = 0;
                while (true) {
                  var raw = el.getAttribute('data-datatrigger-' + i);
                  if (raw === null) break;
                  var colon = raw.indexOf(':');
                  var condPart = raw.substring(0, colon);
                  var setterStr = raw.substring(colon + 1);
                  var eqIdx = condPart.indexOf('=');
                  var path = condPart.substring(0, eqIdx);
                  var expected = condPart.substring(eqIdx + 1);
                  var ctx = el.__xamlContext || {};
                  var actual = String(ctx[path] !== undefined ? ctx[path] : '');
                  if (actual === expected)
                    applySetters(el, setterStr);
                  else
                    clearSetters(el, setterStr);
                  i++;
                }
              }

              // --- MultiDataTrigger evaluation ---
              function evalMultiDataTriggers(el) {
                var i = 0;
                while (true) {
                  var raw = el.getAttribute('data-multidatatrigger-' + i);
                  if (raw === null) break;
                  var colon = raw.indexOf(':');
                  var condPart = raw.substring(0, colon);
                  var setterStr = raw.substring(colon + 1);
                  var ctx = el.__xamlContext || {};
                  var allMatch = condPart.split('&').every(function(cond) {
                    var idx = cond.indexOf('=');
                    var path = cond.substring(0, idx);
                    var expected = cond.substring(idx + 1);
                    return String(ctx[path] !== undefined ? ctx[path] : '') === expected;
                  });
                  if (allMatch) applySetters(el, setterStr);
                  else clearSetters(el, setterStr);
                  i++;
                }
              }

              // --- EventTrigger binding ---
              document.querySelectorAll('[class]').forEach(function(el) {
                Array.prototype.forEach.call(el.attributes, function(attr) {
                  if (!attr.name.startsWith('data-event-trigger-')) return;
                  var htmlEvent = attr.name.replace('data-event-trigger-', '');
                  var actionStr = attr.value;
                  el.addEventListener(htmlEvent, function() {
                    // Parse "ActionType:Key1=Val1;Key2=Val2" sections separated by '|'.
                    actionStr.split('|').forEach(function(actionDef) {
                      var colonIdx = actionDef.indexOf(':');
                      var actionType = colonIdx < 0 ? actionDef : actionDef.substring(0, colonIdx);
                      // BeginStoryboard — apply inline styles from props.
                      if (actionType === 'BeginStoryboard' && colonIdx >= 0) {
                        var props = {};
                        actionDef.substring(colonIdx + 1).split(';').forEach(function(p) {
                          var eq = p.indexOf('=');
                          if (eq >= 0) props[p.substring(0, eq)] = p.substring(eq + 1);
                        });
                        applySetters(el, actionDef.substring(colonIdx + 1));
                      }
                    });
                  });
                });
              });

              // Expose context setter so host frameworks can push data.
              window.xamlSetContext = function(el, ctx) {
                el.__xamlContext = ctx;
                evalDataTriggers(el);
                evalMultiDataTriggers(el);
              };
            })();
            </script>
            """;
    }

    #endregion

    #region Public Methods — Backward-compatible shims

    /// <summary>
    /// [Compatibility shim] Extracts single-condition trigger data attributes.
    /// Delegates to <see cref="ProcessAll"/> and returns the data attributes subset.
    /// </summary>
    public static Dictionary<string, string> Extract(
        IntermediateRepresentationElement element)
        => ProcessAll(element).DataAttributes;

    /// <summary>
    /// [Compatibility shim] Extracts multi-condition trigger data attributes.
    /// Delegates to <see cref="ProcessAll"/> and returns only multi-trigger entries.
    /// </summary>
    public static Dictionary<string, string> ExtractMultiTriggers(
        IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var kv in ProcessAll(element).DataAttributes)
        {
            if (kv.Key.StartsWith("data-multitrigger", StringComparison.Ordinal))
                result[kv.Key] = kv.Value;
        }

        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Resolves the CSS selector that identifies this element in the rendered HTML.
    /// Priority: 1) x:Name attribute  2) Name attribute  3) generated data-xt-id value.
    /// Returns the selector string and, for unnamed elements, the generated ID value
    /// (null when a named selector was resolved so no attribute needs to be injected).
    ///
    /// <para>Special case: <c>ComboBoxItem</c> and <c>ListBoxItem</c> render as
    /// <c>&lt;option&gt;</c> elements whose selection state is controlled by the browser.
    /// Their <c>IsSelected</c> trigger maps to <c>option:checked</c>.
    /// If the parent ComboBox/ListBox has an id, the rule is scoped to that
    /// parent selector (e.g., <c>#statusCombo option</c>); otherwise a global
    /// <c>option</c> selector is used as graceful degradation.</para>
    /// </summary>
    private static (string Selector, string? GeneratedId) ResolveElementSelector(
        IntermediateRepresentationElement element)
    {
        // ComboBoxItem / ListBoxItem render as <option>; selection is browser-native.
        // Use "option" as the element selector so IsSelected=True yields "option:checked".
        // Scope to the parent <select> when the parent has a known id.
        if (element.Type is "ComboBoxItem" or "ListBoxItem")
        {
            var parentSelector = ResolveParentSelectSelector(element.Parent);
            return (parentSelector is not null ? $"{parentSelector} option" : "option", null);
        }

        if (element.Properties.TryGetValue("x:Name", out var xName)
            && !string.IsNullOrWhiteSpace(xName))
            return ($"#{xName}", null);

        if (element.Properties.TryGetValue("Name", out var name)
            && !string.IsNullOrWhiteSpace(name))
            return ($"#{name}", null);

        // Generate a stable unique ID from type + hash of object identity.
        var id = $"xt-{element.Type.ToLowerInvariant()}-{Math.Abs(RuntimeHelpers.GetHashCode(element))}";
        return ($"[data-xt-id=\"{id}\"]", id);
    }

    /// <summary>
    /// Walks up the parent chain looking for a ComboBox or ListBox with a Name/x:Name,
    /// returning the CSS id selector (e.g., "#statusCombo") or null if not found.
    /// </summary>
    private static string? ResolveParentSelectSelector(IntermediateRepresentationElement? parent)
    {
        while (parent is not null)
        {
            if (parent.Type is "ComboBox" or "ListBox")
            {
                if (parent.Properties.TryGetValue("x:Name", out var xn) && !string.IsNullOrWhiteSpace(xn))
                    return $"#{xn}";
                if (parent.Properties.TryGetValue("Name", out var n) && !string.IsNullOrWhiteSpace(n))
                    return $"#{n}";
                // Parent ComboBox/ListBox found but has no id — fall back to global option selector.
                return null;
            }
            parent = parent.Parent;
        }
        return null;
    }

    #endregion
}
