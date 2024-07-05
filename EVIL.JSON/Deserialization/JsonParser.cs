namespace EVIL.JSON.Deserialization;

using System.Linq;
using Ceres.ExecutionEngine.Collections;
using Ceres.ExecutionEngine.TypeSystem;

internal class JsonParser
{
    private JsonLexer _lexer;

    public JsonParser()
    {
        _lexer = new JsonLexer();
    }

    public DynamicValue Parse(string source)
    {
        _lexer.Load(source);
        _lexer.NextToken();

        var value = JsonValue();
        Match(JsonTokenType.EOF);

        return value;
    }

    private DynamicValue JsonValue()
    {
        return _lexer.Token.Type switch
        {
            JsonTokenType.LBrace => Object(),
            JsonTokenType.LBracket => Array(),
            JsonTokenType.String => String(),
            JsonTokenType.Number => Number(),
            JsonTokenType.Null => Null(),
            JsonTokenType.True => Boolean(),
            JsonTokenType.False => Boolean(),
            _ => throw new JsonParsingException(
                $"Expected an object, an array, a string, a number, or 'null'. Found '{_lexer.Token.Value}'.",
                _lexer.Token.Line, _lexer.Token.Column)
        };
    }
        
    private DynamicValue Object()
    {
        var table = new Table();

        Match(JsonTokenType.LBrace);
        {
            while (_lexer.Token.Type != JsonTokenType.RBrace)
            {
                if (_lexer.Token.Type == JsonTokenType.EOF)
                {
                    throw new JsonParsingException(
                        $"Unexpected EOF in an object.",
                        _lexer.Token.Line,
                        _lexer.Token.Column
                    );
                }
                    
                var key = String();
                Match(JsonTokenType.Colon);
                table[key] = JsonValue();
                    
                if (_lexer.Token.Type == JsonTokenType.RBrace)
                {
                    break;
                }
                    
                Match(JsonTokenType.Comma);
            }
        }
        Match(JsonTokenType.RBrace);
            
        return table;
    }

    private DynamicValue Array()
    {
        var array = new Array(0);
            
        Match(JsonTokenType.LBracket);
        {
            while (_lexer.Token.Type != JsonTokenType.RBracket)
            {
                if (_lexer.Token.Type == JsonTokenType.EOF)
                {
                    throw new JsonParsingException(
                        $"Unexpected EOF in an array.",
                        _lexer.Token.Line,
                        _lexer.Token.Column
                    );
                }

                array.Push(JsonValue());

                if (_lexer.Token.Type == JsonTokenType.RBracket)
                {
                    break;
                }
                    
                Match(JsonTokenType.Comma);
            }
        }
        Match(JsonTokenType.RBracket);
            
        return array;
    }

    private DynamicValue String()
    {
        var token = Match(JsonTokenType.String);
        return token.Value;
    }

    private DynamicValue Number()
    {
        var token = Match(JsonTokenType.Number);
            
        if (!double.TryParse(token.Value, out var number))
        {
            throw new JsonParsingException(
                $"The number '{token.Value}' is invalid or malformed.",
                token.Line,
                token.Column
            );
        }

        return number;
    }

    private DynamicValue Boolean()
    {
        var token = Match(JsonTokenType.True, JsonTokenType.False);
            
        if (!bool.TryParse(token.Value, out var boolean))
        {
            throw new JsonParsingException(
                $"The boolean value '{token.Value}' is invalid or malformed.",
                token.Line,
                token.Column
            );
        }

        return boolean;
    }

    private DynamicValue Null()
    {
        Match(JsonTokenType.Null);
        return DynamicValue.Nil;
    }

    private JsonToken Match(params JsonTokenType[] allowedTypes)
    {
        if (!allowedTypes.Contains(_lexer.Token.Type))
        {
            var allowedTypeString = string.Join(',', allowedTypes);

            throw new JsonParsingException(
                $"Expected a token of type(s) [{allowedTypeString}], found '{_lexer.Token.Type}'.",
                _lexer.Token.Line,
                _lexer.Token.Column
            );
        }

        var token = _lexer.Token;
            
        _lexer.NextToken();
        return token;
    }
}