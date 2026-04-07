``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-OKJBPZ : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |     Mean |     Error |   StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |---------:|----------:|---------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 347.8 μs | 316.21 μs | 17.33 μs | 82.5195 | 35.1563 | 1015.12 KB |
| &#39;High Style Diversity (100+ unique)&#39; | 103.6 μs |   9.08 μs |  0.50 μs | 23.6816 |  4.3945 |  290.41 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 178.8 μs | 102.29 μs |  5.61 μs | 25.3906 |       - |   312.5 KB |
