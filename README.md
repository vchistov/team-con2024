# A Review of Approaches to Implementing the Repository with Connection Resiliency

Presentation for Team Convention 2024

Assume:

- SQL is kind of storage
- ORM is prohibited to use
- [Dapper](https://github.com/DapperLib/Dapper) is allowed to use

Transient fault handling policy is not subject of the review. The package like [Polly](https://github.com/App-vNext/Polly) can be used for demonstration purposes. The main goal is to show a storage querying implementation in integration with retry policy.

## All in One

Source code: src/DataAccess.AllInOne

The repository implements some use-cases as a set of methods, e.g. getting list of territories by region Id. A sample includes get operations only, but it can be spread to modifying operations. An implementation is straightforward. The repository keeps a retry policy. Responsibility of each method is implementing use case (creates connection, builds and executes sql query) and enforce it with the policy.

## Strategy with Retry Policy

Source code: src/DataAccess.Strategy

Unlike the previous approach the retry policy is moved out from the repository. Also creation of connection is not responsibility of the repository. A separate strategy implements connection management and transient fault handling. The repository gets the strategy through constructor. Responsibility of each method is implementing use case (builds sql query) and enforce it with the strategy.

## Decorator with Retry Policy

Source code: src/DataAccess.Decorated

The repository implements some use-cases as a set of methods, e.g. getting list of territories by region Id. The repository knows nothing about retry policy. Responsibility of each method is creating a connection, building and executing SQL query over the connection. A decorator implements the repository's interface. It keeps a retry policy. Responsibility of each method is calling appropriate repository's methods and enforce it with the policy.

## Command-Query Separation (CQS)

Source code: src/DataAccess.Cqs

In this approach the use-case is divided into two parts. The first one is contract. It desribes **what** the use-case implements, e.g. getting list of territories. The contract implements `IQuery` interface. The second part is contract implementation. It desribes **how**, e.i. build appropriate SQL query to storage. It implements `IQueryHandler` interface. Unlike the previous approaches use-cases are implemented as classes instead of methods. Responsibility of the repository is mapping queries to appropriate handlers (hardcode). Transient fault handling is implemented as kind of decorator for query handlers. The decorator is called by repository before calling a handler.

## DotNet Source Generators

Source code: src/DataAccess.SourceGenerators

Evolution of the previous approach. Implementation of the repository is generated in compile time instead of hardcode. The source generator relies on types `IQuery` and `IQueryHandler` to generate mapping. The generator is implemented in separate project [Repono](https://github.com/vchistov/repono).

## Benchmarks

```text
BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4291/22H2/2022Update)
Intel Core i7-7700T CPU 2.90GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
```

### Getting List of Territories by Region Id

| Method                           | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| GetTerritoriesByRegion_AllInOne  | 31.77 us | 0.326 us | 0.289 us | 31.63 us |  1.00 |    0.00 | 1.0986 |   4.66 KB |        1.00 |
| GetTerritoriesByRegion_Strategy  | 31.83 us | 0.364 us | 0.304 us | 31.79 us |  1.00 |    0.02 | 1.0986 |   4.73 KB |        1.02 |
| GetTerritoriesByRegion_Decorated | 31.59 us | 0.628 us | 0.880 us | 31.34 us |  1.00 |    0.03 | 1.0986 |    4.5 KB |        0.97 |
| GetTerritoriesByRegion_Cqs       | 31.93 us | 0.249 us | 0.221 us | 31.82 us |  1.01 |    0.01 | 1.1597 |    4.8 KB |        1.03 |
| GetTerritoriesByRegion_SourceGen | 31.81 us | 0.427 us | 0.379 us | 31.80 us |  1.00 |    0.01 | 1.1597 |    4.8 KB |        1.03 |

### Getting Single Territory by Id

| Method                          | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| GetTerritoryByIdAsync_AllInOne  | 20.56 us | 0.283 us | 0.265 us | 20.57 us |  1.00 |    0.00 | 0.7019 |   2.91 KB |        1.00 |
| GetTerritoryByIdAsync_Strategy  | 20.28 us | 0.323 us | 0.286 us | 20.23 us |  0.99 |    0.02 | 0.7019 |   2.98 KB |        1.02 |
| GetTerritoryByIdAsync_Decorated | 20.47 us | 0.260 us | 0.243 us | 20.40 us |  1.00 |    0.02 | 0.6714 |   2.82 KB |        0.97 |
| GetTerritoryByIdAsync_Cqs       | 21.04 us | 0.294 us | 0.431 us | 20.88 us |  1.03 |    0.03 | 0.7629 |   3.13 KB |        1.08 |
| GetTerritoryByIdAsync_SourceGen | 21.29 us | 0.343 us | 0.321 us | 21.20 us |  1.04 |    0.01 | 0.7629 |   3.13 KB |        1.08 |

## See Also

- [MS Docs | Transient fault handling](https://learn.microsoft.com/en-us/azure/architecture/best-practices/transient-faults)
- [Andrew Lock | Creating a source generator](https://andrewlock.net/series/creating-a-source-generator)
- [Martin Othamar | Mediator](https://github.com/martinothamar/Mediator/tree/main)
