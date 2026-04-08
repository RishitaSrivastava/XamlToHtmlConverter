``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-OKJBPZ : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                                  Method |      Mean |        Error |    StdDev |   Gen0 | Allocated |
|---------------------------------------- |----------:|-------------:|----------:|-------:|----------:|
|   &#39;End-to-End: XAML → File (Streaming)&#39; | 661.42 μs | 1,470.090 μs | 80.581 μs |      - |    1168 B |
| &#39;End-to-End: XAML → String (Streaming)&#39; |  15.07 μs |     3.034 μs |  0.166 μs | 0.0610 |     872 B |
