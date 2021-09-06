// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Ir.Model.Builders;
using Yoakke.Lexer;
using Yoakke.Lexer.Streams;
using AttributeTargets = Yoakke.Ir.Model.Attributes.AttributeTargets;
using Type = Yoakke.Ir.Model.Type;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// Parser for IR model elements.
    /// </summary>
    public class IrParser
    {
        /// <summary>
        /// The source stream.
        /// </summary>
        public ITokenStream<IToken<IrTokenType>> Source { get; }

        private readonly Context context;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrParser"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Context"/> for the IR.</param>
        /// <param name="source">The token source to parse from.</param>
        public IrParser(Context context, ITokenStream<IToken<IrTokenType>> source)
        {
            this.context = context;
            this.Source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrParser"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Context"/> for the IR.</param>
        /// <param name="source">The token source to parse from.</param>
        public IrParser(Context context, ILexer<IToken<IrTokenType>> source)
            : this(context, source.AsTokenStream())
        {
        }

        /// <summary>
        /// Parses a <see cref="BasicBlock"/>.
        /// </summary>
        /// <returns>The parsed <see cref="BasicBlock"/>.</returns>
        public BasicBlock ParseBasicBlock()
        {
            this.Expect(IrTokenType.KeywordBlock);
            var name = this.ParseIdentifier();
            var attribs = this.ParseAttributeGroups(AttributeTargets.BasicBlock);
            this.Expect(IrTokenType.Colon);
            var builder = new BasicBlockBuilder(name);
            // Attach all attributes
            AttachAttributes(builder, attribs);
            // Parse instructions
            while (this.Source.TryPeek(out var t) && t.Kind == IrTokenType.Identifier)
            {
                // Assume instruction as long as it's an identifier
                var instr = this.ParseInstruction();
                builder.Instructions.Add(instr);
            }
            return builder;
        }

        /// <summary>
        /// Parses an <see cref="Instruction"/>.
        /// </summary>
        /// <returns>The parsed <see cref="Instruction"/>.</returns>
        public Instruction ParseInstruction()
        {
            // Get the syntax
            var ident = this.ParseIdentifier();
            var syntax = this.context.GetInstructionSyntax(ident);
            // Parse it
            var instr = syntax.Parse(this);
            // Parse the attribute groups that might follow
            var attrs = this.ParseAttributeGroups(AttributeTargets.Instruction);
            // Attach them
            AttachAttributes(instr, attrs);
            // Done
            return instr;
        }

        /// <summary>
        /// Parses a sequence of attribute groups, which is 0 of more occurrences of a singular attribute group.
        /// </summary>
        /// <param name="defaultTarget">The default target to assume.</param>
        /// <returns>The parsed groups, all attributes categorized per target.</returns>
        public IReadOnlyDictionary<AttributeTargets, IList<IAttribute>> ParseAttributeGroups(AttributeTargets defaultTarget)
        {
            var result = new Dictionary<AttributeTargets, IList<IAttribute>>();
            while (this.Source.TryPeek(out var t) && t.Kind == IrTokenType.OpenBracket)
            {
                // Parse a single group
                var partialResul = this.ParseAttributeGroup(defaultTarget);
                // Merge in
                foreach (var (k, v) in partialResul)
                {
                    if (result.TryGetValue(k, out var existing))
                    {
                        foreach (var item in v) existing.Add(item);
                    }
                    else
                    {
                        result.Add(k, v);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses a single attribute group, which is a list of attribute instantiations between brackets,
        /// optionally with target specifiers.
        /// </summary>
        /// <param name="defaultTarget">The default target to assume.</param>
        /// <returns>The parsed groups, all attributes categorized per target.</returns>
        public IReadOnlyDictionary<AttributeTargets, IList<IAttribute>> ParseAttributeGroup(AttributeTargets defaultTarget)
        {
            AttributeTargets currentTarget = defaultTarget;
            var result = new Dictionary<AttributeTargets, IList<IAttribute>>();

            void ParseAttributeGroupElement()
            {
                var specifier = this.TryParseAttributeTargetSpecifier();
                if (specifier is not null) currentTarget = specifier.Value;
                var attr = this.ParseAttribute();
                if (!result!.TryGetValue(currentTarget, out var attrList))
                {
                    attrList = new List<IAttribute>();
                    result.Add(currentTarget, attrList);
                }
                attrList.Add(attr);
            }

            this.Expect(IrTokenType.OpenBracket);
            if (!this.Matches(IrTokenType.CloseBracket))
            {
                // First element
                ParseAttributeGroupElement();
                // Parse as long as a comma follows
                for (; this.Matches(IrTokenType.Comma); ParseAttributeGroupElement())
                {
                    // Pass
                }
                // Closing bracket
                this.Expect(IrTokenType.CloseBracket);
            }

            return result;
        }

        /// <summary>
        /// Parses a single attribute instantiation.
        /// </summary>
        /// <returns>The instantiated <see cref="IAttribute"/>.</returns>
        public IAttribute ParseAttribute()
        {
            // Get the definition
            var ident = this.ParseIdentifier();
            var attribDef = this.context.GetAttributeDefinition(ident);

            var args = new List<Constant>();
            var argIndex = 0;

            void ParseAttributeArgument()
            {
                if (argIndex >= attribDef!.ParameterTypes.Count)
                {
                    throw new InvalidOperationException($"Attribute {attribDef.Name} only expects {attribDef.ParameterTypes.Count} parameters.");
                }
                var argnType = attribDef.ParameterTypes[argIndex++];
                var argn = this.ParseConstant(argnType);
                args!.Add(argn);
            }

            // Arguments between parenthesis
            if (this.Matches(IrTokenType.OpenParen))
            {
                if (!this.Matches(IrTokenType.CloseParen))
                {
                    // Parse first argument
                    ParseAttributeArgument();
                    // Parse as long as a comma follows
                    for (; this.Matches(IrTokenType.Comma); ParseAttributeArgument())
                    {
                        // Pass
                    }
                    // Expect the close parenthesis
                    this.Expect(IrTokenType.CloseParen);
                }
            }

            // Argument after '='
            if (this.Matches(IrTokenType.Assign)) ParseAttributeArgument();

            // Check if we had enough arguments
            if (args.Count != attribDef.ParameterTypes.Count)
            {
                throw new InvalidOperationException($"Attribute {attribDef.Name} expects {attribDef.ParameterTypes.Count} parameters but only {args.Count} were provided.");
            }

            // We are done
            return attribDef.Instantiate(args);
        }

        /// <summary>
        /// Parses a <see cref="Constant"/> of a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of <see cref="Constant"/> to parse.</param>
        /// <returns>The parsed <see cref="Constant"/>.</returns>
        public Constant ParseConstant(Type type) => throw new NotImplementedException("TODO: Parse constant of type Type");

        private AttributeTargets? TryParseAttributeTargetSpecifier()
        {
            if (!this.Source.TryPeek(out var t)) return null;
            AttributeTargets? target = t.Kind switch
            {
                IrTokenType.KeywordAssembly => AttributeTargets.Assembly,
                IrTokenType.KeywordBlock => AttributeTargets.BasicBlock,
                IrTokenType.KeywordField => AttributeTargets.TypeField,
                IrTokenType.KeywordInstruction => AttributeTargets.Instruction,
                IrTokenType.KeywordParameter => AttributeTargets.Parameter,
                IrTokenType.KeywordProcedure => AttributeTargets.Procedure,
                IrTokenType.KeywordReturn => AttributeTargets.ReturnValue,
                IrTokenType.KeywordType => AttributeTargets.TypeDefinition,
                _ => null,
            };
            if (target is not null)
            {
                this.Source.Advance();
                this.Expect(IrTokenType.Colon);
            }
            return target;
        }

        private string ParseIdentifier()
        {
            var ident = this.Expect(IrTokenType.Identifier);
            var result = new StringBuilder(ident.Text);
            while (this.Matches(IrTokenType.Dot))
            {
                ident = this.Expect(IrTokenType.Identifier);
                result.Append('.').Append(ident.Text);
            }
            return result.ToString();
        }

        private IToken<IrTokenType> Expect(IrTokenType tokenType)
        {
            if (!this.Matches(tokenType, out var token))
            {
                throw new InvalidOperationException($"Syntax error, expected {tokenType}");
            }
            return token;
        }

        private bool Matches(IrTokenType tokenType) => this.Matches(tokenType, out _);

        private bool Matches(IrTokenType tokenType, [MaybeNullWhen(false)] out IToken<IrTokenType> token)
        {
            if (this.Source.TryPeek(out token) && token.Kind == tokenType)
            {
                this.Source.Advance();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void AttachAttributes(
            IAttributeTarget attributeTarget,
            IReadOnlyDictionary<AttributeTargets, IList<IAttribute>> attributes)
        {
            foreach (var (target, attrList) in attributes)
            {
                if (target != attributeTarget.Flag)
                {
                    throw new InvalidOperationException($"Invalid attribute target {target} for element");
                }
                // Attachable
                foreach (var attr in attrList) attributeTarget.AddAttribute(attr);
            }
        }
    }
}
