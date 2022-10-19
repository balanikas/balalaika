namespace BlazorApp.Data;

using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

public class BenchmarkService
{
    public async Task<BenchmarkResult> Run(string code)
    {
        var options = ScriptOptions.Default.WithImports("System"); 
        Stopwatch watch = Stopwatch.StartNew();
        try   {
            var evaluationResult = await CSharpScript.EvaluateAsync(code, options);
            
        }
        catch (CompilationErrorException e)
        {
            Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
        }

        var result = new BenchmarkResult{
            ExecutionId = Guid.NewGuid(),
            TimeTaken = watch.Elapsed
            
        };

        return await Task.FromResult(result);
    }
}

public class BenchmarkResult {
    public Guid ExecutionId { get; set;}
    public TimeSpan TimeTaken { get; set;}
}