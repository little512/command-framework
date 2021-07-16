using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using CommandFramework.Attributes;
using CommandFramework.Exceptions;

namespace CommandFramework.Parser
{

    /// <summary>
    /// The delegate for command methods.
    /// </summary>
    /// <param name="userInput">The user's input that's being parsed</param>
    /// <param name="parser">The <see cref="ICommandParser"/> parsing the current command.</param>
    public delegate bool CommandMethod(string userInput, ICommandParser parser);

    /// <summary>
    /// An enum representing the case variance of the given command.
    /// </summary>
    public enum CommandCaseSensitivity
    {
        CaseInvariant,
        CaseSensitive
    }

    /// <summary>
    /// The interface for CommandData objects.
    /// </summary>
    public interface ICommandData
    {

        /// <summary>
        /// The method to invoke when the command is input.
        /// </summary>
        MethodInfo Method { get; init; }

        /// <summary>
        /// The name of the command. This is the name the command will go by when it's used by a user.
        /// <para>
        /// In other words, the name is what the user will input in order to run the command.
        /// </para>
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The description of the command.
        /// <para>
        /// This should contain information on the command in the context of a help menu.
        /// </para>
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The case variance of the given command.
        /// </summary>
        CommandCaseSensitivity CaseSensitivity { get; set; }
    }

    /// <summary>
    /// The default CommandData implementation. 
    /// <para>
    /// You may create your own implementation by making a struct or class that implements <see cref="ICommandData"/>.
    /// </para>
    /// </summary>
    public struct CommandData : ICommandData
    {
        public MethodInfo Method { get; init; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public CommandCaseSensitivity CaseSensitivity { get; set; }
    }

    /// <summary>
    /// The interface for the object which manages user input and parses it.
    /// </summary>
    public interface ICommandParser
    {

        /// <summary>
        /// The prefix of the commands the user will be inputting.
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// Interprets user input and returns a boolean indicating the success or failure of the command.
        /// </summary>
        /// <param name="input">The user's input that will be parsed.</param>
        bool InterpretUserInput(string input);
    }

    /// <summary>
    /// The default CommandParser implementation.
    /// <para>
    /// You may create your own implementation by making a class that implements <see cref="ICommandParser"/>.
    /// </para>
    /// </summary>
    public class CommandParser : ICommandParser
    {
        private readonly Dictionary<string, ICommandData> _commands;

        public string Prefix { get; set; }

        internal CommandParser(Dictionary<string, ICommandData> commands, string prefix = "!")
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

                if (_commands.ContainsKey(commandName.ToLower()))
                {
                    var command = _commands[commandName.ToLower()];

                    switch (command.CaseSensitivity)
                    {
                        case CommandCaseSensitivity.CaseInvariant:
                            return (bool) command.Method.Invoke(null, new object[] {input, this});

                        case CommandCaseSensitivity.CaseSensitive:
                            if (commandName == command.Name)
                                return (bool) command.Method.Invoke(null, new object[] {input, this});

                            break;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a command registered in the command container by name, or throws an exception if one is not found.
        /// </summary>
        /// <param name="name">The name of the command to get.</param>
        /// <exception cref="CommandNotFoundException"></exception>
        public ICommandData GetCommandByName(string name) =>
            _commands.ContainsKey(name) ?
                _commands[name] :
                throw new CommandNotFoundException($"No command by name {name}")
                {
                    CommandName = name
                };

        /// <summary>
        /// Gets the dictionary of registered commands.
        /// </summary>
        public Dictionary<string, ICommandData> GetCommands() =>
            _commands;

        /// <summary>
        /// A basic utility function which splits a user's input and returns everything after the first element.
        /// </summary>
        /// <param name="input">The user's input to be split.</param>
        public static IEnumerable<string> GetArgumentsFromUserInput(string input) =>
            input.Split().Skip(1);
    }

    /// <summary>
    /// A factory class which contains functions to construct a <see cref="CommandParser"/>.
    /// </summary>
    public static class CommandParserFactory
    {
        // this function throws when it fails, maybe catch exception as inline and throw more specific one?
        /// <summary>
        /// Verifies whether a reflected method meets the criteria of being a command method.
        /// </summary>
        /// <param name="method">The method to verify.</param>
        public static bool CheckMethodIsValidCommand(MethodInfo method) =>
            method.IsStatic && method.CreateDelegate<CommandMethod>() is CommandMethod;

        /// <summary>
        /// A method which constructs a new <see cref="CommandParser"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The command container to register functions with.
        /// <example>
        /// <para>
        /// Command container classes should contain static methods which match
        /// the type signature of <see cref="CommandMethod"/>. These methods
        /// will be referenced when commands are interpreted.
        /// </para>
        /// <para>
        /// Command container methods should also have the <see cref="CommandAttribute"/>
        /// attribute attached to them otherwise they will not be considered by the
        /// parser constructor.
        /// </para>
        /// </example>
        /// </typeparam>
        public static CommandParser ConstructCommandParser<T>() where T : class
        {
            Dictionary<string, ICommandData> commandDictionary = new();

            var commandType = typeof(T);
            var methods = commandType.GetMethods();

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<CommandAttribute>();

                if (attr is not null && CheckMethodIsValidCommand(method))
                {
                    commandDictionary.Add(attr.Name.ToLower(), new CommandData
                    {
                        Method = method,
                        Name = attr.Name,
                        Description = attr.Description,
                        CaseSensitivity = attr.CaseSensitivity
                    });
                }
            }

            return new CommandParser(commandDictionary);
        }
    }
}
