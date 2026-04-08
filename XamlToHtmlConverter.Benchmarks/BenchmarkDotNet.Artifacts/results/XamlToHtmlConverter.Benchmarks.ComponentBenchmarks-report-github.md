``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-QPQEVP : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  13.67 ns |  1.078 ns | 0.280 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  64.43 ns |  4.604 ns | 1.196 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  27.30 ns |  1.077 ns | 0.280 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  40.34 ns | 13.448 ns | 3.492 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 253.92 ns | 12.850 ns | 1.989 ns |    5 | 0.1230 | 0.0005 |    1544 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-QPQEVP(IterationCount=5, WarmupCount=3)
