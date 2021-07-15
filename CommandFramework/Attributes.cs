using System;

namespace CommandFramework.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public CommandAttribute() { }
    }
}
