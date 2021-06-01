using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    public enum TokenFormat
    {
        [EnumMember(Value = "relative")]
        Relative,
    }
}
