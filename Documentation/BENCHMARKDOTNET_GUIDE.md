# BenchmarkDotNet Explained: Complete Guide

## Why `dotnet run -c Release` Instead of `dotnet run`?

### The Difference

| Aspect | Debug Mode | Release Mode |
|--------|-----------|--------------|
| **Optimizations** | ❌ None | ✅ Full (inlining, dead code removal) |
| **JIT Compilation** | Basic | Aggressive (method inlining, loop unrolling) |
| **Performance** | ~10-50x slower | True real-world speed |
| **Relevance** | Not realistic | ✅ Production representative |

### Why It Matters for Benchmarks

```
Debug:   Can't convert XAML? Let's take 100ms  
Release: Same conversion? Takes 5ms

Without Release, benchmarks are meaningless.
```

**Rule**: Always use `-c Release` for any performance measurement.

---

## BenchmarkDotNet Output Phases Explained

When you run `dotnet run -c Release`, BenchmarkDotNet automatically executes through **5 phases**:

### Phase 1: Jitting (JIT Compilation)
```
OverheadJitting  1: 1 op, 241600.00 ns, 241.6000 us/op
WorkloadJitting  1: 1 op, 2188500.00 ns, 2.1885 ms/op
```
- **Purpose**: Load the JIT compiler and compile your code to native machine code
- **What It Means**: First run is always slow (one-time setup cost)
- **Included in Final Result?** ❌ NO - this is pure overhead

---

### Phase 2: Pilot Phase
```
WorkloadPilot   1: 16 op, 768400.00 ns, 48.0250 us/op
WorkloadPilot   2: 32 op, 1927400.00 ns, 60.2313 us/op
WorkloadPilot   3: 64 op, 4386000.00 ns, 68.5312 us/op
...
WorkloadPilot  12: 32768 op, 606198200.00 ns, 18.4997 us/op
```
- **Purpose**: Find the optimal batch size so each measurement takes ~100ms
- **What It Does**: 
  - Runs 16 operations (takes 48us)
  - Runs 32 operations (takes 60us)
  - Keeps doubling until hitting ~100ms target
  - Settles on 32,768 operations per batch
- **Why**: Too small batch = measurements too fast to be accurate; too large = tests run forever
- **Included in Final Result?** ❌ NO - calibration only

---

### Phase 3: Warmup Phase
```
OverheadWarmup   1: 32768 op, 104600.00 ns, 3.1921 ns/op
OverheadWarmup   2: 32768 op, 100100.00 ns, 3.0548 ns/op
OverheadWarmup   3: 32768 op, 103100.00 ns, 3.1464 ns/op

WorkloadWarmup   1: 32768 op, 732970200.00 ns, 22.3685 us/op
WorkloadWarmup   2: 32768 op, 813609800.00 ns, 24.8294 us/op
WorkloadWarmup   3: 32768 op, 930598200.00 ns, 28.3996 us/op
```
- **Purpose**: Pre-warm CPU caches, branch predictors, and JIT hot-path optimizations
- **What It Does**:
  - Measures measurement overhead (OverheadWarmup)
  - Runs your benchmark 3 times (WorkloadWarmup)
  - Allows CPU to reach steady-state performance
- **Why**: Without warmup, first real measurements would be artificially slow
- **Included in Final Result?** ❌ NO - these are thrown away

---

### Phase 4: Actual Measurement Phase ⭐ **THIS IS YOUR REAL DATA**
```
OverheadActual   1: 32768 op, 104600.00 ns, 3.1921 ns/op
OverheadActual   2: 32768 op, 104900.00 ns, 3.2013 ns/op
...
OverheadActual   5: 32768 op, 105300.00 ns, 3.2135 ns/op

WorkloadActual   1: 32768 op, 804509000.00 ns, 24.5517 us/op
WorkloadActual   2: 32768 op, 855188600.00 ns, 26.0983 us/op
WorkloadActual   3: 32768 op, 709822300.00 ns, 21.6621 us/op
WorkloadActual   4: 32768 op, 683800800.00 ns, 20.8679 us/op
WorkloadActual   5: 32768 op, 715608500.00 ns, 21.8386 us/op

AfterActualRun
WorkloadResult   1: 32768 op, 804405700.00 ns, 24.5485 us/op
WorkloadResult   2: 32768 op, 855085300.00 ns, 26.0951 us/op
...
```

- **Purpose**: Collect real performance data
- **Default**: 5 iterations (5 measurements)
- **What's Measured**:
  - `OverheadActual`: Cost of the measurement framework (loop overhead, etc.)
  - `WorkloadActual`: Total time for 32,768 operations of your code
  - `WorkloadResult`: Same as Actual, but measured after GC completes
- **Included in Final Result?** ✅ **YES** - These 5 measurements are averaged for your final result

