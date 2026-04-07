``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LVLSQK : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |    Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|---------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  16.99 ns | 1.366 ns | 0.355 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  63.47 ns | 1.740 ns | 0.452 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  28.19 ns | 0.699 ns | 0.181 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  38.70 ns | 1.277 ns | 0.332 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 259.79 ns | 4.804 ns | 1.248 ns |    5 | 0.1235 | 0.0005 |    1552 B |
|      &#39;XamlLoader: Load&#39; |        NA |       NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-LVLSQK(IterationCount=5, WarmupCount=3)
