// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Ir.Model.Builders;
using Yoakke.Lexer;
using Yoakke.Streams;
using AttributeTargets = Yoakke.Ir.Model.Attributes.AttributeTargets;
using Type = Yoakke.Ir.Model.Type;

namespace Yoakke.Ir.Syntax;

/// <summary>
/// Parser for IR model elements.
/// </summary>
public class IrParser
{
  /// <summary>
  /// The source stream.
  /// </summary>
  public IPeekableStream<IToken<IrTokenType>> Source { get; }

  private readonly Context context;
  private readonly Dictionary<string, ProcedureBuilder> procedureBuilders = new();
  private readonly Dictionary<string, BasicBlockBuilder> basicBlockBuilders = new();
  private readonly Dictionary<string, Instruction> valueInstructions = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="IrParser"/> class.
  /// </summary>
  /// <param name="context">The <see cref="Context"/> for the IR.</param>
  /// <param name="source">The token source to parse from.</param>
  public IrParser(Context context, IStream<IToken<IrTokenType>> source)
  {
    this.context = context;
    this.Source = source.ToBuffered();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="IrParser"/> class.
  /// </summary>
  /// <param name="context">The <see cref="Context"/> for the IR.</param>
  /// <param name="source">The token source to parse from.</param>
  public IrParser(Context context, ILexer<IToken<IrTokenType>> source)
      : this(context, source.ToStream())
  {
  }

  /// <summary>
  /// Parses an <see cref="Assembly"/>.
  /// </summary>
  /// <returns>The parsed <see cref="Assembly"/>.</returns>
  public Assembly ParseAssembly()
  {
    var builder = new AssemblyBuilder();
    this.PreDefineProcedures();
    while (true)
    {
      if (!this.Source.TryPeek(out var t) || t.Kind == IrTokenType.End) break;
      if (this.EatNewline()) continue;

      if (t.Kind == IrTokenType.KeywordProcedure)
      {
        // A procedure in the assembly
        var proc = this.ParseProcedure();
        builder.WithProcedure(proc);
      }
      else if (t.Kind == IrTokenType.OpenBracket)
      {
        // A global attribute
        var attrs = this.ParseAttributeGroups(AttributeTargets.Assembly);
        AttachAttributes(builder, attrs);
      }
      else
      {
        // Unknown
        throw new NotImplementedException($"Unexpected token {t.Kind}");
      }
    }
    return builder;
  }

  /// <summary>
  /// Parses a <see cref="Procedure"/>.
  /// </summary>
  /// <returns>The parsed <see cref="Procedure"/>.</returns>
  public Procedure ParseProcedure()
  {
    this.Expect(IrTokenType.KeywordProcedure);
    var name = this.ParseIdentifier();
    var builder = this.procedureBuilders[name];
    this.PreDefineBasicBlocks();
    // Parse parameters
    this.Expect(IrTokenType.OpenParen);
    this.Expect(IrTokenType.CloseParen);
    // TODO: Parse return type
    // Check, if it has a body
    if (this.Matches(IrTokenType.Colon))
    {
      // Parse attached attributes
      // TODO: We need to be smarter here, return value is also a valid target, a simple attach won't do
      this.ParseAttributeGroups(builder, AttributeTargets.Procedure);
      this.Expect(IrTokenType.Newline);

      // It has a body
      while (this.Source.TryPeek(out var t) && t.Kind == IrTokenType.KeywordBlock)
      {
        var bb = this.ParseBasicBlock();
        builder.WithBasicBlock(bb);
      }
    }
    else
    {
      // Parse attached attributes
      // TODO: We need to be smarter here, return value is also a valid target, a simple attach won't do
      this.ParseAttributeGroups(builder, AttributeTargets.Procedure);
    }
    // Assign first block as entry
    if (builder.BasicBlocks.Count > 0) builder.WithEntryAt(builder.BasicBlocks[0]);
    return builder;
  }

  /// <summary>
  /// Parses a <see cref="BasicBlock"/>.
  /// </summary>
  /// <returns>The parsed <see cref="BasicBlock"/>.</returns>
  public BasicBlock ParseBasicBlock()
  {
    this.Expect(IrTokenType.KeywordBlock);
    var name = this.ParseIdentifier();
    var builder = this.basicBlockBuilders[name];
    // Attach all attributes
    this.Expect(IrTokenType.Colon);
    this.ParseAttributeGroups(builder, AttributeTargets.BasicBlock);
    this.Expect(IrTokenType.Newline);
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
    // If this is a value-producing instruction, store the name
    string? valueName = null;
    var offset = 0;
    var temp = this.PeekIdentifier(ref offset);
    if (temp is not null && this.Source.TryLookAhead(offset, out var t) && t.Kind == IrTokenType.Assign)
    {
      // This is indeed a value name
      this.Source.Consume(offset + 1);
      valueName = temp;
    }
    // Get the syntax
    var name = this.ParseIdentifier();
    var syntax = this.context.GetInstructionSyntax(name);
    // Parse it
    var instr = syntax.Parse(this);
    // Parse the attribute groups that might follow
    var attrs = this.ParseAttributeGroups(AttributeTargets.Instruction);
    // Attach them
    AttachAttributes(instr, attrs);
    // If this instruction produced a value, store
    if (valueName is not null)
    {
      if (instr.ResultType is null) throw new InvalidOperationException($"The instruction {name} does not produce a value");
      this.valueInstructions[valueName] = instr;
    }
    // Eat newline
    this.EatNewline();
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
  /// Parses a <see cref="Value"/>.
  /// </summary>
  /// <param name="type">The <see cref="Type"/> of the value to parse.</param>
  /// <returns>The parsed <see cref="Value"/>.</returns>
  public Value ParseValue(Type type)
  {
    if (this.Source.TryPeek(out var peek) && peek.Kind == IrTokenType.Identifier)
    {
      // It's a referenced value
      // TODO: Not necessarily! An identifier could be a type or even a named constant later!
      var name = this.ParseIdentifier();
      var ins = this.valueInstructions[name];
      if (!type.Equals(ins.ResultType)) throw new NotImplementedException("TODO: type mismatch for referenced value");
      return new Value.Result(ins, name);
    }
    else
    {
      // It must be a constant
      var constant = this.ParseConstant(type);
      return new Value.Constant(constant);
    }
  }

  /// <summary>
  /// Parses a <see cref="Constant"/> of a given <see cref="Type"/>.
  /// </summary>
  /// <param name="type">The <see cref="Type"/> of <see cref="Constant"/> to parse.</param>
  /// <returns>The parsed <see cref="Constant"/>.</returns>
  public Constant ParseConstant(Type type) => type switch
  {
    Type.Int i => this.ParseIntConstant(i),
    _ => throw new ArgumentOutOfRangeException(nameof(type)),
  };

  /// <summary>
  /// Parses a <see cref="Type"/>.
  /// </summary>
  /// <returns>The parsed <see cref="Type"/>.</returns>
  public Type ParseType()
  {
    // TODO: Some more advanced type parsing
    var name = this.ParseIdentifier();
    var type = this.context.GetTypeDefinition(name);
    return type;
  }

  /// <summary>
  /// Parses a dot-separated identifier.
  /// </summary>
  /// <returns>The parsed identifier.</returns>
  public string ParseIdentifier()
  {
    var offset = 0;
    var ident = this.PeekIdentifier(ref offset);
    if (ident is null) throw new InvalidOperationException("identifier expected");
    this.Source.Consume(offset);
    return ident;
  }

  /// <summary>
  /// Expects a certain kind of token to come up in the input and consumes it.
  /// If the upcoming token is not of the specified kind, an error is raised.
  /// </summary>
  /// <param name="tokenType">The exact token kind to expect.</param>
  /// <returns>The consumed token.</returns>
  public IToken<IrTokenType> Expect(IrTokenType tokenType)
  {
    if (!this.Matches(tokenType, out var token))
    {
      throw new InvalidOperationException($"Syntax error, expected {tokenType}");
    }
    return token;
  }

  /// <summary>
  /// Checks, if the upcoming token is of a certain kind. If so, consumes it.
  /// </summary>
  /// <param name="tokenType">The token kind to check.</param>
  /// <returns>True, if the upcoming token had kind <paramref name="tokenType"/>.</returns>
  public bool Matches(IrTokenType tokenType) => this.Matches(tokenType, out _);

  /// <summary>
  /// Checks, if the upcoming token is of a certain kind. If so, consumes it.
  /// </summary>
  /// <param name="tokenType">The token kind to check.</param>
  /// <param name="token">The consumed token, if it matched the kind.</param>
  /// <returns>True, if the upcoming token had kind <paramref name="tokenType"/>.</returns>
  public bool Matches(IrTokenType tokenType, [MaybeNullWhen(false)] out IToken<IrTokenType> token)
  {
    if (this.Source.TryPeek(out token) && token.Kind == tokenType)
    {
      this.Source.Consume();
      return true;
    }
    else
    {
      return false;
    }
  }

  private Constant.Int ParseIntConstant(Type.Int type)
  {
    var negate = this.Matches(IrTokenType.Minus);
    var tok = this.Expect(IrTokenType.IntLiteral);
    var number = BigInteger.Parse(tok.Text);
    if (negate) number = -number;
    return new Constant.Int(type, number);
  }

  private void PreDefineProcedures()
  {
    for (var i = 0; this.Source.TryLookAhead(i, out var t);)
    {
      ++i;
      if (t.Kind != IrTokenType.KeywordProcedure) continue;

      // A procedure keyword is coming up, get the identifier
      var ident = this.PeekIdentifier(ref i);
      if (ident is not null) this.procedureBuilders[ident] = new ProcedureBuilder(ident);
    }
  }

  private void PreDefineBasicBlocks()
  {
    this.basicBlockBuilders.Clear();
    this.valueInstructions.Clear();
    for (var i = 0; this.Source.TryLookAhead(i, out var t);)
    {
      ++i;
      if (t.Kind == IrTokenType.KeywordProcedure) break;
      if (t.Kind != IrTokenType.KeywordBlock) continue;

      // A block keyword is coming up, get the identifier
      var ident = this.PeekIdentifier(ref i);
      if (ident is not null) this.basicBlockBuilders[ident] = new BasicBlockBuilder(ident);
    }
  }

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
      this.Source.Consume();
      this.Expect(IrTokenType.Colon);
    }
    return target;
  }

  private string? PeekIdentifier(ref int offset)
  {
    if (!this.Source.TryLookAhead(offset, out var t0) || t0.Kind != IrTokenType.Identifier) return null;
    var result = new StringBuilder(t0.Text);
    ++offset;
    while (this.Source.TryLookAhead(offset, out var tDot) && tDot.Kind == IrTokenType.Dot
        && this.Source.TryLookAhead(offset + 1, out var tName) && tName.Kind == IrTokenType.Identifier)
    {
      result.Append('.').Append(tName.Text);
      offset += 2;
    }
    return result.ToString();
  }

  private bool EatNewline() => this.Matches(IrTokenType.Newline);

  private void ParseAttributeGroups(IAttributeTarget attributeTarget, AttributeTargets defaultTarget)
  {
    var attribs = this.ParseAttributeGroups(defaultTarget);
    AttachAttributes(attributeTarget, attribs);
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
