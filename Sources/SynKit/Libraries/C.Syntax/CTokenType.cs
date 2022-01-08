// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// Different token kinds for a C token.
/// </summary>
public enum CTokenType
{
    /// <summary>
    /// End of source.
    /// </summary>
    End,

    /// <summary>
    /// Unrecognized.
    /// </summary>
    Unknown,

    /// <summary>
    /// '#', for preprocessing (directive and stringify).
    /// </summary>
    Hash,

    /// <summary>
    /// '##', for preprocessing (paste).
    /// </summary>
    HashHash,

    /// <summary>
    /// '('.
    /// </summary>
    OpenParen,

    /// <summary>
    /// ')'.
    /// </summary>
    CloseParen,

    /// <summary>
    /// '['.
    /// </summary>
    OpenBracket,

    /// <summary>
    /// ']'.
    /// </summary>
    CloseBracket,

    /// <summary>
    /// '{'.
    /// </summary>
    OpenBrace,

    /// <summary>
    /// '}'.
    /// </summary>
    CloseBrace,

    /// <summary>
    /// '+'.
    /// </summary>
    Add,

    /// <summary>
    /// '+='.
    /// </summary>
    AddAssign,

    /// <summary>
    /// '-'.
    /// </summary>
    Subtract,

    /// <summary>
    /// '-='.
    /// </summary>
    SubtractAssign,

    /// <summary>
    /// '*'.
    /// </summary>
    Multiply,

    /// <summary>
    /// '*='.
    /// </summary>
    MultiplyAssign,

    /// <summary>
    /// '/'.
    /// </summary>
    Divide,

    /// <summary>
    /// '/='.
    /// </summary>
    DivideAssign,

    /// <summary>
    /// '%'.
    /// </summary>
    Modulo,

    /// <summary>
    /// '%='.
    /// </summary>
    ModuloAssign,

    /// <summary>
    /// '&amp;'.
    /// </summary>
    BitAnd,

    /// <summary>
    /// '&amp;='.
    /// </summary>
    BitAndAssign,

    /// <summary>
    /// '|'.
    /// </summary>
    BitOr,

    /// <summary>
    /// '|='.
    /// </summary>
    BitOrAssign,

    /// <summary>
    /// '^'.
    /// </summary>
    BitXor,

    /// <summary>
    /// '^='.
    /// </summary>
    BitXorAssign,

    /// <summary>
    /// '~'.
    /// </summary>
    BitNot,

    /// <summary>
    /// '&amp;&amp;'.
    /// </summary>
    LogicalAnd,

    /// <summary>
    /// '||'.
    /// </summary>
    LogicalOr,

    /// <summary>
    /// '!'.
    /// </summary>
    LogicalNot,

    /// <summary>
    /// '++'.
    /// </summary>
    Increment,

    /// <summary>
    /// '--'.
    /// </summary>
    Decrement,

    /// <summary>
    /// '=='.
    /// </summary>
    Equal,

    /// <summary>
    /// '!='.
    /// </summary>
    NotEqual,

    /// <summary>
    /// '&gt;'.
    /// </summary>
    Greater,

    /// <summary>
    /// '&gt;='.
    /// </summary>
    GreaterEqual,

    /// <summary>
    /// '&lt;'.
    /// </summary>
    Less,

    /// <summary>
    /// '&lt;='.
    /// </summary>
    LessEqual,

    /// <summary>
    /// '='.
    /// </summary>
    Assign,

    /// <summary>
    /// '&lt;&lt;'.
    /// </summary>
    ShiftLeft,

    /// <summary>
    /// '&lt;&lt;='.
    /// </summary>
    ShiftLeftAssign,

    /// <summary>
    /// '&gt;&gt;'.
    /// </summary>
    ShiftRight,

    /// <summary>
    /// '&gt;&gt;='.
    /// </summary>
    ShiftRightAssign,

    /// <summary>
    /// '->'.
    /// </summary>
    Arrow,

    /// <summary>
    /// '.'.
    /// </summary>
    Dot,

    /// <summary>
    /// ','.
    /// </summary>
    Comma,

    /// <summary>
    /// ':'.
    /// </summary>
    Colon,

    /// <summary>
    /// ';'.
    /// </summary>
    Semicolon,

    /// <summary>
    /// '...'.
    /// </summary>
    Ellipsis,

    /// <summary>
    /// '?'.
    /// </summary>
    QuestionMark,

    /// <summary>
    /// Any C integer literal, regardless of radix.
    /// </summary>
    IntLiteral,

    /// <summary>
    /// Any C floating point number.
    /// </summary>
    FloatLiteral,

