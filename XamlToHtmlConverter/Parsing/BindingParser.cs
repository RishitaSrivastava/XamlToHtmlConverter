// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Parses XAML binding expressions (e.g., "{Binding Path=Name, Mode=TwoWay}")
/// into structured <see cref="IntermediateRepresentationBinding"/> objects.
/// </summary>
public static class BindingParser
{
    #region Public Methods

    /// <summary>
    /// Parses a XAML binding expression string into an IR binding object.
    /// Supports Path, Mode, ElementName, and RelativeSource properties.
    /// </summary>
    /// <param name="value">The XAML binding expression string (e.g., "{Binding UserName}").</param>
    /// <returns>
    /// An <see cref="IntermediateRepresentationBinding"/> object if parsing succeeds;
    /// otherwise, <c>null</c> if the value is not a valid binding expression.
    /// </returns>
    /// <remarks>
    /// Examples:
    /// <list type="bullet">
    /// <item><description>"{Binding Name}" → Path="Name"</description></item>
    /// <item><description>"{Binding Path=Name, Mode=TwoWay}" → Path="Name", Mode="TwoWay"</description></item>
    /// <item><description>"{Binding ElementName=TextBox1}" → ElementName="TextBox1"</description></item>
    /// </list>
    /// </remarks>
    public static IntermediateRepresentationBinding? Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!value.StartsWith("{Binding") || !value.EndsWith("}"))
            return null;

        var inner = value.Substring(8, value.Length - 9).Trim();

        var binding = new IntermediateRepresentationBinding();

        // Simple case: just a path without properties (e.g., "{Binding Name}")
        if (!inner.Contains("="))
        {
            binding.Path = inner;
            return binding;
        }

        // Complex case: parse comma-separated properties
        var parts = inner.Split(',');

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (trimmed.StartsWith("Path="))
                binding.Path = trimmed.Substring(5);

            else if (trimmed.StartsWith("Mode="))
                binding.Mode = trimmed.Substring(5);

            else if (trimmed.StartsWith("ElementName="))
                binding.ElementName = trimmed.Substring(12);

            else if (trimmed.StartsWith("RelativeSource="))
                binding.RelativeSource = trimmed.Substring(15);
        }

        return binding;
    }

    #endregion
    }