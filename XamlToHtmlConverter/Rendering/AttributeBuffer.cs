using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    public class AttributeBuffer
    {
        private readonly Dictionary<string, string> v_Attributes = new();

        public void Add(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                return;

            v_Attributes[name] = value;
        }

        public void WriteTo(StringBuilder sb)
        {
            foreach (var attr in v_Attributes)
            {
                sb.Append(' ');
                sb.Append(attr.Key);
                sb.Append("=\"");
                sb.Append(attr.Value);
                sb.Append('"');
            }
        }
    }
}