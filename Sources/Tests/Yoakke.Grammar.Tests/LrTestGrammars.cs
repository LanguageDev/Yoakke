// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Grammar.Tests
{
    internal static class LrTestGrammars
    {
        public const string Lr0Grammar = @"
S -> a S b
S -> a S c
S -> d b
";

        public const string SlrGrammar = @"
S -> E
E -> 1 E
E -> 1
";

        public const string LalrGrammar = @"
S -> a A c
S -> a B d
S -> B c
A -> z
B -> z
";

        public const string ClrGrammar = @"
S -> a E a
S -> b E b
S -> a F b
S -> b F a
E -> e
F -> e
";
    }
}
