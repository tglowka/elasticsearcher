namespace ElasticSearcher;

public static class ConsoleExtension
{
    public static void WriteInfo(string message)
    {
        SetColor(ConsoleColor.White);
        WriteMessageAndResetColor(message);
    }

    public static void WriteSuccess(string message)
    {
        SetColor(ConsoleColor.Green);
        WriteMessageAndResetColor(message);
    }

    public static void WriteWarning(string message)
    {
        SetColor(ConsoleColor.Yellow);
        WriteMessageAndResetColor(message);
    }

    public static void WriteError(string message)
    {
        SetColor(ConsoleColor.Red);
        WriteMessageAndResetColor(message);
    }

    public static void PrintCollection<TCollection, TItem>(
        IReadOnlyCollection<TCollection>? collection,
        Func<TCollection, TItem> selector
    )
    {
        if (collection is null || !collection.Any())
            return;

        var i = 1;

        foreach (var item in collection)
        {
            WriteSuccess($"{i++}. {selector(item)}");
        }
    }

    public static void PrintNestedCollection<TCollection, TItem, TNestedCollectionItem, G>(
        IEnumerable<TCollection>? collection,
        Func<TCollection, TItem> selector,
        Func<TCollection, IEnumerable<TNestedCollectionItem>?> nestedCollectionSelector,
        Func<TNestedCollectionItem, G> nestedSelector
    )
    {
        if (collection is null || !collection.Any())
            return;

        var i = 1;

        foreach (var item in collection)
        {
            WriteSuccess($"{i}. {selector(item)}");

            var j = 1;

            foreach (var nestedItem in nestedCollectionSelector(item))
            {
                WriteSuccess($"\t{i}.{j}. {nestedSelector(nestedItem)}");
                ++j;
            }

            ++i;
        }
    }

    private static void SetColor(ConsoleColor color)
        => Console.ForegroundColor = color;

    private static void WriteMessageAndResetColor(string message)
    {
        Console.WriteLine(message);
        Console.ResetColor();
    }
}