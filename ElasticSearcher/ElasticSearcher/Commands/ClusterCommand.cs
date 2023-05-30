using System.CommandLine;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class ClusterCommand : Command
{
    public static readonly string[] PossibleOperations =
    {
        "health", "pending-tasks", "get-settings"
    };

    public ClusterCommand() : base("cluster", "Get cluster info.")
    {
        var operation = new OperationArg().FromAmong(PossibleOperations);
        AddArgument(operation);
        this.SetHandler(SetHandler, operation, GlobalOptions.UriOption);
    }

    private static async Task SetHandler(string operation, Uri uri)
    {
        Context.SetClient(uri);
        switch (operation)
        {
            case "health":
                await OperationsHandler.HandleOperationAsync(Context.Client.Cluster.HealthAsync, x => x);
                break;
            case "pending-tasks":
                await OperationsHandler.HandleOperationAsync(Context.Client.Cluster.PendingTasksAsync, x => x);
                break;
            case "get-settings":
                await OperationsHandler.HandleOperationAsync(Context.Client.Cluster.GetSettingsAsync, x => x);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation),
                    $"{nameof(ClusterCommand)}.{nameof(SetHandler)} - {operation} out of range.");
        }
    }
}