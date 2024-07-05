namespace EVIL.JSON;

using System.IO;
using System.Text;
using Ceres.ExecutionEngine.TypeSystem;
using Deserialization;
using Serialization;

public static class EvilJson
{
    public static string Serialize(DynamicValue value)
    {
        var sb = new StringBuilder();
            
        using (var writer = new StringWriter(sb))
        {
            var emitter = new EvilJsonEmitter(writer);
            EvilJsonSerializer.SerializeDynamicValue(emitter, value);
        }

        return sb.ToString();
    }

    public static DynamicValue Deserialize(string json)
        => new JsonParser().Parse(json);
}