using System;
using Ceres.ExecutionEngine.Concurrency;
using Ceres.ExecutionEngine.TypeSystem;
using Ceres.Runtime;
using Ceres.Runtime.Extensions;
using EVIL.CommonTypes.TypeSystem;

namespace EVIL.JSON.Runtime
{
    public class JsonModule : RuntimeModule
    {
        public override string FullyQualifiedName { get; } = "json";

        [RuntimeModuleFunction("stringify")]
        [EvilDocFunction(
            "Attempts to serialize the provided value into JSON.",
            ReturnType = DynamicValueType.String,
            Returns = "JSON representation of the provided value.")]
        [EvilDocArgument("value", "The value to be serialized.", CanBeNil = true)]
        [EvilDocArgument(
            "ignore_errors",
            "Whether to return an empty string or terminate the program upon error.", 
            DynamicValueType.Boolean,
            DefaultValue = "true")]
        private static DynamicValue Serialize(Fiber _, params DynamicValue[] args)
        {
            args.ExpectAtLeast(1)
                .ExpectAtMost(2)
                .ExpectAnyAt(0, out var dynamicValue)
                .OptionalBooleanAt(1, true, out var ignoreErrors);

            try
            {
                return EvilJson.Serialize(dynamicValue);
            }
            catch (Exception e)
            {
                if (ignoreErrors)
                {
                    return string.Empty;
                }

                throw new EvilRuntimeException($"Unable to serialize the provided object.", e);
            }
        }
        
        [RuntimeModuleFunction("parse")]
        [EvilDocFunction(
            "Attempts to dserialize the provided string back into a usable dynamic value.",
            IsAnyReturn = true,
            Returns = "Object represented by the provided JSON string.")]
        [EvilDocArgument(
            "json",
            "The JSON string to be deserialized.",
            DynamicValueType.String)]
        [EvilDocArgument(
            "ignore_errors",
            "Whether to return Nil or terminate the program upon deserialization error.", 
            DynamicValueType.Boolean,
            DefaultValue = "true")]
        private static DynamicValue Deserialize(Fiber _, params DynamicValue[] args)
        {
            args.ExpectAtLeast(1)
                .ExpectAtMost(2)
                .ExpectStringAt(0, out var json)
                .OptionalBooleanAt(1, true, out var ignoreErrors);

            try
            {
                return EvilJson.Deserialize(json);
            }
            catch (Exception e)
            {
                if (ignoreErrors)
                {
                    return DynamicValue.Nil;
                }

                throw new EvilRuntimeException($"Unable to deserialize the provided JSON string.", e);
            }
        }
    }
}