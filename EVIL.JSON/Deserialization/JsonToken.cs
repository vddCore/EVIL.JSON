using System;

namespace EVIL.JSON.Deserialization
{
    internal struct JsonToken : IEquatable<JsonToken>
    {
        public int Line { get; init; }
        public int Column { get; init; }
        public string Value { get; }
        public JsonTokenType Type { get; }

        public bool IsString => Type == JsonTokenType.String;
        public bool IsNumber => Type == JsonTokenType.Number;

        public JsonToken(
            int Line,
            int Column,
            string Value,
            JsonTokenType Type
        )
        {
            this.Line = Line;
            this.Column = Column;
            this.Value = Value;
            this.Type = Type;
        }

        public bool Equals(JsonToken other)
            => Value == other.Value && Type == other.Type;

        public static bool operator ==(JsonToken a, JsonToken b)
            => a.Equals(b);

        public static bool operator !=(JsonToken a, JsonToken b)
            => !a.Equals(b);

        public override int GetHashCode()
            => HashCode.Combine(Value, (int)Type);

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;

            return Equals((JsonToken)obj);
        }

        public static readonly JsonToken LBrace = new(-1, -1, "{", JsonTokenType.LBrace);
        public static readonly JsonToken RBrace = new(-1, -1, "}", JsonTokenType.RBrace);
        public static readonly JsonToken LBracket = new(-1, -1, "[", JsonTokenType.LBracket);
        public static readonly JsonToken RBracket = new(-1, -1, "]", JsonTokenType.RBracket);
        public static readonly JsonToken Colon = new(-1, -1, ":", JsonTokenType.Colon);
        public static readonly JsonToken Comma = new(-1, -1, ",", JsonTokenType.Comma);
        public static readonly JsonToken True = new(-1, -1, "true", JsonTokenType.True);
        public static readonly JsonToken False = new(-1, -1, "true", JsonTokenType.False);
        public static readonly JsonToken Null = new(-1, -1, "true", JsonTokenType.Null);
        public static readonly JsonToken EOF = new(-1, -1, "<EOF>", JsonTokenType.EOF);
    }
}