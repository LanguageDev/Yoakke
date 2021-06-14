// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class SignatureHelpClientCapabilities
    {
        public class SignatureInformationCapabilities
        {
            public class ParameterInformationCapabilities
            {
                /// <summary>
                /// The client supports processing label offsets instead of a
                /// simple label string.
                /// </summary>
                [Since(3, 14, 0)]
                [JsonProperty("labelOffsetSupport", NullValueHandling = NullValueHandling.Ignore)]
                public bool? LabelOffsetSupport { get; set; }
            }

            /// <summary>
            /// Client supports the follow content formats for the documentation
            /// property. The order describes the preferred format of the client.
            /// </summary>
            [JsonProperty("documentationFormat", NullValueHandling = NullValueHandling.Ignore)]
            public IReadOnlyList<MarkupKind>? DocumentationFormat { get; set; }

            /// <summary>
            /// Client capabilities specific to parameter information.
            /// </summary>
            [JsonProperty("parameterInformation", NullValueHandling = NullValueHandling.Ignore)]
            public ParameterInformationCapabilities? ParameterInformation { get; set; }

            /// <summary>
            /// The client supports the `activeParameter` property on
            /// `SignatureInformation` literal.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("activeParameterSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? ActiveParameterSupport { get; set; }
        }

        /// <summary>
        /// Whether signature help supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// The client supports the following `SignatureInformation`
        /// specific properties.
        /// </summary>
        [JsonProperty("signatureInformation", NullValueHandling = NullValueHandling.Ignore)]
        public SignatureInformationCapabilities? SignatureInformation { get; set; }

        /// <summary>
        /// The client supports to send additional context information for a
        /// `textDocument/signatureHelp` request. A client that opts into
        /// contextSupport will also support the `retriggerCharacters` on
        /// `SignatureHelpOptions`.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("contextSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContextSupport { get; set; }
    }
}
