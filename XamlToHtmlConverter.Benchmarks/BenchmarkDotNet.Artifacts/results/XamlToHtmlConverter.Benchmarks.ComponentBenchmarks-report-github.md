``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-AWBITN : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |    Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|---------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  15.56 ns | 2.547 ns | 0.661 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  63.53 ns | 9.977 ns | 2.591 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  29.65 ns | 3.973 ns | 1.032 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  38.37 ns | 3.532 ns | 0.547 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 236.13 ns | 6.220 ns | 1.615 ns |    5 | 0.1135 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |       NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-AWBITN(IterationCount=5, WarmupCount=3)
