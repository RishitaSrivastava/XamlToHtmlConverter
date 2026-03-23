``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-EJPQAA : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  13.91 ns |  2.042 ns | 0.530 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  64.22 ns | 15.133 ns | 3.930 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  28.11 ns |  2.203 ns | 0.572 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  36.83 ns |  0.491 ns | 0.076 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 220.94 ns | 24.479 ns | 6.357 ns |    5 | 0.1135 | 0.0007 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-EJPQAA(IterationCount=5, WarmupCount=3)
