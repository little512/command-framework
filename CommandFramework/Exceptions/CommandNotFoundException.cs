using System;

namespace CommandFramework.Exceptions
{
    /// <summary>
    /// An exception which is thrown when a command is not found by functions that retrieve commands.
    /// </summary>
    [System.Serializable]
    public class CommandNotFoundException : System.Exception
    {
        /// <summary>
        /// The name of the command which was attempted to be retrieved.
        /// </summary>
        public string CommandName { get; set; }

        public CommandNotFoundException() { }
        public CommandNotFoundException(string message) : base(message) { }
        public CommandNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected CommandNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
