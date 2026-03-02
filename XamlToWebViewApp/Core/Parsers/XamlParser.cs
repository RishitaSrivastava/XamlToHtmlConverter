using System;
using System.IO;
using System.Xml.Linq;

namespace XamlToWebViewApp.Core.Parsers
{
    /// <summary>
    /// Responsible for loading and parsing a XAML file
    /// into an XElement representation using LINQ to XML.
    ///
    /// This acts as the entry point of the conversion pipeline:
    /// XAML → XElement → IR → HTML.
    /// </summary>
    public class XamlParser
    {
        /// <summary>
        /// Loads a XAML file from disk and parses it into an XElement.
        /// </summary>
        /// <param name="filePath">Absolute path to the XAML file.</param>
        /// <returns>Root XElement of the parsed XAML.</returns>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the specified file does not exist.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when XAML parsing fails.
        /// </exception>
        public XElement Parse(string filePath)
        {
            // Validate file existence
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    $"XAML file not found: {filePath}");
            }

            try
            {
                // Load XAML using LINQ to XML
                return XElement.Load(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to parse XAML file.", ex);
            }
        }
    }
}





















//using System.Xml.Linq;

//namespace XamlToWebViewApp.Core.Parsers
//{
//    /// <summary>
//    /// Parses XAML using LINQ to XML and builds
//    /// a hierarchical IR tree.
//    /// </summary>
//    public class XamlParser
//    {
//        /// <summary>
//        /// Loads and parses a XAML file using LINQ to XML.
//        /// </summary>
//        /// <param name="path">Path of the XAML file</param>
//        /// <returns>Root XElement of the XAML document</returns>
//        public XElement? Parse(string path)
//        {
//            return XDocument.Load(path).Root;
//        }
//    }
//}