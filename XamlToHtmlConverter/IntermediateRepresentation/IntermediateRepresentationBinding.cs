// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a parsed XAML data binding expression in the intermediate representation.
/// Captures binding metadata such as Path, Mode, ElementName, and RelativeSource
/// extracted from XAML markup like "{Binding UserName, Mode=TwoWay}".
/// </summary>
public class IntermediateRepresentationBinding
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the property path for the binding.
    /// Corresponds to the Path parameter in XAML bindings (e.g., "UserName" or "User.Name").
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the binding mode.
    /// Valid values include "OneWay", "TwoWay", "OneTime", "OneWayToSource".
    /// </summary>
    public string? Mode { get; set; }

    /// <summary>
    /// Gets or sets the name of the element to use as the binding source.
    /// Used when binding to another element in the visual tree.
    /// </summary>
    public string? ElementName { get; set; }

    /// <summary>
    /// Gets or sets the relative source specification for the binding.
    /// Defines the binding source relative to the position of the binding target.
    /// </summary>
    public string? RelativeSource { get; set; }

    #endregion
}