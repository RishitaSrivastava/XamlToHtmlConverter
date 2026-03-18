``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-ATVYNA : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  16.01 ns |  1.515 ns | 0.393 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  71.04 ns | 11.779 ns | 3.059 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  32.44 ns |  2.614 ns | 0.679 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  41.41 ns |  2.955 ns | 0.457 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 273.36 ns | 26.559 ns | 6.897 ns |    5 | 0.1135 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-ATVYNA(IterationCount=5, WarmupCount=3)
