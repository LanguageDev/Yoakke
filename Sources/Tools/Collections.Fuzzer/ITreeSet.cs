// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Fuzzer;

internal interface ITreeSet
{
    public int Count { get; }

    public string ToTestCaseString();
    public bool IsValid(IEnumerable<int> expectedElements);
    public bool Insert(int k);
    public bool Delete(int k);
}
