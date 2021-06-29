// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.C.Syntax.Macros
{
    /// <summary>
    /// Implementation of __COUNTER__.
    /// </summary>
    public class CounterMacro : IMacro
    {
        private int count = 0;

        public string Name => "__COUNTER__";

        public IReadOnlyList<string>? Parameters => null;

        public IEnumerable<CToken> Expand(IPreProcessor preProcessor, IReadOnlyList<IReadOnlyList<CToken>> arguments)
        {
            var str = (this.count++).ToString();
            // TODO: Proper range?
            var token = new CToken(default, str, default, str, CTokenType.IntLiteral);
            return new[] { token };
        }
    }
}
