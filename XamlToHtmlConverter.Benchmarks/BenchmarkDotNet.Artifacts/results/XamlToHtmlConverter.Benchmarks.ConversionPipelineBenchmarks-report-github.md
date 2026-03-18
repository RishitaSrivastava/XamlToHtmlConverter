``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-ATVYNA : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method | Mean | Error | Rank |
|------------------------- |-----:|------:|-----:|
| &#39;Sample XAML Conversion&#39; |   NA |    NA |    ? |
|  &#39;Large XAML Conversion&#39; |   NA |    NA |    ? |
|     &#39;Parsing Phase Only&#39; |   NA |    NA |    ? |
|   &#39;Rendering Phase Only&#39; |   NA |    NA |    ? |

Benchmarks with issues:
  ConversionPipelineBenchmarks.'Sample XAML Conversion': Job-ATVYNA(IterationCount=5, WarmupCount=3)
  ConversionPipelineBenchmarks.'Large XAML Conversion': Job-ATVYNA(IterationCount=5, WarmupCount=3)
  ConversionPipelineBenchmarks.'Parsing Phase Only': Job-ATVYNA(IterationCount=5, WarmupCount=3)
  ConversionPipelineBenchmarks.'Rendering Phase Only': Job-ATVYNA(IterationCount=5, WarmupCount=3)
