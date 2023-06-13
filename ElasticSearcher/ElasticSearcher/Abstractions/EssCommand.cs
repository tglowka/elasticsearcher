using System.CommandLine;

namespace ElasticSearcher.Abstractions
{
    public abstract class EssCommand : Command
    {
        public abstract string CLICommand { get; }
        public abstract string[] CLIPossibleOperations { get; }
        public virtual Task<string[]> CLIGetDynamicArguments() => Task.FromResult(Array.Empty<string>());

        protected EssCommand(string name, string description) : base(name, description)
        {
        }
    }
}
