``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.103
  [Host]     : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2
  Job-AJXHXC : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method | Mean | Error | Rank |
|----------------------------------------- |-----:|------:|-----:|
|                XmlToIrConverterRecursive |   NA |    NA |    ? |
|                XmlToIrConverterLinqStyle |   NA |    NA |    ? |
|                  &#39;Binding Parse: Simple&#39; |   NA |    NA |    ? |
| &#39;Style Registry: Multiple Registrations&#39; |   NA |    NA |    ? |

Benchmarks with issues:
  AlgorithmComparisonBenchmarks.XmlToIrConverterRecursive: Job-AJXHXC(IterationCount=5, WarmupCount=3)
  AlgorithmComparisonBenchmarks.XmlToIrConverterLinqStyle: Job-AJXHXC(IterationCount=5, WarmupCount=3)
  AlgorithmComparisonBenchmarks.'Binding Parse: Simple': Job-AJXHXC(IterationCount=5, WarmupCount=3)
  AlgorithmComparisonBenchmarks.'Style Registry: Multiple Registrations': Job-AJXHXC(IterationCount=5, WarmupCount=3)
