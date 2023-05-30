using Elastic.Clients.Elasticsearch;

namespace ElasticSearcher;

public class Context
{
    public static ElasticsearchClient Client { get; private set; }

    public static void SetClient(Uri uri)
        => Client = new ElasticsearchClient(uri);
}