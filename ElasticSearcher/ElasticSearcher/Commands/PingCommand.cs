using System.CommandLine;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

internal class PingCommand : Command
{
    public PingCommand() : base("ping", "Test the reachability of the Elasticsearch node.")
    {
        this.SetHandler(SetHandler, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(Uri uri)
    {
        Context.SetClient(uri);
        await OperationsHandler.HandleOperationAsync(Context.Client.PingAsync, x => x.IsValidResponse);
    }
}