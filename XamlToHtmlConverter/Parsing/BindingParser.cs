using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing;

public static class BindingParser
{
    public static IntermediateRepresentationBinding? Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!value.StartsWith("{Binding") || !value.EndsWith("}"))
            return null;

        var inner = value.Substring(8, value.Length - 9).Trim();

        var binding = new IntermediateRepresentationBinding();

        if (!inner.Contains("="))
        {
            binding.Path = inner;
            return binding;
        }

        var parts = inner.Split(',');

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (trimmed.StartsWith("Path="))
                binding.Path = trimmed.Substring(5);

            else if (trimmed.StartsWith("Mode="))
                binding.Mode = trimmed.Substring(5);

            else if (trimmed.StartsWith("ElementName="))
                binding.ElementName = trimmed.Substring(12);

            else if (trimmed.StartsWith("RelativeSource="))
                binding.RelativeSource = trimmed.Substring(15);
        }

        return binding;
    }
}