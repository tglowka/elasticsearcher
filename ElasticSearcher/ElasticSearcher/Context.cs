using Elastic.Clients.Elasticsearch;

namespace ElasticSearcher;

public class Context
{
    private static bool IsInteractive;

    public static ElasticsearchClient Client { get; private set; }
    public static string Uri { get; private set; }

    public static void SetClient(Uri uri)
    {
        if (!IsInteractive)
        {
            Client = new ElasticsearchClient(uri);
            Uri = uri.OriginalString;
        }
    }

    public static void SetClientInteractive(Uri uri)
    {
        IsInteractive = true;
        Client = new ElasticsearchClient(uri);
        Uri = uri.OriginalString;
    }
}