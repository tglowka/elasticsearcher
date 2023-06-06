using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
