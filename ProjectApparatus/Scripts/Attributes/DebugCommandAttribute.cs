using System;

[AttributeUsage(AttributeTargets.Class)]
public class DebugCommandAttribute : Attribute
{
    public string Syntax { get; }

    public DebugCommandAttribute(string syntax)
    {
        Syntax = syntax;
    }
}
