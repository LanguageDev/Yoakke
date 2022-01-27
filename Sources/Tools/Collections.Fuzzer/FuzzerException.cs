// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Fuzzer;

internal class FuzzerException : Exception
{
    public FuzzerException(ValidationException validationException, string testCase, string operation)
        : this(validationException.Message, testCase, operation)
    {
    }

    public FuzzerException(string message, string testCase, string operation)
        : base($"{message}\nTest case:\n  Tree: {testCase}\n  Operation: {operation}")
    {
    }
}
