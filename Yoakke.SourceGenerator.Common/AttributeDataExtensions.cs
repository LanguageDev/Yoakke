using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    public static class AttributeDataExtensions
    {
        public static object? GetCtorValue(this AttributeData data, int idx = 0) => data.ConstructorArguments[idx].Value;
    }
}
