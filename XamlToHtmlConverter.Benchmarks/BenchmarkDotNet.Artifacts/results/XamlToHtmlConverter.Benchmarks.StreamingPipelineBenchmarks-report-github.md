``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-MEAWCW : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                                  Method |      Mean |      Error |   StdDev |   Gen0 | Allocated |
|---------------------------------------- |----------:|-----------:|---------:|-------:|----------:|
|   &#39;End-to-End: XAML → File (Streaming)&#39; | 699.98 μs | 134.668 μs | 7.382 μs |      - |    1169 B |
| &#39;End-to-End: XAML → String (Streaming)&#39; |  15.13 μs |   0.136 μs | 0.007 μs | 0.0610 |     872 B |
