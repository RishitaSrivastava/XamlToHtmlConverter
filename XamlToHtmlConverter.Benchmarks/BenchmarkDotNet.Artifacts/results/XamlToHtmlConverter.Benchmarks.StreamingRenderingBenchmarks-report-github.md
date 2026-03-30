``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-VCVTSE : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                Method | Mean | Error | Rank |
|-------------------------------------- |-----:|------:|-----:|
|     &#39;Regular Rendering (Full Buffer)&#39; |   NA |    NA |    ? |
|   &#39;Streaming Rendering (File Output)&#39; |   NA |    NA |    ? |
| &#39;Streaming Rendering (String Buffer)&#39; |   NA |    NA |    ? |

Benchmarks with issues:
  StreamingRenderingBenchmarks.'Regular Rendering (Full Buffer)': Job-VCVTSE(IterationCount=5, WarmupCount=3)
  StreamingRenderingBenchmarks.'Streaming Rendering (File Output)': Job-VCVTSE(IterationCount=5, WarmupCount=3)
  StreamingRenderingBenchmarks.'Streaming Rendering (String Buffer)': Job-VCVTSE(IterationCount=5, WarmupCount=3)
