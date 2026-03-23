``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-EJPQAA : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |  StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|--------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 250.8 μs | 14.64 μs | 2.27 μs |    2 | 23.4375 | 4.8828 |  287.5 KB |
|  &#39;Large XAML Conversion&#39; | 288.9 μs | 10.51 μs | 2.73 μs |    3 | 29.2969 | 7.8125 | 359.43 KB |
|     &#39;Parsing Phase Only&#39; | 167.8 μs | 37.70 μs | 9.79 μs |    1 |  9.2773 | 2.9297 | 119.26 KB |
|   &#39;Rendering Phase Only&#39; | 253.2 μs | 18.66 μs | 4.85 μs |    2 | 23.4375 | 4.8828 |  287.5 KB |
