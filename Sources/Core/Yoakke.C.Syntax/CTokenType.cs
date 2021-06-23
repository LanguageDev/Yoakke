namespace Yoakke.C.Syntax
{
    /// <summary>
    /// Different token kinds for a C token.
    /// </summary>
    public enum CTokenType
    {
        End,
        Unknown,

        Hash,
        HashHash,

        OpenParen,
        CloseParen,
        OpenBracket,
        CloseBracket,
        OpenBrace,
        CloseBrace,

        Add,
        AddAssign,
        Subtract,
        SubtractAssign,
        Multiply,
        MultiplyAssign,
        Divide,
        DivideAssign,
        Modulo,
        ModuloAssign,

        BitAnd,
        BitAndAssign,
        BitOr,
        BitOrAssign,
        BitXor,
        BitXorAssign,
        BitNot,

        LogicalAnd,
        LogicalOr,
        LogicalNot,

        Increment,
        Decrement,

        Equal,
        NotEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Assign,

        ShiftLeft,
        ShiftLeftAssign,
        ShiftRight,
        ShiftRightAssign,

        Arrow,
        Dot,
        Comma,
        Colon,
        Semicolon,
        Ellipsis,
        QuestionMark,

        IntLiteral,
        FloatLiteral,
        CharLiteral,
        StringLiteral,

        Identifier,

        KeywordAuto,
        KeywordBreak,
        KeywordCase,
        KeywordChar,
        KeywordConst,
        KeywordContinue,
        KeywordDefault,
        KeywordDo,
        KeywordDouble,
        KeywordElse,
        KeywordEnum,
        KeywordExtern,
        KeywordFloat,
        KeywordFor,
        KeywordGoto,
        KeywordIf,
        KeywordInt,
        KeywordLong,
        KeywordRegister,
        KeywordReturn,
        KeywordShort,
        KeywordSigned,
        KeywordSizeof,
        KeywordStatic,
        KeywordStruct,
        KeywordSwitch,
        KeywordTypedef,
        KeywordUnion,
        KeywordUnsigned,
        KeywordVoid,
        KeywordVolatile,
        KeywordWhile,
    }
}
