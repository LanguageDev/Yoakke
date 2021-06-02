using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Basic
{
    public class Location
    {
        [JsonProperty("uri")]
        public DocumentUri Uri { get; set; }
        [JsonProperty("range")]
        public Range Range { get; set; }
    }
}
