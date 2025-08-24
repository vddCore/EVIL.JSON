namespace EVIL.JSON.Serialization;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Ceres.ExecutionEngine.Collections;
using Ceres.ExecutionEngine.TypeSystem;
using CommonTypes.TypeSystem;

internal static class EvilJsonSerializer
{
    public static void SerializeNumber(EvilJsonEmitter emitter, double number)
        => emitter.EmitNumber(number);

    public static void SerializeString(EvilJsonEmitter emitter, string str)
        => emitter.EmitString(str);

    public static void SerializeBoolean(EvilJsonEmitter emitter, bool boolean)
        => emitter.EmitBoolean(boolean);

    public static void SerializeTypeCode(EvilJsonEmitter emitter, DynamicValueType typeCode)
        => emitter.EmitNumber((int)typeCode);

    public static void SerializeTable(EvilJsonEmitter emitter, Table table)
    {
        emitter.EmitLeftBrace();
        emitter.EmitNewLine();
        emitter.Indent();

        var list = table.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            var pair = list[i];

            var key = pair.Key;
            var value = pair.Value;

            if (key.Type != DynamicValueType.String)
            {
                key = key.ConvertToString();
            }

            emitter.EmitIndentation();
            emitter.EmitKey(key.String!);
            emitter.EmitSpace();
            SerializeDynamicValue(emitter, value);

            if (i < list.Count - 1)
            {
                emitter.EmitComma();
            }

            emitter.EmitNewLine();
        }

        emitter.Unindent();
        emitter.EmitIndentation();
        emitter.EmitRightBrace();
    }

    public static void SerializeArray(EvilJsonEmitter emitter, Array array)
    {
        emitter.EmitLeftBracket();
        emitter.EmitNewLine();
        emitter.Indent();

        for (var i = 0; i < array.Length; i++)
        {
            emitter.EmitIndentation();
            SerializeDynamicValue(emitter, array[i]);

            if (i < array.Length - 1)
            {
                emitter.EmitComma();
            }

            emitter.EmitNewLine();
        }

        emitter.Unindent();
        emitter.EmitIndentation();
        emitter.EmitRightBracket();
    }

    public static void SerializeDynamicValue(EvilJsonEmitter emitter, DynamicValue dynamicValue)
    {
        switch (dynamicValue.Type)
        {
            case DynamicValueType.Number:
                SerializeNumber(emitter, dynamicValue.Number);
                break;

            case DynamicValueType.String:
                SerializeString(emitter, dynamicValue.String!);
                break;

            case DynamicValueType.Boolean:
                SerializeBoolean(emitter, dynamicValue.Boolean);
                break;

            case DynamicValueType.TypeCode:
                SerializeTypeCode(emitter, dynamicValue.TypeCode);
                break;

            case DynamicValueType.Table:
                EnsureNoCircularDependencies(dynamicValue.Table!);
                SerializeTable(emitter, dynamicValue.Table!);
                break;

            case DynamicValueType.Array:
                SerializeArray(emitter, dynamicValue.Array!);
                break;

            default:
                emitter.EmitNull();
                break;
        }
    }

    private static void EnsureNoCircularDependencies(IEnumerable<KeyValuePair<DynamicValue, DynamicValue>> collection)
    {
        var pairs = collection.ToList();
        var queue = new Queue<IEnumerable<KeyValuePair<DynamicValue, DynamicValue>>>();
        var hashSet = new HashSet<IEnumerable<KeyValuePair<DynamicValue, DynamicValue>>>();
        
        hashSet.Add(pairs);
        queue.Enqueue(pairs);

        do
        {
            var processedCollection = queue.Dequeue();

            foreach (var (_, value) in processedCollection)
            {
                if (value.Type == DynamicValueType.Array)
                {
                    if (!hashSet.Add(value.Array!))
                    {
                        throw new SerializationException(
                            "Circular reference in collection graph (array)."
                        );
                    }

                    queue.Enqueue(value.Array!);
                }
                else if (value.Type == DynamicValueType.Table)
                {
                    if (!hashSet.Add(value.Table!))
                    {
                        throw new SerializationException(
                            "Circular reference in collection graph (table)."
                        );
                    }

                    queue.Enqueue(value.Table!);
                }
            }
                
        } while (queue.Any());
    }
}