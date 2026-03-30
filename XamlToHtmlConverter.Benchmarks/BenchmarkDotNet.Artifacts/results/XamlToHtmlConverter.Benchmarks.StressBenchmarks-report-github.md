``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-HTAAMM : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |      Mean |     Error |    StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |----------:|----------:|----------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 289.19 μs | 292.67 μs | 16.042 μs | 82.5195 | 35.1563 | 1015.12 KB |
| &#39;High Style Diversity (100+ unique)&#39; |  92.38 μs |  58.78 μs |  3.222 μs | 23.6816 |  4.3945 |  290.41 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 153.15 μs | 181.45 μs |  9.946 μs | 25.3906 |       - |   312.5 KB |
