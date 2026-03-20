``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-MEAWCW : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |     Mean |    Error |  StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |---------:|---------:|--------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 311.6 μs | 95.46 μs | 5.23 μs | 82.5195 | 38.0859 | 1015.08 KB |
| &#39;High Style Diversity (100+ unique)&#39; | 100.6 μs | 44.25 μs | 2.43 μs | 23.6816 |  4.1504 |  290.38 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 170.4 μs | 45.10 μs | 2.47 μs | 25.3906 |       - |   312.5 KB |
