using System.CommandLine;
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
