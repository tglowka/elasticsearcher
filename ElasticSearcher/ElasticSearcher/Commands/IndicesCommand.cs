using System.CommandLine;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class IndicesCommand : EssCommand
{
    private const string _name = "indices";
    private const string _description = "Get indices info.";

    public override string CLICommand => _name;
    public override string[] CLIPossibleOperations => new[] 
    {
        "stats", "template", "exists", "get", "get-aliases", "refresh"
    };

    public IndicesCommand() : base("indices", "Get indices info.")
    {
        var indicesNames = new IndicesNamesArg();
        var operation = new OperationArg().FromAmong(CLIPossibleOperations);
        AddArgument(operation);
        AddArgument(indicesNames);
        this.SetHandler(SetHandler, operation, indicesNames, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(string operation, string indexName, Uri uri)
    {
        var client = ConnectionContext.GetClient(uri);

        switch (operation)
        {
            case "stats":
                await OperationsHandler.HandleOperationAsync(
                    client.Indices.StatsAsync,
                    new IndicesStatsRequest(indices: indexName),
                    x => x
                );
                break;
            case "template":
                await OperationsHandler.HandleOperationAsync(
                    client.Indices.GetTemplateAsync,
                    new GetTemplateRequest(name: indexName),
                    x => x
                );
                break;
            case "exists":
                await OperationsHandler.HandleOperationAsync(
                    client.Indices.ExistsAsync,
                    new Elastic.Clients.Elasticsearch.IndexManagement.ExistsRequest(indices: indexName),
                    x => x
                );
                break;
            case "get":
                await OperationsHandler.HandleOperationAsync(
                    client.Indices.GetAsync,
                    (Indices)indexName,
                    x => x,
                    x => ConsoleExtension.PrintCollection(x.Indices, s => s.Key)
                );
                break;
            case "get-aliases":
                await OperationsHandler.HandleOperationAsync(
                    client.Indices.GetAsync,
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
                    client.Indices.RefreshAsync,
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

public class OperationArg : Argument<string>
{
    public OperationArg() : base("operation", "Name of the operation.")
    {
    }
}

public class IndicesNamesArg : Argument<string>
{
    public IndicesNamesArg() : base("indices names",
        "Name of the: one index, comma-separated indices or index name pattern (wildcard possible).")
    {
    }
}