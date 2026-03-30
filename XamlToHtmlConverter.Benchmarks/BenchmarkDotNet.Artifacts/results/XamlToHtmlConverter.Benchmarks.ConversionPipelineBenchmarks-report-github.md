``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-VCVTSE : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |  StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|--------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 283.1 μs | 13.65 μs | 3.54 μs |    2 | 23.4375 | 4.8828 | 302.75 KB |
|  &#39;Large XAML Conversion&#39; | 325.8 μs |  9.72 μs | 2.52 μs |    3 | 30.2734 | 7.8125 | 393.56 KB |
|     &#39;Parsing Phase Only&#39; | 171.3 μs | 18.31 μs | 4.76 μs |    1 |  9.2773 | 2.9297 | 121.44 KB |
|   &#39;Rendering Phase Only&#39; | 284.7 μs | 10.08 μs | 1.56 μs |    2 | 23.4375 | 4.8828 | 302.75 KB |
