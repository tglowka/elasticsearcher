using System.CommandLine;
using ElasticSearcher.Abstractions;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class ClusterCommand : EssCommand
{
    private const string _name = "cluster";
    private const string _description = "Get cluster info.";

    public override string CLICommand => _name;

    public override string[] CLIPossibleOperations => new[]
    {
        "health", "pending-tasks", "get-settings"
    };
    
    public ClusterCommand() : base(_name, _description)
    {
        var operation = new OperationArg().FromAmong(CLIPossibleOperations);
        AddArgument(operation);
        this.SetHandler(SetHandler, operation, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(string operation, Uri uri)
    {
        var client = Context.GetClient(uri);
        switch (operation)
        {
            case "health":
                await OperationsHandler.HandleOperationAsync(client.Cluster.HealthAsync, x => x);
                break;
            case "pending-tasks":
                await OperationsHandler.HandleOperationAsync(client.Cluster.PendingTasksAsync, x => x);
                break;
            case "get-settings":
                await OperationsHandler.HandleOperationAsync(client.Cluster.GetSettingsAsync, x => x);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation),
                    $"{nameof(ClusterCommand)}.{nameof(SetHandler)} - {operation} out of range.");
        }
    }
}