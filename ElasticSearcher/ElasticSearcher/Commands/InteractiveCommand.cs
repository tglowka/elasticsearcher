using System.CommandLine;
using System.Reflection;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

internal class InteractiveCommand : EssCommand
{
    private const string _name = "in";
    private const string _description = "Start interactive mode";

    public override string CLIName => _name;
    public override string[] CLIPossibleOperations => Array.Empty<string>();

    public InteractiveCommand() : base(_name, _description)
    {
        this.SetHandler(SetHandler, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(Uri uri)
    {
        ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

        Context.SetClient(uri);

        while (true)
        {
            string input = ReadLine.Read("ess>>> ");
            await CommandFactory.CreateRootCommand().InvokeAsync(input);
        }
    }
}

internal class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        var commands = Assembly
            .GetAssembly(typeof(AutoCompletionHandler)).DefinedTypes
            .Where(x => x.IsSubclassOf(typeof(EssCommand)))
            .Select(x =>
            {
                var obj = (Activator.CreateInstance(x.AsType()) as EssCommand);
                var name = obj.CLIName;
                var operations = obj.CLIPossibleOperations;
                var results = operations.Select(op => $"{name} {op}");
                return results;
            })
            .SelectMany(x => x)
            .ToArray();

        if (string.IsNullOrEmpty(text))
        {
            return commands;
        }
        else
            return null;
    }
}