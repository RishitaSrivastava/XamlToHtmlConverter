``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-HQWSCD : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |      Mean |     Error |    StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |----------:|----------:|----------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 299.09 μs | 424.35 μs | 23.260 μs | 82.5195 | 38.0859 | 1015.08 KB |
| &#39;High Style Diversity (100+ unique)&#39; |  87.01 μs | 101.11 μs |  5.542 μs | 23.6816 |  4.1504 |  290.38 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 162.75 μs | 340.41 μs | 18.659 μs | 25.3906 |       - |   312.5 KB |
