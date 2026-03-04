using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XamlToWebViewApp.Core.Rendering
{
    /// <summary>
    /// Automatically discovers and provides renderers
    /// using reflection and RendererAttribute.
    /// </summary>
    public static class RendererFactory
    {
        private static readonly Dictionary<string, IElementRenderer> _renderers;

        static RendererFactory()
        {
            _renderers = new Dictionary<string, IElementRenderer>();

            RegisterRenderers();
        }

        /// <summary>
        /// Scans assembly and registers all renderer classes automatically.
        /// </summary>
        private static void RegisterRenderers()
        {
            var rendererTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    typeof(IElementRenderer).IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract);

            foreach (var type in rendererTypes)
            {
                var attribute = type.GetCustomAttribute<RendererAttribute>();

                if (attribute != null)
                {
                    var instance = (IElementRenderer)Activator.CreateInstance(type)!;
                    _renderers[attribute.ElementType] = instance;
                }
            }
        }

        /// <summary>
        /// Returns the renderer associated with the given XAML element type.
        /// If no renderer is found, a default renderer is returned.
        /// </summary>
        public static IElementRenderer GetRenderer(string type)
        {
            return _renderers.TryGetValue(type, out var renderer)
                ? renderer
                : new DefaultRenderer();
        }
    }
}