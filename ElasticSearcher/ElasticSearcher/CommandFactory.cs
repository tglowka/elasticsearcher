using ElasticSearcher.Commands;

namespace ElasticSearcher;

public class CommandFactory
{
    public static ElasticSearcherRootCommand CreateRootCommand()
    {
        var rootCommand = new ElasticSearcherRootCommand();
        rootCommand.AddCommand(new PingCommand());
        rootCommand.AddCommand(new ClusterCommand());
        rootCommand.AddCommand(new IndicesCommand());
        rootCommand.AddCommand(new DocCommand());
        rootCommand.AddCommand(new InteractiveCommand());

        return rootCommand;
    }
}