---

### Phase 5: Final Statistics & Report
```
Mean = 23.001 us, StdErr = 0.992 us (4.31%), N = 5, StdDev = 2.219 us
Min = 20.865 us, Q1 = 21.659 us, Median = 21.835 us, Q3 = 24.549 us, Max = 26.095 us
IQR = 2.890 us, LowerFence = 17.324 us, UpperFence = 28.883 us
ConfidenceInterval = [14.457 us; 31.544 us] (CI 99.9%), Margin = 8.544 us (37.15% of Mean)
Skewness = 0.36, Kurtosis = 1.02, MValue = 2
```

- **Purpose**: Analyze the 5 measurements and compute statistics
- **Output**:
  - **Mean**: Average performance (23.001 microseconds per operation) ← **Use this for comparisons**
  - **StdDev**: Variability (2.219 us = 9.6% variance = GOOD)
  - **Min/Max**: Fastest (20.865 us) vs Slowest (26.095 us)
  - **Median**: Middle value (ignores outliers)
  - **CI**: 99.9% confident true value is in [14.457, 31.544] us

---

## Understanding Specific Terms

### What is "Overhead"?

```
WorkloadActual 1: 32768 op, 804509000.00 ns
OverheadActual 1: 32768 op, 104600.00 ns
```

**Overhead** = Time to run the measurement loop with NO code in it
- Includes: loop iteration, increment counter, call stack
- Does NOT include: your actual benchmark code

**Real time for your code**:
```
(804509000 ns - 104600 ns) / 32768 ops = 24.05 us/op
```

BenchmarkDotNet subtracts this automatically.

---

### What is "AfterActualRun"?

After each `WorkloadActual` measurement, BenchmarkDotNet measures again with `WorkloadResult` to capture any GC collections that happened during the run.

```
// Before GC signal
WorkloadActual 1: ... 24.5517 us/op

// After GC completes (if GC happened)
WorkloadResult 1: ... 24.5485 us/op
```

Most of the time these are nearly identical. If they differ significantly, it means the GC ran during measurement.

---

## Reading the Final Report

### The Mean (Average)
```
Mean = 23.001 us
```
- Each operation takes ~23 microseconds on average
- **Use this for comparing benchmarks across versions**

### The StdDev (Standard Deviation)
```
StdDev = 2.219 us
Percentage = 2.219 / 23.001 = 9.6%
```
- Variance is 9.6% of the mean
- **Interpretation**:
  - < 1%: Excellent (very consistent)
  - 1-5%: Very good (consistent)
  - 5-10%: Good ← Our result here
  - 10-20%: Acceptable
  - \> 20%: Problematic (GC interference, context switches)

### Confidence Interval (CI)
```
CI 99.9% = [14.457 us;  31.544 us]
```
- We're 99.9% confident the true average is between 14.457 and 31.544 microseconds
- Wider interval = less confidence
- Narrower interval = more confidence (fewer outliers)

---

## Performance Analysis Example

```
ConversionPipelineBenchmarks.ConvertXamlToIr:
Mean = 23.001 us, StdErr = 0.992 us (4.31%), N = 5, StdDev = 2.219 us
```

**What this tells you**:
- ✅ Each XAML-to-IR conversion takes ~23 microseconds
- ✅ Variance is low (9.6%) = very consistent performance
- ✅ Can be confident in comparing this to other versions
- ✅ No major GC or context-switch interference

---

## Why Not Just Use `dotnet run` (Debug Mode)?

```
Debug Build Output:
  Mean = 250.000 us

Release Build Output:
  Mean = 23.001 us  ← 10x faster!
```

Debug mode adds:
- No method inlining (extra function call overhead)
- No loop unrolling
- Debug checks and assertions still running
- Smaller code, less branch prediction optimization

**Result**: Benchmarks in Debug are 5-20x slower than reality. Not useful.

---

## Summary

| Phase | Output | Counted? | Why |
|-------|--------|----------|-----|
| Jitting | OverheadJitting, WorkloadJitting | ❌ | One-time setup |
| Pilot | WorkloadPilot 1-12 | ❌ | Calibration |
| Warmup | OverheadWarmup, WorkloadWarmup | ❌ | Pre-warm caches |
| **Actual** | **OverheadActual, WorkloadActual** | **✅** | **Real measurements** |
| Statistics | Mean, StdDev, Min, Max, CI | ✅ | Final result |

**Remember**:
- Always use `-c Release` for real benchmarks
- Focus on the **Mean** value for comparisons
- Check **StdDev** to see if the benchmark is stable
- An **StdDev < 10%** means you can trust the results

---

**Generated**: March 23, 2026  
For: XamlToHtmlConverter.Benchmarks project  
Framework: BenchmarkDotNet 0.13.x
