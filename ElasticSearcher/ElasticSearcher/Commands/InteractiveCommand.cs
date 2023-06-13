using System.CommandLine;
using Elastic.Clients.Elasticsearch;
using ElasticSearcher.Abstractions;
using ElasticSearcher.AutoCompletion;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

internal class InteractiveCommand : EssCommand
{
    private const string _name = "in";
    private const string _description = "Start interactive mode";

    public override string CLICommand => _name;
    public override string[] CLIPossibleOperations => Array.Empty<string>();

    public InteractiveCommand() : base(_name, _description)
    {
        this.SetHandler(SetHandler, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(Uri uri)
    {
        var testClient = new ElasticsearchClient(uri);
        var result = await testClient.PingAsync();

        if (result.IsValidResponse)
        {
            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            ConnectionContext.SetClientInteractive(testClient, uri);

            while (true)
            {
                string input = ReadLine.Read($"ess ({ConnectionContext.GetInteractiveUriString()})>>> ");
                await CommandFactory.CreateRootCommandInteractive().InvokeAsync(input);
            }
        }
        else
        {
            ConsoleExtension.WriteError("The Elasticsearch node is not reachable for given URI.");
        }
    }
}
