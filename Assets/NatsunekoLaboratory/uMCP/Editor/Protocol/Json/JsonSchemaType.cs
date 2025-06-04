using System.Runtime.Serialization;

namespace NatsunekoLaboratory.uMCP.Protocol.Json
{
    public enum JsonSchemaType
    {
        [EnumMember(Value = "string")]
        String,

        [EnumMember(Value = "integer")]
        Integer,

        [EnumMember(Value = "number")]
        Number,

        [EnumMember(Value = "boolean")]
        Boolean,

        [EnumMember(Value = "array")]
        Array,

        [EnumMember(Value = "object")]
        Object
    }
}
