﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class DocumentOnTypeFormattingOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like `}`.
        /// </summary>
        [JsonProperty("firstTriggerCharacter")]
        public string FirstTriggerCharacter { get; set; }
        /// <summary>
        /// More trigger characters.
        /// </summary>
        [JsonProperty("moreTriggerCharacter", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<string>? MoreTriggerCharacter { get; set; }
    }
}
