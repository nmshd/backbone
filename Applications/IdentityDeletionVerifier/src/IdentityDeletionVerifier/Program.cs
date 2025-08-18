// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Text;
using Backbone.IdentityDeletionVerifier.Commands;

Console.OutputEncoding = Encoding.UTF8;

var command = new RootCommand
{
    new InitCommand(),
    new CheckCommand()
};

return await command.Parse(args).InvokeAsync();
