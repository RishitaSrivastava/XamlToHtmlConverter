public static class VirtualizationStyleInjector
{
    public static string Build()
    {
        return @"
<style>
.virtual-scroll-host {
    height: 400px;
    overflow-y: auto;
    position: relative;
}

.virtual-scroll-content {
    position: relative;
}
</style>";
    }
}