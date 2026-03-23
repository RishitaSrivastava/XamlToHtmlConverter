``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-EJPQAA : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |     StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-----------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 18,084.09 ns | 1,087.516 ns | 282.424 ns |    4 | 6.3477 | 1.9226 |   79928 B |
|                XmlToIrConverterLinqStyle | 15,942.62 ns | 1,780.909 ns | 462.496 ns |    3 | 5.6763 | 1.5564 |   71512 B |
|                  &#39;Binding Parse: Simple&#39; |     16.06 ns |     3.490 ns |   0.540 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,087.77 ns |   565.201 ns | 146.781 ns |    2 | 0.4044 |      - |    5096 B |
