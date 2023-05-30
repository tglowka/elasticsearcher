using System.CommandLine;

namespace ElasticSearcher;

class Program
{
    static async Task<int> Main(string[] args)
        => await CommandFactory.CreateRootCommand().InvokeAsync(args);
}