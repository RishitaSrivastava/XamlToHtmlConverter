using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using XamlToWebViewApp.Core.Generators;
using XamlToWebViewApp.Core.IR;
using XamlToWebViewApp.Core.Parsers;

namespace XamlToWebViewApp
{
    public partial class MainWindow : Window
    {
        private bool _webViewInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ensures WebView2 initializes only once.
        /// Prevents black screen caused by early initialization.
        /// </summary>
        private async Task EnsureWebViewReadyAsync()
        {
            if (_webViewInitialized)
                return;

            await Preview.EnsureCoreWebView2Async();
            _webViewInitialized = true;
        }

        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Samples",
                    "sample.xml");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show(
                        "sample.xml not found in output directory.",
                        "File Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // 1️⃣ Parse
                var parser = new XamlParser();
                var xamlRoot = parser.Parse(filePath);

                // 2️⃣ IR
                var builder = new IrBuilder();
                var ir = builder.Build(xamlRoot);

                // 3️⃣ HTML
                var generator = new HtmlGenerator();
                var html = generator.Generate(ir);

                // 4️⃣ Initialize WebView ONLY when needed
                await EnsureWebViewReadyAsync();

                if (Preview.CoreWebView2 != null)
                {
                    //// Reset surface (fixes random blank rendering)
                    //Preview.CoreWebView2.Navigate("about:blank");

                    //// Load HTML
                    //Preview.NavigateToString(html);

                    // Create temp html file
                    string tempFile = Path.Combine(
                        Path.GetTempPath(),
                        "preview.html");

                    File.WriteAllText(tempFile, html);

#if DEBUG
                    //MessageBox.Show(html);
                    System.Diagnostics.Debug.WriteLine("----- Generated HTML -----");
                    System.Diagnostics.Debug.WriteLine(html);
                    System.Diagnostics.Debug.WriteLine("--------------------------");
#endif

                    // Navigate using file URL (VERY STABLE)
                    Preview.CoreWebView2.Navigate(new Uri(tempFile).AbsoluteUri);
                }
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









//using System;
//using System.IO;
//using System.Windows;
//using System.Windows.Controls;
//using XamlToWebViewApp.Core.Generators;
//using XamlToWebViewApp.Core.IR;
//using XamlToWebViewApp.Core.Parsers;

//namespace XamlToWebViewApp
//{
//    public partial class MainWindow : Window
//    {
//        public MainWindow()
//        {
//            InitializeComponent();
//        }

//        private async void Convert_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                //Build path to sample.xaml inside output directory

//                string filePath = Path.Combine(
//                        AppDomain.CurrentDomain.BaseDirectory,
//                        "Samples",
//                        "sample.xml");

//                //string filePath = Path.Combine(
//                //        AppDomain.CurrentDomain.BaseDirectory,
//                //        "sample.xaml");

//                //string filePath = Path.Combine(
//                //        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.FullName,
//                //        "sample.xaml");
//                //string projectPath = Directory
//                //        .GetParent(AppDomain.CurrentDomain.BaseDirectory)!
//                //        .Parent!
//                //        .Parent!
//                //        .Parent!
//                //        .FullName;

//                //string filePath = Path.Combine(projectPath, "sample.xaml");
//                //MessageBox.Show(File.Exists(filePath).ToString());

//                MessageBox.Show(filePath);


//                if (!File.Exists(filePath))
//                {
//                    MessageBox.Show(
//                        "sample.xaml not found in output directory.",
//                        "File Error",
//                        MessageBoxButton.OK,
//                        MessageBoxImage.Error);
//                    return;
//                }

//                // 1️⃣ Parse XAML
//                var parser = new XamlParser();
//                var xamlRoot = parser.Parse(filePath);

//                // 2️⃣ Convert to IR
//                var builder = new IrBuilder();
//                var ir = builder.Build(xamlRoot);

//                // 3️⃣ Generate HTML
//                var generator = new HtmlGenerator();
//                var html = generator.Generate(ir);

//                // 4️⃣ Display in WebView
//                await Preview.EnsureCoreWebView2Async();
//                Preview.NavigateToString(html);
//                //MessageBox.Show(html);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(
//                    ex.Message,
//                    "Unexpected Error",
//                    MessageBoxButton.OK,
//                    MessageBoxImage.Error);
//            }
//        }
//    }
//}