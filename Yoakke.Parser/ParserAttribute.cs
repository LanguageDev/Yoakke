using System;

namespace Yoakke.Parser
{
    /// <summary>
    /// An attribute to mark a class as a parser with rule methods inside.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ParserAttribute : Attribute
    {
    }
}
