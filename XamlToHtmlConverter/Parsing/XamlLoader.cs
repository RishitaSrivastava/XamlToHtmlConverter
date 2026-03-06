// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Responsible for loading a XAML file from disk into an XML DOM.
/// Contains no IR conversion logic; serves solely as a file-loading utility.
/// </summary>
public class XamlLoader
{
    #region Public Methods

    /// <summary>
    /// Loads the XAML file at the specified path and returns it as an <see cref="XDocument"/>.
    /// </summary>
    /// <param name="path">The file system path to the XAML file.</param>
    /// <returns>An <see cref="XDocument"/> representing the loaded XAML document.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when no file exists at the specified <paramref name="path"/>.</exception>
    public XDocument Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.");

        if (!File.Exists(path))
            throw new FileNotFoundException("XAML file not found.", path);

        return XDocument.Load(path);
    }

    #endregion
}