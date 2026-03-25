``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-MNXVDQ : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  15.75 ns |  4.062 ns | 1.055 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  60.00 ns |  5.231 ns | 1.358 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  27.01 ns |  1.631 ns | 0.423 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  36.95 ns |  1.547 ns | 0.402 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 225.66 ns | 26.467 ns | 6.873 ns |    5 | 0.1135 | 0.0007 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-MNXVDQ(IterationCount=5, WarmupCount=3)
