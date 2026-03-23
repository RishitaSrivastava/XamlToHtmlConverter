# XamlToHtmlConverter Benchmarking System

> Professional performance measurement framework for tracking optimization impact and detecting regressions.

## 🎯 Overview

This benchmarking system provides comprehensive performance analysis using **BenchmarkDotNet**, industry-standard for .NET performance testing.

### What It Measures

- **Execution Time**: Milliseconds per operation with statistical analysis
- **Memory Allocations**: Bytes allocated per operation
- **GC Pressure**: Garbage collection collections triggered
- **Throughput**: Operations per second
- **Statistical Variance**: StdDev, Min, Max for reliability
- **Regression Detection**: Automatic comparison with previous runs

---

## 📦 Benchmark Categories

### 1. **ConversionPipelineBenchmarks**

Full end-to-end XAML to HTML conversion pipeline testing.

```
✅ ConvertSampleXaml        - Small document (100-200 elements)
✅ ConvertLargeXaml         - Complex document (500+ elements)
✅ ParseOnly               - XML → IR conversion phase only
✅ RenderOnly              - IR → HTML rendering phase only
```

**Use Case**: Monitor overall performance and identify bottleneck phases.

### 2. **ComponentBenchmarks**

Isolated component testing for focused analysis.

```
✅ ParseSimpleBinding       - Simple "{Binding UserName}" parsing
✅ ParseComplexBinding      - Complex multi-property binding parsing
✅ RegisterStyle            - CSS style registration and deduplication
✅ StyleCacheHit            - Cache effectiveness measurement
✅ ParseXmlToIr             - Parser performance in isolation
✅ LoadXaml                 - File I/O + XML parsing
✅ RendererLookup           - Renderer resolution performance
```

**Use Case**: Debug specific component bottlenecks in isolation.

### 3. **AlgorithmComparisonBenchmarks**

Compare alternative implementations to measure optimization gains.

```
✅ ConvertRecursive         - Current recursive implementation
✅ ConvertLinqStyle         - Previous LINQ-based implementation
```

**Use Case**: Validate optimization correctness and measure improvement.

### 4. **StressBenchmarks**

High-load scenarios to test performance scaling.

```
✅ HighElementCount         - 500+ elements with varied styles
✅ HighStyleDiversity       - 100+ unique styles
✅ RepeatedBindingParsing   - 1000 binding parse operations
```

**Use Case**: Identify performance degradation at scale.

---

## 🚀 Quick Start

### Run All Benchmarks (Release Mode - Important!)

```bash
cd XamlToHtmlConverter.Benchmarks
dotnet run -c Release
```

### Run Specific Benchmark Class

```bash
dotnet run -c Release --filter "*ConversionPipelineBenchmarks*"
```

### Run Single Benchmark

```bash
dotnet run -c Release --filter "*ConvertSampleXaml*"
```

### Save Results as Baseline

```bash
# Run benchmarks
dotnet run -c Release

# Results auto-save to ./BenchmarkResults/
# Copy latest to baseline.json for future comparisons
```

---

## 📊 Understanding Results

### Typical Output

```
| Method                      | Mean       | StdDev   | Memory   | Alloc   |
|:--------------------------- |-----------:|----------:|----------:|-------:|
| ConvertSampleXaml           | 15.23 ms   | 1.42 ms  | 2.45 MB  | 4,521  |
| ConvertLargeXaml            | 42.15 ms   | 3.21 ms  | 6.78 MB  | 12,450 |
| ParseOnly                   | 5.32 ms    | 0.58 ms  | 0.82 MB  | 1,234  |
| RenderOnly                  | 9.91 ms    | 0.84 ms  | 1.63 MB  | 3,287  |
```

### Key Metrics

| Metric     | What It Means | Good Value |
|-----------|--------------|-----------|
| **Mean**  | Average execution time | As low as possible |
| **StdDev**| Consistency (lower = more predictable) | < 10% of Mean |
| **Memory**| Peak memory used | Linear to element count |
| **Alloc** | Allocations per operation | < 100 per element |

---

## 📈 Performance Tracking

### Baseline Comparison

After first run, subsequent runs automatically compare against baseline:

```
📊 Comparison Summary:
  Improvements: 3 ✅
  Regressions:  0
  
  • ConvertSampleXaml
    Time: -8.5% ✅ (2ms faster)
    Memory: -15.3% ✅ (0.3MB less)
    
  • ParseOnly
    Time: -12.1% ✅ (optimization working!)
    Memory: -22.5% ✅
```

### Regression Detection

Automatic alerts for performance regressions (> 5% slowdown):

