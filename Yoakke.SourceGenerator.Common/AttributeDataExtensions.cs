using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SourceGenerator.Common
{
    public static class AttributeDataExtensions
    {
        public static object GetCtorValue(this AttributeData data, int idx = 0)
        {
            return data.ConstructorArguments[idx].Value;
        }

        public static bool TryGetPropertyValue(this AttributeData data, string name, out object value)
        {
            var matches = data
                .NamedArguments
                .Where(kv => kv.Key == name)
                .Select(kv => kv.Value);
            value = matches.FirstOrDefault().Value;
            return matches.Any();
        }

        public static object GetPropertyValue(this AttributeData data, string name)
        {
            if (!data.TryGetPropertyValue(name, out var value)) throw new KeyNotFoundException();
            return value;
        }
    }
}
