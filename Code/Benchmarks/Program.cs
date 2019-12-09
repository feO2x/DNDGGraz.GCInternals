using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, CreateDefaultConfig());
        }

        private static IConfig CreateDefaultConfig() =>
            DefaultConfig.Instance
                         .With(Job.Default.With(CoreRuntime.Core31))
                         .With(MemoryDiagnoser.Default);
    }
}