// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;

namespace Yoakke.Grammar.Sample
{
    public interface INondetStack
    {
        public ILrParsingTable ParsingTable { get; }

        public string ToDot();
        public void Feed(Terminal terminal);
        public bool Step();
    }
}
