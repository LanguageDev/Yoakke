// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Client
{
    public class RegistrationParams
    {
        [JsonProperty("registrations")]
        public IReadOnlyList<Registration> Registrations { get; set; }
    }
}
