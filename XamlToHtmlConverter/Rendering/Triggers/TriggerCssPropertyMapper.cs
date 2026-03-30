// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Provides static lookup tables that map WPF property/value combinations to
/// CSS pseudo-classes and WPF setter properties to CSS property names.
/// Centralises all WPF→CSS vocabulary in one place so handlers never hard-code mappings.
/// </summary>
public static class TriggerCssPropertyMapper
{
    #region Private Data

    // Key: (WPF property name lowered, trigger value lowered) → CSS pseudo-class suffix.
    // Value-sensitive entries (e.g. IsEnabled=False → :disabled) come last so the
    // value-agnostic fallback can be expressed as ("ismouseover", "") etc.
    private static readonly Dictionary<(string Property, string Value), string> s_PseudoClassMap
        = new(PseudoKeyComparer.Instance)
        {
            { ("ismouseover",   "true"),  ":hover"         },
            { ("ispressed",     "true"),  ":active"        },
            { ("isfocused",     "true"),  ":focus"         },
            { ("iskeyboardfocused", "true"), ":focus"      },
            { ("iskeyboardfocuswithin", "true"), ":focus-within" },
            { ("isdefault",     "true"),  ":default"       },
            { ("isenabled",     "false"), ":disabled"      },
            { ("ischecked",     "true"),  ":checked"       },
            { ("isselected",    "true"),  ":checked"       },
        };

