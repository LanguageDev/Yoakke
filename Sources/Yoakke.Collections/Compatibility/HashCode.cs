﻿// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Collections.Compatibility
{
    internal struct HashCode
    {
        public static int Combine(params object?[] values)
        {
            var h = default(HashCode);
            foreach (var item in values) h.Add(item);
            return h.ToHashCode();
        }

        private int code;

        public void Add(object? obj)
        {
            if (obj is null)
            {
                this.code = CombineHashCodes(this.code, 0);
                return;
            }
            this.code = CombineHashCodes(this.code, obj.GetHashCode());
        }

        public int ToHashCode() => this.code;

        private static int CombineHashCodes(int code1, int code2)
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + code1;
                hash = (hash * 31) + code2;
                return hash;
            }
        }
    }
}

