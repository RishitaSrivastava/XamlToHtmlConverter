``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LMRASR : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |   StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|---------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 292.4 μs |  9.71 μs |  2.52 μs |    2 | 24.4141 | 4.8828 | 300.38 KB |
|  &#39;Large XAML Conversion&#39; | 332.3 μs | 10.22 μs |  2.65 μs |    2 | 30.2734 | 7.8125 | 374.21 KB |
|     &#39;Parsing Phase Only&#39; | 177.4 μs |  3.90 μs |  1.01 μs |    1 |  9.2773 | 2.9297 | 119.26 KB |
|   &#39;Rendering Phase Only&#39; | 311.8 μs | 92.11 μs | 23.92 μs |    2 | 24.4141 | 4.8828 | 300.38 KB |
