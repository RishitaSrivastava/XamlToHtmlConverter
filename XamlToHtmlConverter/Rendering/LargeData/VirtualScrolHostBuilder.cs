using System.Text;

public static class VirtualScrollHostBuilder
{
    public static void Build(StringBuilder sb, int indent)
    {
        var spacing = new string(' ', indent);

        sb.AppendLine($"{spacing}<div class=\"virtual-scroll-host\">");
        sb.AppendLine($"{spacing}  <div class=\"virtual-scroll-content\"></div>");
        sb.AppendLine($"{spacing}</div>");
    }
}