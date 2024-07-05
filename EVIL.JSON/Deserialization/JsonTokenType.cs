namespace EVIL.JSON.Deserialization;

internal enum JsonTokenType
{
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    Colon,
    Comma,
    True,
    False,
    Null,
    String,
    Number,
    EOF
}