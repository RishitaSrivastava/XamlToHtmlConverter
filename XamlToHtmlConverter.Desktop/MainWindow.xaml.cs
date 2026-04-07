using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;
using Microsoft.Win32;
using Microsoft.Web.WebView2.Wpf;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// Main window for the split-view XAML to HTML converter.
/// Left panel: XAML editing, Right panel: HTML output
/// </summary>
public partial class MainWindow : Window
{
    private readonly XmlToIrConverterRecursive _converter;
    private readonly HtmlRenderer _renderer;
    private string _currentFilePath = string.Empty;
    private string _currentHtmlOutput = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        _converter = new XmlToIrConverterRecursive();
        _renderer = HtmlRendererFactory.Create();
        
        InitializeWebView();
        LoadSampleXaml();
    }

    /// <summary>
    /// Initializes WebView2 control
    /// </summary>
    private async void InitializeWebView()
    {
        try
        {
            var userDataFolder = Path.Combine(Path.GetTempPath(), "HtmlWebViewCache");
            await HtmlWebView.EnsureCoreWebView2Async(null);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 initialization warning: {ex.Message}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// Handles the Open File button click
    /// </summary>
    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "XAML files (*.xaml)|*.xaml|All files (*.*)|*.*",
            Title = "Open XAML File"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                _currentFilePath = dialog.FileName;
                var xamlContent = File.ReadAllText(_currentFilePath);
                XamlEditor.Text = xamlContent;
                FileStatusText.Text = $"📂 Loaded: {Path.GetFileName(_currentFilePath)}";
                ConversionStatusText.Text = "File loaded. Click 'Convert' to convert to HTML.";
                HtmlOutput.Text = string.Empty;
                HtmlStatusText.Text = "Awaiting conversion...";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ConversionStatusText.Text = "Error loading file";
            }
        }
    }

    /// <summary>
    /// Loads the sample XAML file
    /// </summary>
    private void LoadSample_Click(object sender, RoutedEventArgs e)
    {
        LoadSampleXaml();
    }

    /// <summary>
    /// Loads the built-in sample XAML
    /// </summary>
    private void LoadSampleXaml()
    {
        var sampleXaml = @"<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
         xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
         Title=""Sample Application"" Width=""400"" Height=""300"">
    <StackPanel Margin=""20"">
        <TextBlock Text=""Welcome to XAML to HTML Converter"" FontSize=""18"" FontWeight=""Bold"" Margin=""0,0,0,10""/>
        <TextBlock Text=""This demonstrates the conversion of XAML to HTML"" Margin=""0,0,0,20""/>
        
        <TextBlock Text=""Input Name:"" FontWeight=""Bold""/>
        <TextBox Placeholder=""Enter your name"" Margin=""0,5,0,15"" Padding=""8""/>
        
        <Button Content=""Submit"" Background=""#0078D4"" Foreground=""White"" Padding=""10"" Margin=""0,10,0,0""/>
    </StackPanel>
</Window>";

        _currentFilePath = "sample.xaml";
        XamlEditor.Text = sampleXaml;
        FileStatusText.Text = "📄 Sample XAML loaded";
        ConversionStatusText.Text = "Ready to convert...";
        HtmlOutput.Text = string.Empty;
        HtmlStatusText.Text = "Awaiting conversion...";
    }

    /// <summary>
    /// Handles the Convert button click
    /// Converts XAML to HTML using the existing conversion pipeline
    /// </summary>
    private void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(XamlEditor.Text))
        {
            MessageBox.Show("Please load or enter XAML content first.", "No Content", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            ConversionStatusText.Text = "Converting...";
            var startTime = DateTime.Now;

            // Save XAML to temporary file
            var tempXamlPath = Path.Combine(Path.GetTempPath(), "temp_input.xaml");
            File.WriteAllText(tempXamlPath, XamlEditor.Text);

            // Load and convert
            // Load XAML using the parser
            var loader = new XamlLoader();
            var document = loader.Load(tempXamlPath);

            if (document.Root == null)
            {
                ConversionStatusText.Text = "Error: Invalid XAML - no root element";
                HtmlOutput.Text = "Error: The XAML must have a root element.";
                DisplayXamlPreviewError("Invalid XAML: No root element");
                return;
            }

            // Render XAML visually
            RenderXamlVisually(XamlEditor.Text);

            // Convert XAML to IR
            var ir = _converter.Convert(document.Root);

            // Render to HTML
            var html = _renderer.RenderDocument(ir);

            // Display HTML
            HtmlOutput.Text = html;
            _currentHtmlOutput = html;
            
            // Display HTML in WebView
            DisplayHtmlInWebView(html);

            var elapsed = DateTime.Now - startTime;
            ConversionStatusText.Text = $"✓ Converted successfully in {elapsed.TotalMilliseconds:F0}ms";
            HtmlStatusText.Text = $"HTML generated ({html.Length} characters)";
            MetricsText.Text = $"Conversion time: {elapsed.TotalMilliseconds:F2}ms | HTML size: {html.Length / 1024.0:F2}KB";

            // Cleanup
            if (File.Exists(tempXamlPath))
                File.Delete(tempXamlPath);
        }
        catch (Exception ex)
        {
            ConversionStatusText.Text = "❌ Conversion failed";
            HtmlOutput.Text = $"Error during conversion:\n{ex.Message}\n\n{ex.StackTrace}";
            HtmlStatusText.Text = "Error: " + ex.Message;
            DisplayXamlPreviewError($"Conversion Error:\n{ex.Message}");
            MessageBox.Show($"Conversion error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Renders XAML as visual output
    /// </summary>
    private void RenderXamlVisually(string xamlString)
    {
        try
        {
            // Prepare XAML for preview by replacing Window/Page root with a safe container
            var xamlToRender = PrepareXamlForPreview(xamlString);

            using (var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xamlToRender)))
            {
                var xamlElement = XamlReader.Load(memoryStream);
                
                if (xamlElement != null)
                {
                    XamlPreviewHost.Child = null;

                    if (xamlElement is UIElement uiElement)
                    {
                        XamlPreviewHost.Child = new Border
                        {
                            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                            Padding = new Thickness(10),
                            Child = uiElement
                        };
                    }
                    else
                    {
                        DisplayXamlPreviewError($"XAML parsed but not a UIElement: {xamlElement.GetType().Name}");
                    }
                }
            }
        }
        catch (XamlParseException xpe)
        {
            DisplayXamlPreviewError($"XAML Parse Error:\n{xpe.Message}");
        }
        catch (Exception ex)
        {
            DisplayXamlPreviewError($"Error rendering XAML:\n{ex.Message}");
        }
    }

    /// <summary>
    /// Prepares XAML for live preview by replacing Window/Page/UserControl root elements
    /// with a ScrollViewer so XamlReader.Load can parse it without a host window.
    /// </summary>
    private static string PrepareXamlForPreview(string xamlString)
    {
        try
        {
            var doc = XDocument.Parse(xamlString);
            var root = doc.Root;
            if (root == null) return xamlString;

            var rootName = root.Name.LocalName;

            if (rootName is "Window" or "Page" or "UserControl")
            {
                // Preserve all namespace declarations from the root element
                var nsDeclarations = string.Join(" ",
                    root.Attributes()
                        .Where(a => a.IsNamespaceDeclaration || a.Name.LocalName == "xmlns")
                        .Select(a => $"{a.Name}=\"{a.Value}\""));

                // Get all child elements as inner XML
                var innerXml = string.Join(Environment.NewLine, root.Elements().Select(e => e.ToString()));

                if (string.IsNullOrWhiteSpace(innerXml))
                    return xamlString;

                // Wrap children in a ScrollViewer > StackPanel for safe rendering
                return $@"<ScrollViewer {nsDeclarations}
                    VerticalScrollBarVisibility=""Auto""
                    HorizontalScrollBarVisibility=""Auto"">
                    <StackPanel>
                        {innerXml}
                    </StackPanel>
                </ScrollViewer>";
            }

            return xamlString;
        }
        catch
        {
            // If XML parsing fails, return original so XamlReader reports the real error
            return xamlString;
        }
    }

    /// <summary>
    /// Displays error message in XAML preview area
    /// </summary>
    private void DisplayXamlPreviewError(string message)
    {
        XamlPreviewHost.Child = new TextBlock
        {
            Text = message,
            Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10),
            VerticalAlignment = VerticalAlignment.Top
        };
    }

    /// <summary>
    /// Displays HTML in WebView2
    /// </summary>
    private void DisplayHtmlInWebView(string html)
    {
        try
        {
            if (HtmlWebView.CoreWebView2 != null)
            {
                HtmlWebView.CoreWebView2.NavigateToString(html);
            }
            else
            {
                // WebView2 not ready, show message
                HtmlWebView.NavigateToString("<html><body><p style='color: #999;'>WebView2 initializing...</p></body></html>");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error displaying HTML in WebView: {ex.Message}", "WebView Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    /// <summary>
    /// Copies the HTML output to clipboard
    /// </summary>
    private void CopyHtml_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentHtmlOutput))
        {
            MessageBox.Show("No HTML to copy. Please convert XAML first.", "Empty", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            Clipboard.SetText(_currentHtmlOutput);
            MessageBox.Show("HTML copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Saves the HTML output to a file
    /// </summary>
    private void SaveHtml_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(HtmlOutput.Text))
        {
            MessageBox.Show("No HTML to save. Please convert XAML first.", "Empty", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*",
            Title = "Save HTML File",
            DefaultExt = ".html",
            FileName = Path.GetFileNameWithoutExtension(_currentFilePath) + ".html"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, HtmlOutput.Text);
                MessageBox.Show($"HTML saved to:\n{dialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
