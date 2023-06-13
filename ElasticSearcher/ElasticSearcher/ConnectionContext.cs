using Elastic.Clients.Elasticsearch;

namespace ElasticSearcher;

public class ConnectionContext
{
    private static readonly Dictionary<string, ElasticsearchClient> Clients = new();
    private static ElasticsearchClient? InteractiveClient;
    private static bool IsInteractive => InteractiveClient is not null;
    private static string? InteractiveUriString;

    public static ElasticsearchClient GetInteractiveClient()
    {
        if (IsInteractive)
        {
            return InteractiveClient!;
        }
        else
        {
            throw new InvalidOperationException("Cannot get interactive client when not in the interactive mode.");
        }
    }

    public static string GetInteractiveUriString()
    {
        if (IsInteractive)
        {
            return InteractiveUriString!;
        }
        else
        {
            throw new InvalidOperationException("Cannot get interactive uri string when not in the interactive mode.");
        }
    }

    public static ElasticsearchClient GetClient(Uri uri)
    {
        if (IsInteractive)
        {
            return InteractiveClient!;
        }
        else
        {
            var clientExists = Clients.TryGetValue(uri.OriginalString, out var client);

            if (clientExists)
            {
                return client!;
            }
            else
            {
                client = new ElasticsearchClient(uri);
                Clients.Add(uri.OriginalString, client);
                return client;
            }
        }
    }

    internal static void SetClientInteractive(ElasticsearchClient client, Uri uri)
    {
        InteractiveClient = client;
        InteractiveUriString = uri.OriginalString;
    }
}