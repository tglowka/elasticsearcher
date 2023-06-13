using System.CommandLine;
using System.CommandLine.Parsing;

namespace ElasticSearcher.Options;

public class UriOption : Option<Uri>
{
    public UriOption() : base(
        name: "--uri",
        parseArgument: Parse,
        isDefault: true,
        description: "URI")
    {
    }

    private static Uri Parse(ArgumentResult result)
    {
        switch (result.Tokens)
        {
            case []:
                {
                    return new Uri("http://localhost:9200");
                }
            case [var uriString]:
                {
                    if (string.IsNullOrEmpty(uriString.Value))
                    {
                        result.ErrorMessage = "URI cannot be empty.";
                        return null!;
                    }

                    var created = Uri.TryCreate(uriString.Value, UriKind.Absolute, out var uri);

                    if (!created)
                    {
                        result.ErrorMessage = "URI is not well formed.";
                        return null!;
                    }

                    if (uri!.Scheme != Uri.UriSchemeHttp && uri!.Scheme != Uri.UriSchemeHttps)
                    {
                        result.ErrorMessage = "Scheme must be either HTTP or HTTPS.";
                        return null!;
                    }

                    return uri!;
                }
            default:
                {
                    result.ErrorMessage = "Too many arguments.";
                    return null!;
                }
        }
    }
}