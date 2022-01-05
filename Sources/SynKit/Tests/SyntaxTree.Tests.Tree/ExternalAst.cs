using Yoakke.SynKit.SyntaxTree.Attributes;

namespace Yoakke.SynKit.SyntaxTree.Tests.Tree;

[SyntaxTree]
public abstract partial record ExternalAst
{
    public partial record Foo : ExternalAst;

    public partial record Bar : ExternalAst;

    public partial record Baz : ExternalAst;
}
