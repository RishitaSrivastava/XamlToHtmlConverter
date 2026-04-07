``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-LVLSQK : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |       StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-------------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 19,562.11 ns | 4,115.793 ns | 1,068.858 ns |    3 | 6.4697 | 2.1362 |   81432 B |
|                XmlToIrConverterLinqStyle | 19,058.38 ns |   650.236 ns |   168.864 ns |    3 | 6.3171 | 2.0752 |   79456 B |
|                  &#39;Binding Parse: Simple&#39; |     16.24 ns |     1.341 ns |     0.348 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,432.57 ns |   147.853 ns |    38.397 ns |    2 | 0.4044 |      - |    5136 B |
