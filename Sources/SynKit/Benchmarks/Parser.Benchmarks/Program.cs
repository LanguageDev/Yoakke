// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using BenchmarkDotNet.Running;

if (args.Length == 1 && args[0] == "parser")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().ManualExpressionParser();
}
else
{
   BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
