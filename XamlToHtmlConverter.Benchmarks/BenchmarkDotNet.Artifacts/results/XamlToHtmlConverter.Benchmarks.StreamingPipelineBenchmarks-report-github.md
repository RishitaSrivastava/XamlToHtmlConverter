``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-HTAAMM : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                                  Method |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|---------------------------------------- |----------:|----------:|----------:|-------:|----------:|
|   &#39;End-to-End: XAML → File (Streaming)&#39; | 633.86 μs | 302.70 μs | 16.592 μs |      - |    1169 B |
| &#39;End-to-End: XAML → String (Streaming)&#39; |  14.74 μs |  14.74 μs |  0.808 μs | 0.0610 |     872 B |
