using System.CommandLine;
using Elastic.Clients.Elasticsearch;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class DocCommand : EssCommand
{
    private const string _name = "doc";
    private const string _description = "Operations related to the documents.";

    public override string CLICommand => _name;
    public override string[] CLIPossibleOperations => new[]
    {
        "search", "exists"
    };

    public override async Task<string[]> CLIGetDynamicArguments() 
        => await GetCLIPossibleArguments();

    public DocCommand() : base(_name, _description)
    {
        var indexName = new IndexNameArg();
        var operation = new OperationArg().FromAmong(CLIPossibleOperations);
        var id = new IdArg();
        AddArgument(operation);
        AddArgument(indexName);
        AddArgument(id);

        this.SetHandler(SetHandler, operation, indexName, id, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(string operation, string indexName, string id, Uri uri)
    {
        var client = Context.GetClient(uri);
        switch (operation)
        {
            case "search":
                {
                    await OperationsHandler.HandleOperationAsync(
                        client.GetAsync<object>,
                        (IndexName)indexName,
                        (Id)id,
                        x => x.Source);
                    break;
                }
            case "exists":
                {
                    await OperationsHandler.HandleOperationAsync(
                        client.ExistsAsync,
                        (IndexName)indexName,
                        (Id)id,
                        x => x.Exists);
                    break;
                }
        }
    }

    private async Task<string[]> GetCLIPossibleArguments()
    {
        var indices = await Context.GetInteractiveClient().Indices.GetAsync("_all");

        if (indices.IsSuccess())
        {
            return indices.Indices.Select(x => x.Key.ToString()).ToArray();
        }

        return Array.Empty<string>();
    }
}

internal class IdArg : Argument<string>
{
    public IdArg() : base("id", "Id of the document to search for.")
    {
    }
}

internal class IndexNameArg : Argument<string>
{
    public IndexNameArg() : base("index name", "Index name.")
    {
    }
}