using NUnit.Framework;
using System.Xml.Linq;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Tests.Rendering
{
    public class DockPanelDockTests
    {
        [Test]
        public void DockPanel_WithTopDock_RendersCorrectStyle()
        {
            var xaml =
@"<DockPanel>
    <Button DockPanel.Dock='Top' Content='Header'/>
</DockPanel>";

            var document = XDocument.Parse(xaml);

            var converter = new XmlToIrConverterRecursive();
            var ir = converter.Convert(document.Root!);

            var renderer = HtmlRendererFactory.Create();
            var html = renderer.RenderDocument(ir);

            Assert.That(html, Does.Contain("align-self:stretch"));
        }
    }
}