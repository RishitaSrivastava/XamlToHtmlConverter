``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-QPQEVP : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |       StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-------------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 22,339.57 ns | 7,115.676 ns | 1,101.159 ns |    4 | 6.9885 | 2.2888 |   87728 B |
|                XmlToIrConverterLinqStyle | 18,422.56 ns | 1,420.231 ns |   219.782 ns |    3 | 6.3171 | 2.0752 |   79456 B |
|                  &#39;Binding Parse: Simple&#39; |     14.83 ns |     1.436 ns |     0.373 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,508.52 ns | 1,162.831 ns |   179.949 ns |    2 | 0.4044 |      - |    5136 B |
