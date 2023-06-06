using System.CommandLine;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

internal class PingCommand : EssCommand
{
    private const string _name = "ping";
    private const string _description = "Test the reachability of the Elasticsearch node.";

    public override string CLIName => _name;
    public override string[] CLIPossibleOperations => Array.Empty<string>();

    public PingCommand() : base(_name, _description)
    {
        this.SetHandler(SetHandler, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(Uri uri)
    {
        Context.SetClient(uri);
        await OperationsHandler.HandleOperationAsync(Context.Client.PingAsync, x => x.IsValidResponse);
    }
}