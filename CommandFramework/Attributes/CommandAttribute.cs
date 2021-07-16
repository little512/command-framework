using System;

using CommandFramework.Parser;

namespace CommandFramework.Attributes
{
    /// <summary>
    /// The attribute which defines a command in a command container class.
    /// <para>
    /// This attribute must be attached to a method in an attribute container class,
    /// otherwise it will not be considered by relevant command parser constructors.
    /// </para>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the command. This is what the user will type preceded by the prefix
        /// defined in the parser.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the command. This is likely what will be used in a help menu
        /// to describe the utility of the command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The case variance of the given command.
        /// </summary>
        public CommandCaseSensitivity CaseSensitivity { get; set; } = CommandCaseSensitivity.CaseInvariant;
        
        public CommandAttribute() { }
    }
}
