using System.CommandLine;
using System.CommandLine.Parsing;
using Elastic.Clients.Elasticsearch;
using ElasticSearcher.Abstractions;

namespace ElasticSearcher.Commands;

public class SetConnectionCommand : EssCommand
{
    private const string _name = "set-connection";
    private const string _description = "Set the connection URI to the Elasticsearch.";

    public override string CLICommand => _name;
    public override string[] CLIPossibleOperations => Array.Empty<string>();

    public SetConnectionCommand() : base(_name, _description)
    {
        var uriArg = new UriArgument();
        AddArgument(uriArg);
        this.SetHandler(SetHandler, uriArg);
    }

    private static async Task SetHandler(Uri uri)
    {
        var testClient = new ElasticsearchClient(uri);
        var result = await testClient.PingAsync();
        
        if(result.IsValidResponse)
        {
            ConnectionContext.SetClientInteractive(testClient , uri);
        }
        else
        {
            ConsoleExtension.WriteError("The Elasticsearch node is not reachable for given URI.");
        }
    }
}

public class UriArgument : Argument<Uri>
{
    public UriArgument() : base(
        name: "uri",
        parse: Parse,
        isDefault: true,
        description: "URI"
    )
    {
    }

    private static Uri Parse(ArgumentResult result)
    {
        switch (result.Tokens)
        {
            case []:
                {
                    result.ErrorMessage = "URI cannot be empty.";
                    return null!;
                }
            case [var uriString]:
                {
                    if (string.IsNullOrEmpty(uriString.Value))
                    {
                        result.ErrorMessage = "URI cannot be empty.";
                        return null!;
                    }

                    var created = Uri.TryCreate(uriString.Value, UriKind.Absolute, out var uri);

                    if (!created)
                    {
                        result.ErrorMessage = "URI is not well formed.";
                        return null!;
                    }

                    if (uri!.Scheme != Uri.UriSchemeHttp && uri!.Scheme != Uri.UriSchemeHttps)
                    {
                        result.ErrorMessage = "Scheme must be either HTTP or HTTPS.";
                        return null!;
                    }

                    return uri!;
                }
            default:
                {
                    result.ErrorMessage = "Too many arguments.";
                    return null!;
                }
        }
    }
}