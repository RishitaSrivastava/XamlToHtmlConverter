``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-MNXVDQ : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |   StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|---------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 301.6 μs | 58.60 μs | 15.22 μs |    2 | 27.3438 | 6.8359 |  342.6 KB |
|  &#39;Large XAML Conversion&#39; | 378.8 μs | 24.35 μs |  6.32 μs |    3 | 35.1563 | 9.7656 | 439.31 KB |
|     &#39;Parsing Phase Only&#39; | 157.6 μs | 13.21 μs |  2.04 μs |    1 |  9.2773 | 2.9297 | 119.26 KB |
|   &#39;Rendering Phase Only&#39; | 296.4 μs | 57.86 μs | 15.03 μs |    2 | 27.3438 | 6.8359 |  342.6 KB |
