using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Lambda;

public class BenchmarkService
{
    public async Task<BenchmarkResult> Run(string code)
    {


        try
        {

            var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            BenchmarkTemplate.CurrentCode = code;
            Summary summary;
#if DEBUG
            var config = new DebugInProcessConfig().WithArtifactsPath("/tmp").WithOptions(ConfigOptions.DisableLogFile);
            summary = BenchmarkRunner.Run<BenchmarkTemplate>(config);

#else
            summary = BenchmarkRunner.Run<BenchmarkTemplate>(ManualConfig
                    .Create(DefaultConfig.Instance)
                    .WithArtifactsPath("/tmp")
                    .WithOptions(ConfigOptions.DisableLogFile));

#endif
            var text = sw.ToString();
            Console.SetOut(originalOut);

            var result = new BenchmarkResult
            {
                ExecutionId = Guid.NewGuid(),
                Log = text,
                TimeTaken = summary.TotalTime
            };
            return await Task.FromResult(result);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.ToString());
            throw;
        }


    }

    public class BenchmarkTemplate
    {
        public static string CurrentCode { get; set; }
        private Script<object> _script;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _script = CSharpScript.Create(CurrentCode);
            _script.Compile();
        }

        [Benchmark]
        public async Task Benchmark() => await _script.RunAsync();
    }
}
