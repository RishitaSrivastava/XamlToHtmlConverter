// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using BenchmarkDotNet.Running;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Entry point for the XamlToHtmlConverter benchmarking suite.
/// Runs performance benchmarks using BenchmarkDotNet framework.
/// 
/// Usage:
///   dotnet run -c Release
/// 
/// Output:
///   - BenchmarkDotNet results (execution time, memory, GC stats)
///   - Baseline comparison if previous run exists
///   - Memory allocation breakdown
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(System.Reflection.Assembly.GetExecutingAssembly());
    }
}
