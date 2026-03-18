``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-AJXHXC : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                Method | Mean | Error | Rank |
|-------------------------------------- |-----:|------:|-----:|
|     &#39;Regular Rendering (Full Buffer)&#39; |   NA |    NA |    ? |
|   &#39;Streaming Rendering (File Output)&#39; |   NA |    NA |    ? |
| &#39;Streaming Rendering (String Buffer)&#39; |   NA |    NA |    ? |

Benchmarks with issues:
  StreamingRenderingBenchmarks.'Regular Rendering (Full Buffer)': Job-AJXHXC(IterationCount=5, WarmupCount=3)
  StreamingRenderingBenchmarks.'Streaming Rendering (File Output)': Job-AJXHXC(IterationCount=5, WarmupCount=3)
  StreamingRenderingBenchmarks.'Streaming Rendering (String Buffer)': Job-AJXHXC(IterationCount=5, WarmupCount=3)
