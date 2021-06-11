﻿// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;

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
