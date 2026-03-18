``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-AJXHXC : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |    Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|---------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  18.93 ns | 5.643 ns | 1.465 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  68.36 ns | 3.372 ns | 0.876 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  31.28 ns | 4.607 ns | 0.713 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  40.54 ns | 1.172 ns | 0.181 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 257.08 ns | 7.694 ns | 1.998 ns |    5 | 0.1135 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |       NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-AJXHXC(IterationCount=5, WarmupCount=3)
