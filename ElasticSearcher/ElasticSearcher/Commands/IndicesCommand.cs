using System.CommandLine;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class IndicesCommand : Command
{
    public static readonly string[] PossibleOperations =
    {
        "stats", "template", "exists", "get", "get-aliases", "refresh"
    };

    public IndicesCommand() : base("indices", "Get indices info.")
    {
        var indicesNames = new IndicesNamesArg();
        var operation = new OperationArg().FromAmong(PossibleOperations);
        AddArgument(operation);
        AddArgument(indicesNames);
        this.SetHandler(SetHandler, operation, indicesNames, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(string operation, string indexName, Uri uri)
    {
        Context.SetClient(uri);

        switch (operation)
        {
            case "stats":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.StatsAsync,
                    new IndicesStatsRequest(indices: indexName),
                    x => x
                );
                break;
            case "template":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.GetTemplateAsync,
                    new GetTemplateRequest(name: indexName),
                    x => x
                );
                break;
            case "exists":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.ExistsAsync,
                    new Elastic.Clients.Elasticsearch.IndexManagement.ExistsRequest(indices: indexName),
                    x => x
                );
                break;
            case "get":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.GetAsync,
                    (Indices)indexName,
                    x => x,
                    x => ConsoleExtension.PrintCollection(x.Indices, s => s.Key)
                );
                break;
            case "get-aliases":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.GetAsync,
                    (Indices)indexName,
                    x => x,
                    x => ConsoleExtension.PrintNestedCollection
                    (
                        x.Indices,
                        t => t.Key,
                        y => y.Value.Aliases,
                        z => z.Key
                    )
                );
                break;
            case "refresh":
                await OperationsHandler.HandleOperationAsync(
                    Context.Client.Indices.RefreshAsync,
                    (Indices)indexName,
                    x => x
                );
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation),
                    $"{nameof(IndicesCommand)}.{nameof(SetHandler)} - {operation} out of range.");
        }
    }
}

internal class OperationArg : Argument<string>
{
    public OperationArg() : base("operation", "Name of the operation.")
    {
    }
}

internal class IndicesNamesArg : Argument<string>
{
    public IndicesNamesArg() : base("indices names",
        "Name of the: one index, comma-separated indices or index name pattern (wildcard possible).")
    {
    }
}