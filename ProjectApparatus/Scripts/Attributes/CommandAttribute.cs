using System;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute : Attribute
{
    public string Syntax { get; }

    public CommandAttribute(string syntax)
    {
        Syntax = syntax;
    }
}
