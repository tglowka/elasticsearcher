using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;

namespace ElasticSearcher;

public static class OperationsHandler
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static async Task HandleOperationAsync<TResponse, TSelector>(
        Func<CancellationToken, Task<TResponse>> operation,
        Func<TResponse, TSelector> selector,
        Action<TSelector>? printFunc = null,
        CancellationToken ct = default
    ) where TResponse : ElasticsearchResponse
        => HandleResult(await operation(ct), selector, printFunc);

    public static async Task HandleOperationAsync<TResponse, TRequest, TSelector>(
        Func<TRequest, CancellationToken, Task<TResponse>> operation,
        TRequest req,
        Func<TResponse, TSelector> selector,
        Action<TSelector>? printFunc = null,
        CancellationToken ct = default
    ) where TResponse : ElasticsearchResponse
        => HandleResult(await operation(req, ct), selector, printFunc);

    public static async Task HandleOperationAsync<TResponse, TRequest1, TRequest2, TSelector>(
        Func<TRequest1, TRequest2, CancellationToken, Task<TResponse>> operation,
        TRequest1 req1,
        TRequest2 req2,
        Func<TResponse, TSelector> selector,
        Action<TSelector>? printFunc = null,
        CancellationToken ct = default
    ) where TResponse : ElasticsearchResponse
        => HandleResult(await operation(req1, req2, ct), selector, printFunc);

    private static void HandleResult<T, TSelector>(
        T result,
        Func<T, TSelector> selector,
        Action<TSelector>? printFunc = null
    )
        where T : ElasticsearchResponse
    {
        if (!result.IsSuccess())
        {
            ConsoleExtension.WriteError($"Operation failed.{Environment.NewLine}" +
                                        $"ElasticsearchServerError: {result.ElasticsearchServerError}{Environment.NewLine}" +
                                        $"DebugInformation: {result.DebugInformation}");
        }
        else
        {
            printFunc ??= DefaultPrint;
            printFunc(selector(result));
        }
    }

    private static void DefaultPrint<T>(T arg)
    {
        var jsonString = JsonSerializer.Serialize(arg, Options);
        ConsoleExtension.WriteSuccess(jsonString);
    }
}