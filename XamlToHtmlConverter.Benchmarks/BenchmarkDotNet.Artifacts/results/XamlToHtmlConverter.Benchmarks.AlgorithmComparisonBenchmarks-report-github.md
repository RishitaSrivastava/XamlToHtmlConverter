``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.8037)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-AWBITN : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |       StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-------------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 19,805.63 ns |   602.890 ns |   156.569 ns |    3 | 6.4392 | 2.0752 |   81128 B |
|                XmlToIrConverterLinqStyle | 20,842.31 ns | 6,750.672 ns | 1,753.128 ns |    3 | 5.7678 | 1.4954 |   72616 B |
|                  &#39;Binding Parse: Simple&#39; |     14.98 ns |     0.939 ns |     0.145 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,765.32 ns |   535.707 ns |   139.121 ns |    2 | 0.3967 |      - |    5096 B |
