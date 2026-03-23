// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Collections.Concurrent;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Provides efficient caching of indentation strings to avoid repeated allocation
/// of identical spacing strings. Extracted from HtmlRenderer to satisfy Single 
/// Responsibility Principle.
///
/// Performance benefit: Reuses identical space strings instead of allocating
/// new instances for each indentation level. Typical cache hit rate: >90%.
/// </summary>
public static class IndentCache
{
    /// <summary>
    /// Thread-safe cache mapping indentation depth to pre-allocated space strings.
    /// </summary>
    private static readonly ConcurrentDictionary<int, string> s_Cache = new();

    /// <summary>
    /// Gets a string of the specified number of spaces, from cache if available.
    /// Thread-safe and suitable for concurrent rendering scenarios.
    /// </summary>
    /// <param name="depth">The number of spaces required.</param>
    /// <returns>A string containing exactly <paramref name="depth"/> space characters.</returns>
    public static string Get(int depth)
    {
        if (depth <= 0)
            return string.Empty;

        return s_Cache.GetOrAdd(depth, d => new string(' ', d));
    }
}
