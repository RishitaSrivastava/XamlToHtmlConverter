``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-BBHMCM : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |      Mean |     Error |   StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |----------:|----------:|---------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 299.90 μs | 171.70 μs | 9.411 μs | 79.1016 | 33.2031 | 1015.08 KB |
| &#39;High Style Diversity (100+ unique)&#39; |  90.94 μs |  26.05 μs | 1.428 μs | 22.7051 |  3.6621 |  290.38 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 152.60 μs |  78.38 μs | 4.296 μs | 24.4141 |       - |   312.5 KB |
