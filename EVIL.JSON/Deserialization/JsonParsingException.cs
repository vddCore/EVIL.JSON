namespace EVIL.JSON.Deserialization;

using System;

public class JsonParsingException : Exception
{
    public int Line { get; }
    public int Column { get; }

    internal JsonParsingException(string message, int line, int column) 
        : base(message)
    {
        Line = line;
        Column = column;
    }
}