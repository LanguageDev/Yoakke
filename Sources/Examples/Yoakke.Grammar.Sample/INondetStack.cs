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

        public int ShiftCount { get; }
        public int ReduceCount { get; }

        public int? CurrentState { get; }
        public IEnumerable<IIncrementalTreeNode> Trees { get; }

        public string ToDot();
        public void Feed(IIncrementalTreeNode currentNode);
        public bool Step();
    }
}
