using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// Static registration options to be returned in the initialize request.
    /// </summary>
    public interface IStaticRegistrationOptions
    {
        /// <summary>
        /// The id used to register the request. The id can be used to deregister
        /// the request again. See also Registration#id.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }
    }
}
