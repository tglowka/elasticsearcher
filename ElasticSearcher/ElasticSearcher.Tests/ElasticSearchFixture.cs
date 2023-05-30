using System.CommandLine;
using AutoFixture;

namespace ElasticSearcher.Tests;

[CollectionDefinition("Elasticsearch collection")]
public class ElasticSearchCollection : ICollectionFixture<ElasticSearchFixture>
{
}

public class ElasticSearchFixture : IAsyncLifetime
{
    private Fixture Fixture { get; set; }

    public const string Index = "test-documents";
    public RootCommand RootCommand { get; private set; }

    public Task InitializeAsync()
    {
        Fixture = new Fixture();
        RootCommand = CommandFactory.CreateRootCommand();

        Context.SetClient(new Uri("http://localhost:9200"));

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}