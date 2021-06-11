using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// A set of predefined code action kinds.
    /// </summary>
    public enum CodeActionKind
    {
        /// <summary>
        /// Empty kind.
        /// </summary>
        [EnumMember(Value = "")]
        Empty,

        /// <summary>
        /// Base kind for quickfix actions: 'quickfix'.
        /// </summary>
        [EnumMember(Value = "quickfix")]
        QuickFix,

        /// <summary>
        /// Base kind for refactoring actions: 'refactor'.
        /// </summary>
        [EnumMember(Value = "refactor")]
        Refactor,

        /// <summary>
        /// Base kind for refactoring extraction actions: 'refactor.extract'.
        ///
        /// Example extract actions:
        ///
        /// - Extract method
        /// - Extract function
        /// - Extract variable
        /// - Extract interface from class
        /// - ...
        /// </summary>
        [EnumMember(Value = "refactor.extract")]
        RefactorExtract,

        /// <summary>
        /// Base kind for refactoring inline actions: 'refactor.inline'.
        ///
        /// Example inline actions:
        ///
        /// - Inline function
        /// - Inline variable
        /// - Inline constant
        /// - ...
        /// </summary>
        [EnumMember(Value = "refactor.inline")]
        RefactorInline,

        /// <summary>
        /// Base kind for refactoring rewrite actions: 'refactor.rewrite'.
        ///
        /// Example rewrite actions:
        ///
        /// - Convert JavaScript function to class
        /// - Add or remove parameter
        /// - Encapsulate field
        /// - Make method static
        /// - Move method to base class
        /// - ...
        /// </summary>
        [EnumMember(Value = "refactor.rewrite")]
        RefactorRewrite,

        /// <summary>
        /// Base kind for source actions: `source`.
        ///
        /// Source code actions apply to the entire file.
        /// </summary>
        [EnumMember(Value = "source")]
        Source,

        /// <summary>
        /// Base kind for an organize imports source action:
        /// `source.organizeImports`.
        /// </summary>
        [EnumMember(Value = "source.organizeImports")]
        SourceOrganizeImports,
    }
}
