using System;

namespace EVIL.JSON.Deserialization
{
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
}