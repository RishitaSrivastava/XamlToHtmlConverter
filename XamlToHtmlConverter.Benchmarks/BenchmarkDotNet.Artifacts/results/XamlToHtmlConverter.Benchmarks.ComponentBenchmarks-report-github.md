``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LMRASR : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  16.11 ns |  0.998 ns | 0.155 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  71.74 ns | 36.161 ns | 9.391 ns |    3 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  37.02 ns | 26.310 ns | 6.833 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  35.12 ns |  0.738 ns | 0.192 ns |    2 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 236.53 ns | 13.886 ns | 3.606 ns |    4 | 0.1135 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-LMRASR(IterationCount=5, WarmupCount=3)
