``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-AWBITN : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |  StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|--------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 288.0 μs | 10.97 μs | 2.85 μs |    2 | 24.4141 | 4.8828 |  308.6 KB |
|  &#39;Large XAML Conversion&#39; | 328.1 μs |  9.94 μs | 2.58 μs |    3 | 30.2734 | 7.8125 | 374.21 KB |
|     &#39;Parsing Phase Only&#39; | 176.2 μs | 14.55 μs | 3.78 μs |    1 |  9.7656 | 2.4414 | 121.29 KB |
|   &#39;Rendering Phase Only&#39; | 290.6 μs |  5.42 μs | 0.84 μs |    2 | 24.4141 | 4.8828 |  308.6 KB |
