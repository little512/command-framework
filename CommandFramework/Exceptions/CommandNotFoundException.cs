using System;

namespace CommandFramework.Exceptions
{
    [System.Serializable]
    public class CommandNotFoundException : System.Exception
    {
        public string CommandName { get; set; }

        public CommandNotFoundException() { }
        public CommandNotFoundException(string message) : base(message) { }
        public CommandNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected CommandNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
