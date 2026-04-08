``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LVLSQK : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |     Error |   StdDev | Rank |    Gen0 |    Gen1 | Allocated |
|------------------------- |---------:|----------:|---------:|-----:|--------:|--------:|----------:|
| &#39;Sample XAML Conversion&#39; | 387.5 μs | 114.29 μs | 29.68 μs |    3 | 32.2266 |  7.8125 |  399.2 KB |
|  &#39;Large XAML Conversion&#39; | 520.6 μs | 156.42 μs | 40.62 μs |    4 | 44.9219 | 13.6719 | 557.63 KB |
|     &#39;Parsing Phase Only&#39; | 177.7 μs |   8.85 μs |  2.30 μs |    1 |  9.7656 |  2.9297 | 121.55 KB |
|   &#39;Rendering Phase Only&#39; | 308.8 μs |  18.43 μs |  2.85 μs |    2 | 32.2266 |  7.8125 |  399.2 KB |
