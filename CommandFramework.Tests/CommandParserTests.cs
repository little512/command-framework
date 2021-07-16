using System;
using Xunit;

using CommandFramework.Parser;
using CommandFramework.Attributes;
using CommandFramework.Exceptions;

namespace CommandFramework.Tests
{
    public class CommandContainer
    {
        [Command(Name = "test", Description = "This is a test function")]
        public static bool TestCommand(string input, ICommandParser parser)
        {
            Console.WriteLine($"Test command: {input}\nThe prefix is: {parser.Prefix}");

            return true;
        }

        [Command(Name = "help", Description = "Shows this message")]
        public static bool HelpCommand(string _, ICommandParser parser)
        {
            var p = parser as CommandParser;
            var commands = p.GetCommands();

            Console.WriteLine("List of commands:");
            foreach (var command in commands.Values)
            {
                Console.WriteLine($"- {p.Prefix}{command.Name}");
                Console.WriteLine($"\t{command.Description}");
            }

            return true;
        }

        [Command(Name = "CaseSensitive", Description = "This command is case sensitive", CaseSensitivity = CommandCaseSensitivity.CaseSensitive)]
        public static bool CaseSensitiveCommand(string input, ICommandParser _)
        {
            Console.WriteLine($"Case preserved input: {input}");

            return true;
        }
    }

    // TODO: write more tests

    public class CommandFramework_CommandParser
    {
        private readonly CommandParser commands;

        public CommandFramework_CommandParser()
        {
            commands = CommandParserFactory.ConstructCommandParser<CommandContainer>();
        }

        [Theory]
        [InlineData("!test")]
        [InlineData("!help")]
        [InlineData("!CaseSensitive")]
        public void CommandFramework_CommandParser_InterpretUserInput(string input)
        {
            Assert.True(commands.InterpretUserInput(input) == CommandResult.CommandSucceeded);
        }

        [Theory]
        [InlineData("!", "!test")]
        [InlineData("!!", "!!test")]
        [InlineData("a.", "a.test")]
        [InlineData("", "test")]
        public void CommandFramework_CommandParser_ChangePrefix(string prefix, string input)
        {
            var oldPrefix = commands.Prefix;

            commands.Prefix = prefix;
            Assert.True(commands.InterpretUserInput(input) == CommandResult.CommandSucceeded);

            commands.Prefix = oldPrefix;
        }
        
        [Theory]
        [InlineData("test")]
        public void CommandFramework_CommandParser_GetCommandByName(string commandName)
        {
            var command = commands.GetCommandByName(commandName);

            Assert.True(command.Name == commandName);
        }

        [Theory]
        [InlineData("nonsense")]
        public void CommandFramework_CommandParser_EnsureWrongNameThrows(string commandName)
        {
            ICommandData command;

            Assert.Throws<CommandNotFoundException>(() => command = commands.GetCommandByName(commandName));
        }

        [Theory]
        [InlineData("!casesensitive")]
        [InlineData("!Casesensitive")]
        [InlineData("!caseSensitive")]
        public void CommandFramework_CommandParser_EnsureCaseSensitiveCommandsFail(string input)
        {
            Assert.True(commands.InterpretUserInput(input) == CommandResult.CommandNotFound);
        }
    }
}
