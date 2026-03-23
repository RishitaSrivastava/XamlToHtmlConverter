using XamlToHtmlConverter.IntermediateRepresentation;

public static class VirtualizationDetector
{
    public static bool RequiresVirtualization(IntermediateRepresentationElement element)
    {
        if (!element.Bindings.ContainsKey("ItemsSource"))
            return false;

        return element.Type == "ListView"
            || element.Type == "DataGrid"
            || element.Type == "ItemsControl";
    }
}