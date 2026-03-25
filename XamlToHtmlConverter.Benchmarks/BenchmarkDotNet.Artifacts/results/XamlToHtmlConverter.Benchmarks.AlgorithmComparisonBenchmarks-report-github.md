``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-MNXVDQ : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |     StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-----------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 18,888.11 ns | 1,486.245 ns | 229.998 ns |    4 | 6.3477 | 1.9226 |   79928 B |
|                XmlToIrConverterLinqStyle | 16,912.03 ns | 2,319.226 ns | 602.296 ns |    3 | 5.6763 | 1.5564 |   71512 B |
|                  &#39;Binding Parse: Simple&#39; |     21.51 ns |     9.849 ns |   2.558 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,325.94 ns |   629.125 ns | 163.382 ns |    2 | 0.3967 |      - |    5096 B |
