using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    public enum TraceValue
    {
        [EnumMember(Value = "off")]
        Off,
        [EnumMember(Value = "messages")]
        Messages,
        [EnumMember(Value = "verbose")]
        Verbose,
    }
}
