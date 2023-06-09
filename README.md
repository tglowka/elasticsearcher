# Overview

This is a repository of a CLI tool called ElasticSearcher. The main purpose of the tool is to quick-querying the Elasticsearch in order to check whether a document of a given id exists, show the document content, list all the indices for a given pattern (e.g. `tmp*`), list corresponding aliases etc. For now the tool is very limited and provides just a few commands to play with. However, I plan to add more and more commands to the tool over time and I would be greatful for any contribution.

**Important: The tool works with Elasticsearch version 8.**

# Installation

The tool can be installed from the NuGet - https://www.nuget.org/packages/BaldSolutions.ElasticSearcher

# Usage

Once the tool is installed you can use the `ess` command to start querying the Elasticsearch. To check available commands, arguments and options the `--help`, `-h` or `-?` options show the tool description. Try to run:

```
ess --help
```

and you should see:
![image](https://github.com/tglowka/elasticsearcher/assets/38429856/2a7699de-db5e-4211-af01-5e82af7524d5)
Each subsequent command (e.g. `ping` or `doc`) also accepts `--help` option for further explanation:

```
ess doc --help
```

![image](https://github.com/tglowka/elasticsearcher/assets/38429856/e2e22db9-d5f6-4255-940e-73f510d4b462)

# Interactive Mode

The tool has an option called `interactive` (kind of a REPL). The interactive mode has the same functionalities as the ones mentioned in the [Examples](#examples) (no `ess` needed) sectiond and it has autocompletion feature. The only difference is that it does not apply the `--uri` option when in the interactive mode. To change the URI you must use the `set-connection` command.

## Autocompletion

For now the autocompletion feature works when the `Tab` key is pressed. It suggests the command name and the operation (if applicable, e.g. the `ping` command has only the command part - `ping` and no operation part whereas the `doc` command has command part - `doc` and operation part - `exists|search`). Moreover, for the `doc` command has the autocompletion for the `index name` - type `doc exists ` and press tab and you shold get index name as completion.

# Examples

## List indices

```
ess indices get tm*
```

![image](https://github.com/tglowka/elasticsearcher/assets/38429856/8a3d8bba-68cf-47c1-a63e-75b90570c622)

```
ess indices get *
```

![image](https://github.com/tglowka/elasticsearcher/assets/38429856/115b38e7-5692-43fd-8109-d7156abc49ad)

## List aliases

```
ess indices get-aliases tm*
```

![image](https://github.com/tglowka/elasticsearcher/assets/38429856/b26de425-d660-4e8f-a959-c87043c1777c)

## Search for a document

```
ess doc search tmp 581fbogByNfQfwWmNPFh
```

![image](https://github.com/tglowka/elasticsearcher/assets/38429856/57a334b1-345b-4ce9-9fd8-951b6a0e6890)

## Combine the tool with VisualStudio Code

```
ess doc search tmp 581fbogByNfQfwWmNPFh | code -
```

This command opens the VisualStudio Code with the output of the `ess` command.

# Contribution

I would be grateful for showing me any suggestion/idea/bug or other limitation/flaw. If there is anything you want to add regarding the tool please create an Issue or create a PR.