    /// <summary>
    /// A C character literal.
    /// </summary>
    CharLiteral,

    /// <summary>
    /// A C string literal.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Any C identifier that is not a keyword.
    /// </summary>
    Identifier,

    /// <summary>
    /// 'auto' keyword.
    /// </summary>
    KeywordAuto,

    /// <summary>
    /// 'break' keyword.
    /// </summary>
    KeywordBreak,

    /// <summary>
    /// 'case' keyword.
    /// </summary>
    KeywordCase,

    /// <summary>
    /// 'char' keyword.
    /// </summary>
    KeywordChar,

    /// <summary>
    /// 'const' keyword.
    /// </summary>
    KeywordConst,

    /// <summary>
    /// 'continue' keyword.
    /// </summary>
    KeywordContinue,

    /// <summary>
    /// 'default' keyword.
    /// </summary>
    KeywordDefault,

    /// <summary>
    /// 'do' keyword.
    /// </summary>
    KeywordDo,

    /// <summary>
    /// 'double' keyword.
    /// </summary>
    KeywordDouble,

    /// <summary>
    /// 'else' keyword.
    /// </summary>
    KeywordElse,

    /// <summary>
    /// 'enum' keyword.
    /// </summary>
    KeywordEnum,

    /// <summary>
    /// 'extern' keyword.
    /// </summary>
    KeywordExtern,

    /// <summary>
    /// 'float' keyword.
    /// </summary>
    KeywordFloat,

    /// <summary>
    /// 'for' keyword.
    /// </summary>
    KeywordFor,

    /// <summary>
    /// 'goto' keyword.
    /// </summary>
    KeywordGoto,

    /// <summary>
    /// 'if' keyword.
    /// </summary>
    KeywordIf,

    /// <summary>
    /// 'inline' keyword.
    /// </summary>
    KeywordInline,

    /// <summary>
    /// 'int' keyword.
    /// </summary>
    KeywordInt,

    /// <summary>
    /// 'long' keyword.
    /// </summary>
    KeywordLong,

    /// <summary>
    /// 'register' keyword.
    /// </summary>
    KeywordRegister,

    /// <summary>
    /// 'restrict' keyword.
    /// </summary>
    KeywordRestrict,

    /// <summary>
    /// 'return' keyword.
    /// </summary>
    KeywordReturn,

    /// <summary>
    /// 'short' keyword.
    /// </summary>
    KeywordShort,

    /// <summary>
    /// 'signed' keyword.
    /// </summary>
    KeywordSigned,

    /// <summary>
    /// 'sizeof' keyword.
    /// </summary>
    KeywordSizeof,

    /// <summary>
    /// 'static' keyword.
    /// </summary>
    KeywordStatic,

    /// <summary>
    /// 'struct' keyword.
    /// </summary>
    KeywordStruct,

    /// <summary>
    /// 'switch' keyword.
    /// </summary>
    KeywordSwitch,

    /// <summary>
    /// 'typedef' keyword.
    /// </summary>
    KeywordTypedef,

    /// <summary>
    /// 'union' keyword.
    /// </summary>
    KeywordUnion,

    /// <summary>
    /// 'unsigned' keyword.
    /// </summary>
    KeywordUnsigned,

    /// <summary>
    /// 'void' keyword.
    /// </summary>
    KeywordVoid,

    /// <summary>
    /// 'volatile' keyword.
    /// </summary>
    KeywordVolatile,

    /// <summary>
    /// 'while' keyword.
    /// </summary>
    KeywordWhile,

    /// <summary>
    /// '_Alignas' keyword.
    /// </summary>
    KeywordAlignAs,

    /// <summary>
    /// '_Alignof' keyword.
    /// </summary>
    KeywordAlignOf,

    /// <summary>
    /// '_Atomic' keyword.
    /// </summary>
    KeywordAtomic,

    /// <summary>
    /// '_Bool' keyword.
    /// </summary>
    KeywordBool,

    /// <summary>
    /// '_Complex' keyword.
    /// </summary>
    KeywordComplex,

    /// <summary>
    /// '_Generic' keyword.
    /// </summary>
    KeywordGeneric,

    /// <summary>
    /// '_Imaginary' keyword.
    /// </summary>
    KeywordImaginary,

    /// <summary>
    /// '_Noreturn' keyword.
    /// </summary>
    KeywordNoReturn,

    /// <summary>
    /// '_Static_assert' keyword.
    /// </summary>
    KeywordStaticAssert,

    /// <summary>
    /// '_Thread_local' keyword.
    /// </summary>
    KeywordThreadLocal,
}
