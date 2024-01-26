using System.Globalization;
using System.Text;
using EVIL.Lexical;

namespace EVIL.JSON.Deserialization
{
    internal class JsonLexer
    {
        private const string HexAlphabet = "abcdefABCDEF0123456789";

        private int _line = 1;
        private int _column = 1;
        private int _position;

        private string _data = string.Empty;

        private char Character
        {
            get
            {
                if (_position >= _data.Length)
                    return (char)0;

                return _data[_position];
            }
        }

        public int Line => _line;
        public int Column => _column;

        public JsonToken Token { get; private set; }

        public void Load(string data)
        {
            _line = 1;
            _column = 1;
            _position = 0;

            _data = data;

            Token = JsonToken.EOF;
        }

        public JsonToken NextToken()
        {
            SkipWhitespace();

            switch (Character)
            {
                case '{':
                    Token = JsonToken.LBrace with { Line = _line, Column = _column };
                    Advance();
                    break;

                case '}':
                    Token = JsonToken.RBrace with { Line = _line, Column = _column };
                    Advance();
                    break;

                case '[':
                    Token = JsonToken.LBracket with { Line = _line, Column = _column };
                    Advance();
                    break;

                case ']':
                    Token = JsonToken.RBracket with { Line = _line, Column = _column };
                    Advance();
                    break;

                case ',':
                    Token = JsonToken.Comma with { Line = _line, Column = _column };
                    Advance();
                    break;

                case ':':
                    Token = JsonToken.Colon with { Line = _line, Column = _column };
                    Advance();
                    break;

                case (char)0:
                    Token = JsonToken.EOF with { Line = _line, Column = _column };
                    break;
                
                default:
                {
                    if (Character == '-' || char.IsDigit(Character))
                    {
                        Token = GetNumber();
                    }
                    else if (Character == '"')
                    {
                        Token = GetString();
                    }
                    else if (char.IsLetter(Character))
                    {
                        Token = GetConstant();
                    }
                    else
                    {
                        throw new JsonParsingException($"Unexpected character '{Character}'.", _line, _column);
                    }

                    break;
                }
            }

            return Token;
        }

        private JsonToken GetNumber()
        {
            var (line, col) = (_line, _column);

            var value = "";

            if (Character == '-')
            {
                value += Character;
                Advance();
            }

            if (!char.IsDigit(Character))
            {
                throw new JsonParsingException($"Expected a digit [0-9], found '{Character}'.", _line, _column);
            }

            if (char.IsBetween(Character, '1', '9'))
            {
                while (char.IsDigit(Character))
                {
                    value += Character;
                    Advance();
                }
            }
            else
            {
                value += Character;
                Advance();
            }

            if (Character == '.')
            {
                value += ".";
                Advance();

                if (!char.IsDigit(Character))
                {
                    throw new JsonParsingException($"Expected a digit [0-9], found '{Character}'.", _line, _column);
                }

                while (char.IsDigit(Character))
                {
                    value += Character;
                    Advance();
                }
            }

            if (Character == 'e' || Character == 'E')
            {
                value += Character;
                Advance();

                if (Character == '+' || Character == '-')
                {
                    value += Character;
                    Advance();
                }

                if (!char.IsDigit(Character))
                {
                    throw new JsonParsingException($"Expected '+', '-', or a digit [0-9], found '{Character}'.", _line,
                        _column);
                }

                while (char.IsDigit(Character))
                {
                    value += Character;
                    Advance();
                }
            }

            return new JsonToken(line, col, value, JsonTokenType.Number);
        }

        private JsonToken GetString()
        {
            var (line, col) = (_line, _column);

            var str = string.Empty;

            Advance();

            while (Character != '"')
            {
                if (Character == (char)0)
                    throw new JsonParsingException("Unterminated string.", line, col);

                if (Character == '\\')
                {
                    Advance();

                    switch (Character)
                    {
                        case '"':
                            str += '"';
                            break;
                        case '\\':
                            str += '\\';
                            break;
                        case '/':
                            str += '/';
                            break;
                        case 'b':
                            str += '\b';
                            break;
                        case 'f':
                            str += '\f';
                            break;
                        case 'n':
                            str += '\n';
                            break;
                        case 'r':
                            str += '\r';
                            break;
                        case 't':
                            str += '\t';
                            break;
                        case 'u':
                            Advance();
                            str += GetUnicodeSequence();
                            continue;
                        default:
                            throw new JsonParsingException("Unrecognized escape sequence.", _line, _column);
                    }

                    Advance();
                    continue;
                }

                str += Character;
                Advance();
            }

            Advance();
            return new JsonToken(line, col, str, JsonTokenType.String);
        }

        private char GetUnicodeSequence()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < 4; i++)
            {
                if (HexAlphabet.Contains(Character))
                {
                    sb.Append(Character);
                    Advance();
                }
                else
                {
                    throw new LexerException(
                        "Invalid universal character code.",
                        _line, _column
                    );
                }
            }

            return (char)int.Parse(sb.ToString(), NumberStyles.HexNumber);
        }

        private JsonToken GetConstant()
        {
            var constant = "";
            var (line, col) = (_line, _column);

            while (char.IsLetter(Character))
            {
                constant += Character;
                Advance();
            }

            switch (constant)
            {
                case "true":
                    return JsonToken.True with { Line = line, Column = col };

                case "false":
                    return JsonToken.False with { Line = line, Column = col };

                case "null":
                    return JsonToken.Null with { Line = line, Column = col };

                default: throw new JsonParsingException($"Unexpected constant '{constant}'.", line, col);
            }
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(Character))
            {
                Advance();
            }
        }

        private void Advance()
        {
            if (Character == '\n')
            {
                _line++;
                _column = 1;
            }

            _position++;
        }
    }
}