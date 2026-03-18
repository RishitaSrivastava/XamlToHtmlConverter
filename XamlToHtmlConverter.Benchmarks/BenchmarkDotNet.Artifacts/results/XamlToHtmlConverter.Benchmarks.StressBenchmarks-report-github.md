``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-UYJSWG : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |     Mean |    Error |  StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |---------:|---------:|--------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 322.7 μs | 72.40 μs | 3.97 μs | 82.5195 | 36.1328 | 1015.08 KB |
| &#39;High Style Diversity (100+ unique)&#39; | 105.9 μs | 23.31 μs | 1.28 μs | 23.6816 |  4.1504 |  290.38 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 176.8 μs | 33.48 μs | 1.83 μs | 25.3906 |       - |   312.5 KB |
