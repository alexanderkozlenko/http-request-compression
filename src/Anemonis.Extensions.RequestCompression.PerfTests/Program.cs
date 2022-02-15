using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;

var config = ManualConfig.CreateEmpty()
    .AddLogger(new ConsoleLogger(false, ConsoleLogger.CreateGrayScheme()))
    .AddExporter(MarkdownExporter.GitHub)
    .AddDiagnoser(new MemoryDiagnoser(new(false)))
    .AddColumn(StatisticColumn.OperationsPerSecond)
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddJob(Job.Default
        .WithWarmupCount(1)
        .WithIterationTime(TimeInterval.FromMilliseconds(250))
        .WithMinIterationCount(15)
        .WithMaxIterationCount(20)
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithGcMode(new()
        {
            Server = true,
        })
        .WithStrategy(RunStrategy.Throughput))
    .WithSummaryStyle(new(CultureInfo.InvariantCulture, false, SizeUnit.B, TimeUnit.Microsecond));

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()).Run(args, config);
