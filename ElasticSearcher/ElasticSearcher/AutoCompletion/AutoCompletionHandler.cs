using System.Reflection;
using ElasticSearcher.Abstractions;

namespace ElasticSearcher.AutoCompletion;

internal record Completion(string Command, string[] Operations, string[] DynamicArgs);

internal class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
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
        
        //two words + trailing space or three words without a trailing space
        if (tokens.Length == 3)
        {
            return GetDynamicArgs(tokens[0], tokens[1], tokens[2]);
        }

        return null;

        Completion[] GetCompletions()
        => Assembly
            .GetAssembly(typeof(AutoCompletionHandler)).DefinedTypes
            .Where(x => x.IsSubclassOf(typeof(EssCommand)))
            .Select(x =>
            {
                var obj = Activator.CreateInstance(x.AsType()) as EssCommand;
                var command = obj.CLICommand;
                var operations = obj.CLIPossibleOperations;
                var dynamicArgs = obj.CLIGetDynamicArguments().Result;
                return new Completion(command, operations, dynamicArgs);
            })
            .ToArray();

        string[] GetCommands(string tokenCommand)
            => GetCompletions()
                .Where(x => x.Command.StartsWith(tokenCommand))
                .Select(x => x.Command).ToArray();

        string[] GetOperations(string tokenCommand, string tokenOperation)
            => GetCompletions()
                .SingleOrDefault(x => x.Command == tokenCommand)
                ?.Operations
                .Where(x => x.StartsWith(tokenOperation))
                .ToArray();
        
        string[] GetDynamicArgs(string tokenCommand, string tokenOperation, string tokenDynamicArg)
            => GetCompletions()
                .SingleOrDefault(x => x.Command == tokenCommand)
                ?.DynamicArgs
                .Where(x => x.StartsWith(tokenDynamicArg))
                .ToArray();
    }
}