``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LMRASR : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |     StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-----------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 19,991.17 ns | 2,897.515 ns | 752.476 ns |    4 | 6.3477 | 1.9226 |   79928 B |
|                XmlToIrConverterLinqStyle | 17,022.15 ns |   458.858 ns |  71.009 ns |    3 | 5.6763 | 1.5564 |   71512 B |
|                  &#39;Binding Parse: Simple&#39; |     15.66 ns |     1.207 ns |   0.314 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,208.07 ns |   215.498 ns |  33.348 ns |    2 | 0.4044 |      - |    5096 B |
