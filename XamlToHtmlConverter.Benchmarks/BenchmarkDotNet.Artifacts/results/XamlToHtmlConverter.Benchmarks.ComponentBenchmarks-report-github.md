``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-VCVTSE : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  16.34 ns |  0.548 ns | 0.142 ns |    1 | 0.0067 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  66.29 ns | 14.546 ns | 3.778 ns |    4 | 0.0092 |      - |     120 B |
|    &#39;Style Registration&#39; |  29.36 ns |  1.251 ns | 0.325 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  37.59 ns |  0.968 ns | 0.252 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 225.64 ns |  4.507 ns | 0.697 ns |    5 | 0.1090 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-VCVTSE(IterationCount=5, WarmupCount=3)
