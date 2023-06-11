using System.Reflection;
using ElasticSearcher.Abstractions;

namespace ElasticSearcher.AutoCompletion;

internal class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        var commands = Assembly
            .GetAssembly(typeof(AutoCompletionHandler)).DefinedTypes
            .Where(x => x.IsSubclassOf(typeof(EssCommand)))
            .Select(x =>
            {
                var obj = Activator.CreateInstance(x.AsType()) as EssCommand;
                var name = obj.CLIName;
                var operations = obj.CLIPossibleOperations;
                //var results = operations.Select(op => $"{name} {op}");
                return new { Name = name, Operations = operations };
            })
            .ToArray();

        //handles null, empty string or white spaces string
        if (string.IsNullOrEmpty(text.Trim()))
        {
            return GetCommands(string.Empty);
        }

        var tokens = text.Trim()
            .Split(' ')
            .Where(x => !string.IsNullOrEmpty(x)) //remove multiple spaces between words
            .ToArray();

        var IsSpaceEnd = text.LastOrDefault() == ' ';
        if (IsSpaceEnd)
        {
            tokens = tokens.Concat(new[] { string.Empty }).ToArray();
        }

        //one word without leading trailing
        if (tokens.Length == 1)
        {
            return GetCommands(tokens[0]);
        }

        //one word + trailing space or two words without a trailing space
        if (tokens.Length == 2)
        {
            return GetOperations(tokens[0], tokens[1]);
        }

        return null;

        string[] GetCommands(string tokenCommand)
            => commands
                .Where(x => x.Name.StartsWith(tokenCommand))
                .Select(x => x.Name).ToArray();

        string[] GetOperations(string tokenCommand, string tokenOperation)
            => commands
                .SingleOrDefault(x => x.Name == tokenCommand)
                ?.Operations
                .Where(x => x.StartsWith(tokenOperation))
                .ToArray();
    }
}