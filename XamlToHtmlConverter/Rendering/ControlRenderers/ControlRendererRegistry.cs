using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class ControlRendererRegistry
{
    private readonly List<IControlRenderer> v_Renderers;

    public ControlRendererRegistry(IEnumerable<IControlRenderer> renderers)
    {
        v_Renderers = renderers.ToList();
    }
    public IControlRenderer? Resolve(IntermediateRepresentationElement element)
    {
        Console.WriteLine("Resolving renderer for: " + element.Type);

        foreach (var r in v_Renderers)
        {
            Console.WriteLine("Checking renderer: " + r.GetType().Name);

            if (r.CanHandle(element))
            {
                Console.WriteLine("MATCHED: " + r.GetType().Name);
                return r;
            }
        }

        Console.WriteLine("NO RENDERER FOUND");
        return null;
    }
    //public IControlRenderer? Resolve(IntermediateRepresentationElement element)
    //{
    //    return v_Renderers.FirstOrDefault(r => r.CanHandle(element));
    //}
}