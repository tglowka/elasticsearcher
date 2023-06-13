using System.CommandLine;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Commands;
using System.Collections;
using ElasticSearcher.Options;
using Microsoft.VisualBasic;

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


    [Fact]
    public void CommandFactory_CreateRootCommand_AssertSubcommandsOptionsAndArguments()
    {
        //act
        var root = CommandFactory.CreateRootCommand();
        var subcommands = root.Subcommands;
        var options = root.Options;
        var arguments = root.Arguments;

        //assert
        Assert.Equal(5, subcommands.Count);
        Assert.Single(subcommands, x => x is PingCommand);
        Assert.Single(subcommands, x => x is ClusterCommand);
        Assert.Single(subcommands, x => x is IndicesCommand);
        Assert.Single(subcommands, x => x is DocCommand);
        Assert.Single(subcommands, x => x is InteractiveCommand);
        Assert.Equal(1, options.Count);
        Assert.Single(options, x => x is UriOption);
        Assert.Empty(arguments);
    }

    [Fact]
    public void CommandFactory_CreateRootCommandInteractive_AssertSubcommandsOptionsAndArguments()
    {
        //act
        var root = CommandFactory.CreateRootCommandInteractive();
        var subcommands = root.Subcommands;
        var options = root.Options;
        var arguments= root.Arguments;

        //assert
        Assert.Equal(5, subcommands.Count());
        Assert.Single(subcommands, x => x is PingCommand);
        Assert.Single(subcommands, x => x is ClusterCommand);
        Assert.Single(subcommands, x => x is IndicesCommand);
        Assert.Single(subcommands, x => x is DocCommand);
        Assert.Single(subcommands, x => x is SetConnectionCommand);
        Assert.Equal(1, options.Count);
        Assert.Single(options, x => x is UriOption);
        Assert.Empty(arguments);
    }

    [Fact]
    public void Cluster_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<ClusterCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Equal(1, arguments.Count);
        Assert.Single(arguments, x => x is OperationArg);
    }
    
    [Theory]
    [ClassData(typeof(CommandsAndOperations<ClusterCommand>))]
    public async Task Cluster_Succeeded(string commandAndOperation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync(commandAndOperation);

        //assert
        AssertOperationSucceeded(result);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<ClusterCommand>))]
    public async Task Cluster_Failed(string commandAndOperation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"{commandAndOperation} unknown");

        //assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Indices_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<IndicesCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Equal(2, arguments.Count);
        Assert.Single(arguments, x => x is OperationArg);
        Assert.Single(arguments, x => x is IndicesNamesArg);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<IndicesCommand>))]
    public async Task Indices_Succeeded(string commandAndOperation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"{commandAndOperation} {ElasticSearchFixture.Index}");

        //assert
        AssertOperationSucceeded(result);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<IndicesCommand>))]
    public async Task Indices_Failed(string commandAndOperation)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"{commandAndOperation} {ElasticSearchFixture.Index} unknown");

        //assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Doc_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<DocCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Equal(3, arguments.Count);
        Assert.Single(arguments, x => x is OperationArg);
        Assert.Single(arguments, x => x is IndexNameArg);
        Assert.Single(arguments, x => x is IdArg);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<DocCommand>))]
    public async Task Doc_Succeeded(string commandAndOperation)
    {
        //act
        var result =
            await _fixture.RootCommand.InvokeAsync($"{commandAndOperation} {ElasticSearchFixture.Index} {Guid.NewGuid()}");

        //assert
        AssertOperationSucceeded(result);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<DocCommand>))]
    public async Task Doc_Failed(string commandAndOperation)
    {
        //act
        var result =
            await _fixture.RootCommand.InvokeAsync($"{commandAndOperation} {ElasticSearchFixture.Index} {Guid.NewGuid()} unknown");

        //assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Ping_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<PingCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Empty(arguments);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<PingCommand>))]
    public async Task Ping_Succeeded(string command)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync(command);

        //assert
        AssertOperationSucceeded(result);
    }

    [Theory]
    [ClassData(typeof(CommandsAndOperations<PingCommand>))]
    public async Task Ping_Failed(string command)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"{command} unknown");

        //assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData("abcd://///")]
    [InlineData("http://localhost:99999")]
    [InlineData("a-b-c-d")]
    public async Task Ping_InvalidURI_Failed(string uri)
    {
        //act
        var result = await _fixture.RootCommand.InvokeAsync($"ping --uri {uri}");

        //assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void SetConnection_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<SetConnectionCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Equal(1, arguments.Count);
        Assert.Single(arguments, x => x is UriArgument);
    }
    
    [Fact]
    public void Interactive_Structure_Succeeded()
    {
        //act
        var (subcommands, options, arguments) = GetCommandStructure<InteractiveCommand>();

        //assert
        Assert.Empty(subcommands);
        Assert.Empty(options);
        Assert.Empty(arguments);
    }

    public class CommandsAndOperations<T> : IEnumerable<object[]>
    where T : EssCommand, new()
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var obj = new T();
            var command = obj.CLICommand;
            if (!obj.CLIPossibleOperations.Any())
            {
                yield return new object[] { $"{command}" };
            }
            else
            {
                foreach (var operation in obj.CLIPossibleOperations)
                {
                    yield return new object[] { $"{command} {operation}" };
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

    private (IReadOnlyList<Command> Subcommands, IReadOnlyList<Option> Options, IReadOnlyList<Argument> Arguments) GetCommandStructure<T>()
    where T : Command, new()
    {
        var root = new T();
        return (root.Subcommands, root.Options, root.Arguments);
    }
}