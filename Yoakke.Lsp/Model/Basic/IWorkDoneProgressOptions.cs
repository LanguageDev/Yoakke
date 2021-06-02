using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public interface IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }
    }
}
