// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a neutral intermediate representation (IR) element
/// parsed from a XAML document. This model is independent from
/// WPF runtime types and serves as the bridge between XML parsing
/// and HTML rendering.
/// </summary>
public class IntermediateRepresentationElement
{
    #region Public Properties

    /// <summary>
    /// Gets the XAML element type name (e.g., Grid, Button, TextBlock).
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the regular properties defined as attributes in XAML.
    /// Example: Width="100"
    /// </summary>
    public Dictionary<string, string> Properties { get; }

    /// <summary>
    /// Gets or sets the direct inner text content of the element, if any.
    /// </summary>
    public string? InnerText { get; set; }

    /// <summary>
    /// Gets the attached properties (e.g., Grid.Row="1").
    /// These are stored separately from regular properties for layout mapping.
    /// </summary>
    public Dictionary<string, string> AttachedProperties { get; }

    /// <summary>
    /// Gets or sets the child elements in the logical tree.
    /// </summary>
    public List<IntermediateRepresentationElement> Children { get; set; }

    /// <summary>
    /// Gets the collection of row definitions used when the element represents a Grid.
    /// Stores raw XAML GridLength values (e.g., "Auto", "*", "2*").
    /// </summary>
    public List<string> GridRowDefinitions { get; } = new();

    /// <summary>
    /// Gets the collection of column definitions used when the element represents a Grid.
    /// Stores raw XAML GridLength values for column sizing.
    /// </summary>
    public List<string> GridColumnDefinitions { get; } = new();

    /// <summary>
    /// Gets or sets a reference to the parent IR element in the visual tree.
    /// Enables upward traversal and context-aware processing.
    /// </summary>
    public IntermediateRepresentationElement? Parent { get; set; }

    /// <summary>
    /// Gets the resource dictionary associated with this element.
    /// Stores keyed style definitions and other reusable resources.
    /// </summary>
    public Dictionary<string, IntermediateRepresentationStyle> Resources { get; } = new();

    /// <summary>
    /// Gets or sets the control template associated with this element,
    /// if one is defined.
    /// </summary>
    public IntermediateRepresentationElement? Template { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="IntermediateRepresentationElement"/> with the specified element type.
    /// </summary>
    /// <param name="type">The XAML element type name (e.g., Grid, Button, TextBlock).</param>
    public IntermediateRepresentationElement(string type)
    {
        Type = type;
        Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AttachedProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Children = new List<IntermediateRepresentationElement>();
    }

    #endregion
}