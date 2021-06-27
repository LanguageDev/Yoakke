// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A <see cref="IMacro"/> that can be defined by the user.
    /// Has optional arguments, returns it's body with the arguments substituted.
    /// </summary>
    public class UserMacro : IMacro
    {
        public string Name { get; }

        public IReadOnlyList<string>? Parameters { get; }

        private readonly IReadOnlyList<CToken> body;

        public UserMacro(string name, IReadOnlyList<string>? parameters, IReadOnlyList<CToken> body)
        {
            this.Name = name;
            this.Parameters = parameters;
            this.body = body;
        }

        public IEnumerable<CToken> Expand(IReadOnlyList<IReadOnlyList<CToken>> arguments)
        {
            if (this.Parameters is null || this.Parameters.Count == 0)
            {
                // If we have no parameters, just return the body
                return this.body;
            }

            // Otherwise we have to look out for substitutions
            // TODO
            throw new NotImplementedException();
        }
    }
}
