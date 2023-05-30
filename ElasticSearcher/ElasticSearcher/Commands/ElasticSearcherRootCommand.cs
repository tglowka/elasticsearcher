using System.CommandLine;
using ElasticSearcher.Options;

namespace ElasticSearcher.Commands;

public class ElasticSearcherRootCommand : RootCommand
{
    public ElasticSearcherRootCommand() : base("The application for reading data and information from the Elasticsearch.")
    {
        AddGlobalOption(GlobalOptions.UriOption);
    }
}