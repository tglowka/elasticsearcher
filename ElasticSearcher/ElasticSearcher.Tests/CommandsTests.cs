using System.CommandLine;
using ElasticSearcher.Commands;

namespace ElasticSearcher.Tests;

[Collection("Elasticsearch collection")]
public class CommandsTests : IDisposable
{
    private readonly ElasticSearchFixture _fixture;
    private readonly StringWriter _stringWriter;

    public CommandsTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    [Theory]
    [MemberData(nameof(ClusterOperations))]
    public async Task Cluster_PossibleOperations_Succeeded(string operation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"cluster {operation}");

        //assert
        AssertOperationSucceeded(result);
    }

    public static IEnumerable<object[]> ClusterOperations =>
        new ClusterCommand().CLIPossibleOperations.Select(x => new object[] { x });

    [Fact]
    public async Task Cluster_NotPossibleOperation_Failed()
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"cluster unknown");

        //assert
        Assert.Equal(1, result);
    }

    [Theory]
    [MemberData(nameof(IndicesOperations))]
    public async Task Indices_PossibleOperations_Succeeded(string operation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"indices {operation} {ElasticSearchFixture.Index}");

        //assert
        AssertOperationSucceeded(result);
    }

    public static IEnumerable<object[]> IndicesOperations =>
        IndicesCommand.PossibleOperations.Select(x => new object[] { x });

    [Fact]
    public async Task Indices_NotPossibleOperation_Failed()
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"indices unknown {ElasticSearchFixture.Index}");

        //assert
        Assert.Equal(1, result);
    }

    [Theory]
    [MemberData(nameof(DocsOperations))]
    public async Task Doc_PossibleOperations_Succeeded(string operation)
    {
        //act
        var result =
            await _fixture.RootCommand.InvokeAsync($"doc {operation} {ElasticSearchFixture.Index} {Guid.NewGuid()}");

        //assert
        AssertOperationSucceeded(result);
    }

    public static IEnumerable<object[]> DocsOperations =>
        DocCommand.PossibleOperations.Select(x => new object[] { x });

    [Fact]
    public async Task Doc_NotPossibleOperation_Failed()
    {
        //act
        var result =
            await _fixture.RootCommand.InvokeAsync($"doc unknown {ElasticSearchFixture.Index} {Guid.NewGuid()}");

        //assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Ping_PossibleOperationsSucceeded()
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync("ping");

        //assert
        AssertOperationSucceeded(result);
    }

    [Theory]
    [InlineData("abcd://///")]
    [InlineData("http://localhost:99999")]
    [InlineData("a-b-c-d")]
    public async Task Ping_InvalidURI_Failed(string uri)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"ping {uri}");

        //assert
        Assert.Equal(1, result);
    }

    public void Dispose()
    {
        _stringWriter.Flush();
    }

    private void AssertOperationSucceeded(int result)
    {
        Assert.Equal(0, result);
        Assert.DoesNotContain("Operation failed.", _stringWriter.ToString());
    }
}