```
❌ REGRESSION DETECTED!
  • RenderOnly
    Time: +7.2% (2.5ms slower)
    Likely cause: New feature or inefficient code path
```

---

## 🔍 Advanced Usage

### Memory Profiling with Diagnostics

BenchmarkDotNet includes detailed memory diagnostics:

```
Allocated Bytes:    12,450
GC Allocations:     147
Gen0 Collections:   2
Gen1 Collections:   0
Gen2 Collections:   0
```

**Interpretation**:
- Low allocation count = Efficient
- Zero Gen2 collections = Good cache locality
- High Gen0 activity = Short-lived objects (baseline)

### Comparing Versions Side-by-Side

1. **Baseline**: Run benchmarks on original version
   ```bash
   dotnet run -c Release
   ```
   → Saves to `./BenchmarkResults/baseline.json`

2. **Optimized**: Modify code and run again
   ```bash
   dotnet run -c Release
   ```
   → Automatically compares against baseline

3. **Report**: Check regression/improvement results

---

## 🛠️ Customization

### Adding New Benchmarks

1. Create method in appropriate benchmark class
2. Decorate with `[Benchmark]` attribute
3. Run: `dotnet run -c Release`

Example:

```csharp
[Benchmark(Description = "My Custom Test")]
public object MyBenchmark()
{
    // Your code here
    return result;
}
```

### Adjusting Warmup/Target Iterations

Edit benchmark class attribute:

```csharp
[SimpleJob(warmupCount: 5, targetCount: 10)]  // More iterations = more accurate
```

### Memory Diagnostics

```csharp
[MemoryDiagnoser]  // Enables memory tracking (slight overhead)
```

---

## 📋 Benchmark Results Location

All results save to: `./BenchmarkResults/`

Files generated:
- `2026-03-18-12-34-56-results-json.json` - Full results
- `baseline.json` - Baseline for comparisons
- `results.md` - Human-readable report

---

## ⚡ Performance Optimization Workflow

### Before Optimization

```
1. Run current benchmarks
2. Save as baseline
3. Note specific slow benchmarks
```

### During Optimization

```
1. Modify code (one optimization at a time)
2. Run benchmarks
3. Check regression/improvement report
4. Commit only if improvement > 5%
```

### After Optimization

```
1. Compare Phase 1 vs Phase 2 results
2. Calculate total improvement: (Phase1 - Phase2) / Phase1
3. Update baseline: "baseline.json"
4. Document results in DELIVERY_SUMMARY.md
```

---

## 🎯 Optimization Improvement Tracking

Expected improvements from Phase implementations:

| Phase | Benchmark | Target Improvement | Actual |
|------|-----------|-------------------|--------|
| Phase 1 | ParseOnly | 20-25% | ___ |
| Phase 1 | RegisterStyle | 15-20% | ___ |
| Phase 2 | ConvertSampleXaml | 30-40% | ___ |
| Phase 3 | RenderOnly | 10-15% | ___ |
| Phase 4 | All | Cumulative | ___ |

---

## 🔗 Integration with CI/CD

### Track Performance in Commits

```bash
#!/bin/bash
# Run benchmarks and track improvement
cd XamlToHtmlConverter.Benchmarks
dotnet run -c Release
# Commit results to git
git add BenchmarkResults/
git commit -m "Perf: Phase 1 optimization - 25% improvement"
```

### Automated Regression Detection

```bash
# Alert if regression detected
if grep -q "REGRESSION" results.md; then
  echo "❌ Performance regression detected!"
  exit 1
fi
```

---

## 📚 References

- **BenchmarkDotNet Documentation**: https://benchmarkdotnet.org/
- **Performance Optimization Guide**: See PHASE_BY_PHASE_IMPLEMENTATION.md
- **Bottleneck Analysis**: See PERFORMANCE_BOTTLENECKS.md

---

## 💡 Tips

1. **Always run in Release mode**: `-c Release` is mandatory for accurate results
2. **Warmup iterations matter**: Ensures JIT compilation completes
3. **Run multiple times**: Check baseline.json for consistency
4. **Profile memory**: Allocations reveal inefficiencies early
5. **Compare per-element metrics**: Helps identify scaling issues

---

## 🚨 Common Issues

### "Results vary too much (high StdDev)"
- Close other programs
- Run again (increase samples)
- Check for background tasks

### "Memory not tracked"
- Ensure `[MemoryDiagnoser]` attribute present
- Run in Release mode
- May need `--genDiff` flag

### "Baseline comparison missing"
- This is first run - run again to see comparison
- Check `./BenchmarkResults/baseline.json` exists

---

Generated: March 18, 2026
