using System.Xml.Linq;

namespace XamlToWebViewApp.Core.Parsers
{
    /// <summary>
    /// Parses XAML using LINQ to XML and builds
    /// a hierarchical IR tree.
    /// </summary>
    public class XamlParser
    {
        /// <summary>
        /// Loads and parses a XAML file using LINQ to XML.
        /// </summary>
        /// <param name="path">Path of the XAML file</param>
        /// <returns>Root XElement of the XAML document</returns>
        public XElement Parse(string path)
        {
            return XDocument.Load(path).Root;
        }
    }
}