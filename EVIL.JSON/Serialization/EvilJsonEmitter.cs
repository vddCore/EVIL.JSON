using System.IO;

namespace EVIL.JSON.Serialization
{
    internal class EvilJsonEmitter
    {
        private readonly TextWriter _writer;
        private int _indentLevel;
        
        public EvilJsonEmitter(TextWriter writer)
        {
            _writer = writer;
        }

        public void EmitNumber(double d)
        {
            _writer.Write(d);
        }
        
        public void EmitString(string s)
        {
            s = s.Replace("\\", "\\\\")
                 .Replace("\"", "\\\"");
            
            _writer.Write($"\"{s}\"");
        }
        
        public void EmitBoolean(bool b)
        {
            _writer.Write(b.ToString().ToLower());
        }

        public void EmitNull()
            => _writer.Write("null");

        public void EmitSpace()
            => _writer.Write(" ");

        public void EmitNewLine()
            => _writer.WriteLine();

        public void EmitLeftBrace()
            => _writer.Write('{');

        public void EmitRightBrace()
            => _writer.Write('}');

        public void EmitLeftBracket()
            => _writer.Write('[');

        public void EmitRightBracket()
            => _writer.Write(']');

        public void EmitComma()
            => _writer.Write(',');

        public void EmitColon()
            => _writer.Write(':');
        
        public void EmitIndentation()
        {
            for (var i = 0; i < _indentLevel; i++)
            {
                _writer.Write(' ');
            }
        }

        public void Indent()
            => _indentLevel += 2;

        public void Unindent()
        {
            _indentLevel -= 2;
            
            if (_indentLevel < 0)
            {
                _indentLevel = 0;
            }
        }

        public void EmitKey(string key)
        {
            EmitString(key);
            EmitColon();
        }
    }
}
