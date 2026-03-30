``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26100.7840)
Unknown processor
.NET SDK=10.0.102
  [Host]     : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2
  Job-VCVTSE : .NET 8.0.23 (8.0.2325.60607), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |      Error |     StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-----------:|-----------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 19,191.77 ns | 688.644 ns | 106.568 ns |    4 | 6.1951 | 1.8616 |   81128 B |
|                XmlToIrConverterLinqStyle | 16,854.85 ns | 557.662 ns | 144.823 ns |    3 | 5.5542 | 1.5259 |   72616 B |
|                  &#39;Binding Parse: Simple&#39; |     15.89 ns |   1.913 ns |   0.296 ns |    1 | 0.0067 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,469.21 ns | 585.891 ns |  90.667 ns |    2 | 0.3815 |      - |    5096 B |
