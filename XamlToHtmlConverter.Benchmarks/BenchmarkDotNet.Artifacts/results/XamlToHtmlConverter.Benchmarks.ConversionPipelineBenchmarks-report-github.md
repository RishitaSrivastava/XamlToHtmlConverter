``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-QPQEVP : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |   StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|---------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 303.9 μs | 46.84 μs | 12.16 μs |    2 | 26.3672 | 5.8594 | 325.69 KB |
|  &#39;Large XAML Conversion&#39; | 386.8 μs | 89.33 μs | 13.82 μs |    3 | 33.2031 | 7.8125 | 429.21 KB |
|     &#39;Parsing Phase Only&#39; | 197.9 μs | 90.03 μs | 23.38 μs |    1 |  9.7656 | 2.9297 |  127.7 KB |
|   &#39;Rendering Phase Only&#39; | 305.5 μs | 71.48 μs | 11.06 μs |    2 | 26.3672 | 5.8594 | 325.69 KB |
