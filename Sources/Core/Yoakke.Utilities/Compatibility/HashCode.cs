// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Utilities.Compatibility
{
    /// <summary>
    /// Polyfill for the System.HashCode implementation.
    /// Sadly source-generators are still .netstandard 2, if hosted in VS.
    /// </summary>
    internal struct HashCode
    {
        /// <summary>
        /// Combines multiples hash-codes into a single one.
        /// </summary>
        /// <param name="values">The values to combine the hash-codes of.</param>
        /// <returns>The combined hash-codes of <paramref name="values"/>.</returns>
        public static int Combine(params object?[] values)
        {
            var h = default(HashCode);
            foreach (var item in values) h.Add(item);
            return h.ToHashCode();
        }

        private int code;

        /// <summary>
        /// Adds a single value to the hash code.
        /// </summary>
        /// <param name="obj">The value to add to the hash code.</param>
        public void Add(object? obj)
        {
            if (obj is null)
            {
                this.code = CombineHashCodes(this.code, 0);
                return;
            }
            this.code = CombineHashCodes(this.code, obj.GetHashCode());
        }

        /// <summary>
        /// Calculates the final hash code after consecutive <see cref="Add(object?)"/> invocations.
        /// </summary>
        /// <returns>The calculated hash code.</returns>
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