    // WPF setter property name (case-insensitive) → CSS property name.
    private static readonly Dictionary<string, string> s_CssPropertyMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Background",       "background-color"  },
            { "Foreground",       "color"             },
            { "BorderBrush",      "border-color"      },
            { "BorderThickness",  "border-width"      },
            { "FontWeight",       "font-weight"       },
            { "FontSize",         "font-size"         },
            { "FontStyle",        "font-style"        },
            { "FontFamily",       "font-family"       },
            { "Opacity",          "opacity"           },
            { "Visibility",       "visibility"        },
            { "Width",            "width"             },
            { "Height",           "height"            },
            { "Cursor",           "cursor"            },
            { "TextDecorations",  "text-decoration"   },
            { "CornerRadius",     "border-radius"     },
            { "Padding",          "padding"           },
            { "Margin",           "margin"            },
        };

    // WPF colour values to CSS (handles named WPF brushes).
    private static readonly Dictionary<string, string> s_ColorValueMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Transparent",  "transparent"  },
            { "White",        "#ffffff"       },
            { "Black",        "#000000"       },
            { "Red",          "#f44336"       },
            { "Green",        "#4caf50"       },
            { "Blue",         "#2196f3"       },
            { "Yellow",       "#ffeb3b"       },
            { "Orange",       "#ff9800"       },
            { "Gray",         "#9e9e9e"       },
            { "Grey",         "#9e9e9e"       },
            { "LightGray",    "#d3d3d3"       },
            { "DarkGray",     "#a9a9a9"       },
            { "Gold",         "#ffd700"       },
            { "Navy",         "#003087"       },
            { "Teal",         "#009688"       },
            { "Purple",       "#9c27b0"       },
            { "Pink",         "#e91e63"       },
        };

    // WPF font-weight values → CSS equivalents.
    private static readonly Dictionary<string, string> s_FontWeightMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Bold",      "bold"    },
            { "Normal",    "normal"  },
            { "Light",     "300"     },
            { "SemiBold",  "600"     },
            { "ExtraBold", "800"     },
            { "Black",     "900"     },
            { "Thin",      "100"     },
        };

    // WPF cursor values → CSS cursor equivalents.
    private static readonly Dictionary<string, string> s_CursorMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Arrow",       "default"     },
            { "Hand",        "pointer"     },
            { "Wait",        "wait"        },
            { "IBeam",       "text"        },
            { "Cross",       "crosshair"   },
            { "SizeAll",     "move"        },
            { "No",          "not-allowed" },
            { "None",        "none"        },
        };

    // WPF visibility values → CSS visibility equivalents.
    private static readonly Dictionary<string, string> s_VisibilityMap
        = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Visible",   "visible"  },
            { "Hidden",    "hidden"   },
            { "Collapsed", "hidden"   },   // closest CSS equivalent
        };

    #endregion

    #region Public Methods

    /// <summary>
    /// Attempts to resolve the CSS pseudo-class (or attribute selector) corresponding
    /// to a WPF property trigger condition.
    /// </summary>
    /// <param name="wpfProperty">The WPF dependency property name (e.g., "IsMouseOver").</param>
    /// <param name="triggerValue">The value that activates the trigger (e.g., "True").</param>
    /// <param name="pseudoClass">
    /// When this method returns true, contains the CSS pseudo-class/selector suffix
    /// (e.g., ":hover", ":disabled", "[aria-selected=\"true\"]").
    /// </param>
    /// <returns>
    /// <c>true</c> if a CSS mapping exists for this property/value combination;
    /// otherwise <c>false</c>, meaning a data attribute fallback should be used.
    /// </returns>
    public static bool TryGetCssPseudoClass(
        string wpfProperty,
        string triggerValue,
        out string pseudoClass)
    {
        var key = (wpfProperty.ToLowerInvariant(), triggerValue.ToLowerInvariant());

        if (s_PseudoClassMap.TryGetValue(key, out var found))
        {
            pseudoClass = found;
            return true;
        }

        pseudoClass = string.Empty;
        return false;
    }

    /// <summary>
    /// Maps a WPF setter property name and value to a CSS property declaration.
    /// Returns an adapted CSS property name and an adapted CSS value.
    /// Falls back to lower-casing the property name when no mapping is defined.
    /// </summary>
    /// <param name="wpfProperty">The WPF property name (e.g., "Background").</param>
    /// <param name="wpfValue">The WPF property value (e.g., "Blue").</param>
    /// <returns>
    /// A tuple with the CSS property name and the CSS value, both ready to write
    /// into a CSS rule (e.g., ("background-color", "#2196f3")).
    /// </returns>
    public static (string CssProperty, string CssValue) MapSetterToCss(
        string wpfProperty,
        string wpfValue)
    {
        var cssProperty = s_CssPropertyMap.TryGetValue(wpfProperty, out var mapped)
            ? mapped
            : wpfProperty.ToLowerInvariant();

        var cssValue = AdaptCssValue(cssProperty, wpfValue);

        return (cssProperty, cssValue);
    }

    #endregion

    #region Private Methods

    private static string AdaptCssValue(string cssProperty, string wpfValue)
    {
        switch (cssProperty)
        {
            case "background-color":
            case "color":
            case "border-color":
                return s_ColorValueMap.TryGetValue(wpfValue, out var color) ? color : wpfValue;

            case "font-weight":
                return s_FontWeightMap.TryGetValue(wpfValue, out var fw) ? fw : wpfValue.ToLowerInvariant();

            case "cursor":
                return s_CursorMap.TryGetValue(wpfValue, out var cur) ? cur : wpfValue.ToLowerInvariant();

            case "visibility":
                return s_VisibilityMap.TryGetValue(wpfValue, out var vis) ? vis : wpfValue.ToLowerInvariant();

            case "font-style":
                return wpfValue.Equals("Italic", StringComparison.OrdinalIgnoreCase) ? "italic" : "normal";

            case "text-decoration":
                return wpfValue.Equals("Underline", StringComparison.OrdinalIgnoreCase) ? "underline" : "none";

            case "font-size":
                // Append "px" if no unit is present.
                return double.TryParse(wpfValue, out _) ? wpfValue + "px" : wpfValue;

            default:
                return wpfValue;
        }
    }

    #endregion

    #region Helpers

    // Custom comparer so that ("ismouseover", "true") matches regardless of original casing.
    private sealed class PseudoKeyComparer : IEqualityComparer<(string, string)>
    {
        public static readonly PseudoKeyComparer Instance = new();

        public bool Equals((string, string) x, (string, string) y)
            => string.Equals(x.Item1, y.Item1, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Item2, y.Item2, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode((string, string) obj)
            => HashCode.Combine(
                obj.Item1.ToLowerInvariant(),
                obj.Item2.ToLowerInvariant());
    }

    #endregion
}
