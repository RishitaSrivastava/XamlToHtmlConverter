using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

/// <summary>
/// Maps XAML form element types (Button, TextBox, CheckBox, etc.) to default CSS styling.
/// Ensures consistent heights and appearance for all form controls.
/// Supports Dependency Inversion Principle by allowing substitution of styling strategies.
/// </summary>
public class FormElementMapper : IPropertyMapper
{
    // Default height for form elements (buttons, inputs, checkboxes, etc.)
    private const string DEFAULT_FORM_HEIGHT = "32px";

    public bool CanHandle(string propertyName)
    {
        // This mapper doesn't handle specific properties;
        // it applies default styling based on element type instead.
        // We'll hook it via a special mechanism or apply it universally.
        return false;
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        // No-op: this mapper uses element type detection instead
    }

    /// <summary>
    /// Applies default form element styling based on XAML element type.
    /// Called directly from BuildStyle() instead of through PropertyMapperEngine.
    /// Excludes CheckBox and RadioButton (styled differently in label wrapper).
    /// </summary>
    /// <param name="element">The IR element to style.</param>
    /// <param name="sb">The style builder to append CSS to.</param>
    public static void ApplyFormElementStyle(IntermediateRepresentationElement element, StringBuilder sb)
    {
        // Add default height and styling for form controls
        // EXCLUDE CheckBox and RadioButton - they are styled in label wrappers with smaller checkbox height
        switch (element.Type)
        {
            case "Button":
            case "TextBox":
            case "PasswordBox":
            case "DatePicker":
            case "Slider":
            case "ComboBox":
                // Add default height if not explicitly set
                if (!element.Properties.ContainsKey("Height"))
                {
                    sb.Append($"height:{DEFAULT_FORM_HEIGHT};");
                }
                // Ensure proper vertical alignment for inline form elements
                sb.Append("vertical-align:middle;");
                break;
        }
    }

    /// <summary>
    /// Applies smaller checkbox/radiobutton styling (18px instead of 32px).
    /// Used when checkbox/radiobutton is rendered inside a label wrapper.
    /// </summary>
    /// <param name="sb">The style builder to append CSS to.</param>
    public static void ApplyCheckboxStyle(StringBuilder sb)
    {
        sb.Append("width:18px;height:18px;margin:0px 4px;");
    }
}
