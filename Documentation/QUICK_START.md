# XamlToHtmlConverter Benchmarking - Quick Reference

## 🚀 Run Benchmarks

### Full Suite (All Benchmarks)
```powershell
cd XamlToHtmlConverter.Benchmarks
dotnet run -c Release
```

### Pipeline Tests Only
```powershell
dotnet run -c Release --filter "*ConversionPipelineBenchmarks*"
```

### Component Tests Only
```powershell
dotnet run -c Release --filter "*ComponentBenchmarks*"
```

### Comparison Tests Only
```powershell
dotnet run -c Release --filter "*AlgorithmComparisonBenchmarks*"
```

### Stress Tests Only
```powershell
dotnet run -c Release --filter "*StressBenchmarks*"
```

### Single Benchmark
```powershell
# Example: Only test ConvertSampleXaml
dotnet run -c Release --filter "*ConvertSampleXaml*"
```

---

## 📊 Results Location

All benchmark results save to: **`./BenchmarkResults/`**

Files generated:
- `*-results.json` - Full machine-readable results
- `*-results.md` - Human-readable summary
- `*-results.html` - Visual charts (if RPlotExporter enabled)
- `baseline.json` - Baseline for comparisons

---

## 📈 Interpreting Output

### Key Metrics

| Column | Meaning | Good Value |
|--------|---------|-----------|
| **Mean** | Average time | As low as possible |
| **StdDev** | Consistency | < 10% of Mean (predictable) |
| **Median** | 50th percentile | Close to Mean |
| **Min** | Best case | Lower bound |
| **Max** | Worst case | Shouldn't be > 2x Mean |
| **Allocated** | Memory per operation | Minimal allocations |

### Example Output

```
| Method              | Mean      | StdDev   | Median    | Allocated |
|:------------------:|----------:|--------:|----------:|----------:|
| ConvertSampleXaml   | 15.24 ms  | 1.32 ms | 15.18 ms  | 2.45 MB   |
| ConvertLargeXaml    | 42.31 ms  | 3.14 ms | 41.95 ms  | 6.78 MB   |
```

---

## 🔍 Performance Analysis Workflow

### Step 1: Establish Baseline
```powershell
# Run all benchmarks first time
dotnet run -c Release

# This creates baseline.json automatically
# Future runs will compare against this
```

### Step 2: Make Optimization
- Modify code (e.g., Phase 1 optimization)
- Rebuild solution

### Step 3: Run Benchmarks Again
```powershell
dotnet run -c Release
```

### Step 4: Review Comparison
Look for output like:
```
ConvertSampleXaml: -12.5% (IMPROVEMENT ✅)
ConvertLargeXaml: +2.1% (REGRESSION ⚠️)
```

### Step 5: Accept or Revert
- If improvement: commit changes
- If regression: investigate or revert

---

## 💡 Pro Tips

### 1. Run in Release Mode
**ALWAYS** use `-c Release` - Debug mode skews results by 10-100x

### 2. Close Other Programs
Benchmarking is sensitive to background activity:
- Close browser, IDE, Slack, etc.
- Minimize Windows updates
- Disable scheduled tasks

### 3. Multiple Runs = Accuracy
Results stabilize after several runs:
```powershell
# Run 3x to warm up system
dotnet run -c Release
dotnet run -c Release
dotnet run -c Release
```

### 4. Track Improvement
Save results in version control:
```bash
git add ./XamlToHtmlConverter.Benchmarks/BenchmarkResults/
git commit -m "Perf: Phase 1 optimization - 25% improvement"
```

### 5. Compare Specific Benchmarks
```powershell
# Edit baseline.json path to compare different versions
# Then use BenchmarkResultsAnalyzer to generate report
```

---

## 🎯 Expected Improvements by Phase

Track improvements as you implement optimizations:

```
BASELINE (Before any optimization):
  ConvertSampleXaml: 20ms, 3.2MB, 5,000 allocations

PHASE 1 (Parser & Style Caching):
  ConvertSampleXaml: 16ms (-20%), 2.8MB (-12%), 4,200 allocations (-16%)

PHASE 2 (Tree Consolidation):
  ConvertSampleXaml: 12ms (-40%), 2.0MB (-37%), 3,100 allocations (-38%)

PHASE 3 (String Operations):
  ConvertSampleXaml: 10.5ms (-47%), 1.8MB (-43%), 2,800 allocations (-44%)

PHASE 4 (Caching & Observability):
  ConvertSampleXaml: 7.5ms (-62%), 1.5MB (-53%), 2,000 allocations (-60%)
```

---

## 🚨 Troubleshooting

### Issue: Results vary wildly (StdDev > 50%)
**Solution:**
- Close all other applications
- Run benchmark again (3-5 times)
- Check Task Manager for background processes
- Try: `dotnet run -c Release --launchCount 1`

### Issue: ".NET Runtime Version" warnings
**Solution:** This is normal. BenchmarkDotNet auto-manages runtime.

### Issue: Out of memory errors
**Solution:** Large document benchmark uses significant memory
- Reduce sample file size for testing
- Run on machine with 8GB+ RAM
- Or reduce target count: Edit benchmark class attribute

### Issue: "No benchmarks found"
**Solution:**
- Ensure methods have `[Benchmark]` attribute
- Ensure class has `[SimpleJob(...)]` attribute
- Check spelling of method names
- Rebuild solution: `dotnet clean && dotnet build -c Release`

---

## 📊 Advanced: Custom Benchmark

```csharp
[Benchmark(Description = "My Custom Test")]
public void MyCustomBenchmark()
{
    // Your performance-critical code here
    for (int i = 0; i < 1000; i++)
    {
        // Code to benchmark
    }
}
```

Then run:
```powershell
dotnet run -c Release --filter "*MyCustomBenchmark*"
```

---

## 🔗 CI/CD Integration

### GitHub Actions Example
```yaml
- name: Run Benchmarks
  run: |
    cd XamlToHtmlConverter.Benchmarks
    dotnet run -c Release --filter "*ConversionPipelineBenchmarks*"

- name: Check Regressions
  run: |
    if grep -q "⚠️ REGRESSION" BenchmarkResults/*.md; then
      echo "Regression detected!"
      exit 1
    fi
```

---

## 📚 Links

- **Full Guide**: README_BENCHMARKING.md
- **Phases**: PHASE_BY_PHASE_IMPLEMENTATION.md
- **Bottlenecks**: PERFORMANCE_BOTTLENECKS.md
- **BenchmarkDotNet**: https://benchmarkdotnet.org/

---

**Last Updated**: March 18, 2026
