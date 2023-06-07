using System.CommandLine;

namespace ElasticSearcher.Abstractions
{
    public abstract class EssCommand : Command
    {
        public abstract string CLIName { get; }
        public abstract string[] CLIPossibleOperations { get; }

        protected EssCommand(string name, string description) : base(name, description)
        {
        }
    }
}
