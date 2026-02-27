using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using XamlToWebViewApp.Core.Generators;
using XamlToWebViewApp.Core.IR;
using XamlToWebViewApp.Core.Parsers;

namespace XamlToWebViewApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Build path to sample.xaml inside output directory

                string filePath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Samples",
                        "sample.xml");

                //string filePath = Path.Combine(
                //        AppDomain.CurrentDomain.BaseDirectory,
                //        "sample.xaml");

                //string filePath = Path.Combine(
                //        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.FullName,
                //        "sample.xaml");
                //string projectPath = Directory
                //        .GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                //        .Parent!
                //        .Parent!
                //        .Parent!
                //        .FullName;

                //string filePath = Path.Combine(projectPath, "sample.xaml");
                //MessageBox.Show(File.Exists(filePath).ToString());

                MessageBox.Show(filePath);


                if (!File.Exists(filePath))
                {
                    MessageBox.Show(
                        "sample.xaml not found in output directory.",
                        "File Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // 1️⃣ Parse XAML
                var parser = new XamlParser();
                var xamlRoot = parser.Parse(filePath);

                // 2️⃣ Convert to IR
                var builder = new IrBuilder();
                var ir = builder.Build(xamlRoot);

                // 3️⃣ Generate HTML
                var generator = new HtmlGenerator();
                var html = generator.Generate(ir);

                // 4️⃣ Display in WebView
                await Preview.EnsureCoreWebView2Async();
                Preview.NavigateToString(html);
                //MessageBox.Show(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Unexpected Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}