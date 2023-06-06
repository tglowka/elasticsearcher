using System.CommandLine;
using Elastic.Clients.Elasticsearch;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class DocCommand : EssCommand
{
    private const string _name = "doc";
    private const string _description = "Operations related to the documents.";

    public override string CLIName => _name;
    public override string[] CLIPossibleOperations => new[]
    {
        "search", "exists"
    };

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
        Context.SetClient(uri);
        switch (operation)
        {
            case "search":
                {
                    await OperationsHandler.HandleOperationAsync(
                        Context.Client.GetAsync<object>,
                        (IndexName)indexName,
                        (Id)id,
                        x => x.Source);
                    break;
                }
            case "exists":
                {
                    await OperationsHandler.HandleOperationAsync(
                        Context.Client.ExistsAsync,
                        (IndexName)indexName,
                        (Id)id,
                        x => x.Exists);
                    break;
                }
        }
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