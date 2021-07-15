using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using CommandFramework.Attributes;
using CommandFramework.Exceptions;

// TODO: write documentation

namespace CommandFramework.Parser
{
    public interface ICommandParser
    {
        string Prefix { get; set; }
        bool InterpretUserInput(string input);
    }

    public delegate bool CommandMethod(string userInput, ICommandParser parser);

    public struct CommandData
    {
        public MethodInfo Method { get; init; }
        public string Name { get; set; } 
        public string Description { get; set; }
    }

    public class CommandParser : ICommandParser
    {
        private readonly Dictionary<string, CommandData> _commands;

        public string Prefix { get; set; }

        internal CommandParser(Dictionary<string, CommandData> commands, string prefix = "!")
        {
            _commands = commands;

            Prefix = prefix;
        }

        public bool InterpretUserInput(string input)
        {
            if (input.Substring(0, Prefix.Length).Equals(Prefix))
            {
                var args = input.Split();
                var commandName = args[0][Prefix.Length..];

                if (_commands.ContainsKey(commandName))
                {
                    return (bool) _commands[commandName].Method.Invoke(null, new object[] {input, this});
                }
            }

            return false;
        }

        public CommandData GetCommandByName(string name) =>
            _commands.ContainsKey(name) ?
                _commands[name] :
                throw new CommandNotFoundException($"No command by name {name}")
                {
                    CommandName = name
                };

        public Dictionary<string, CommandData> GetCommands() =>
            _commands;

        public static IEnumerable<string> GetArgumentsFromUserInput(string input) =>
            input.Split().Skip(1);
    }

    public static class CommandParserFactory
    {
        // this function throws when it fails, maybe catch exception as inline and throw more specific one?
        public static bool CheckMethodIsValidCommand(MethodInfo method) =>
            method.IsStatic && method.CreateDelegate<CommandMethod>() is CommandMethod;

        public static CommandParser ConstructCommandParser<T>() where T : class
        {
            Dictionary<string, CommandData> commandDictionary = new();

            var commandType = typeof(T);
            var methods = commandType.GetMethods();

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<CommandAttribute>();

                if (attr is not null && CheckMethodIsValidCommand(method))
                {
                    commandDictionary.Add(attr.Name, new CommandData
                    {
                        Method = method,
                        Name = attr.Name,
                        Description = attr.Description
                    });
                }
            }

            return new CommandParser(commandDictionary);
        }
    }
}
