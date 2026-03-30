``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-BBHMCM : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                                  Method |      Mean |     Error |   StdDev |   Gen0 | Allocated |
|---------------------------------------- |----------:|----------:|---------:|-------:|----------:|
|   &#39;End-to-End: XAML → File (Streaming)&#39; | 566.02 μs | 65.952 μs | 3.615 μs |      - |    1217 B |
| &#39;End-to-End: XAML → String (Streaming)&#39; |  13.51 μs |  2.760 μs | 0.151 μs | 0.0610 |     928 B |
