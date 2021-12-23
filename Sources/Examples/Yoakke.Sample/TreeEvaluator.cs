// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Symbols;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.Sample;

[Visitor(typeof(AstNode))]
[Visitor(typeof(Expression), ReturnType = typeof(object))]
public partial class TreeEvaluator
{
  private class Return : Exception
  {
    public readonly object? Value;

    public Return(object? value)
    {
      this.Value = value;
    }
  }

  private class StackFrame
  {
    public readonly Dictionary<ISymbol, object> Values = new();
  }

  private readonly SymbolResolution symbolResolution;
  private readonly StackFrame globalFrame = new();
  private readonly Stack<StackFrame> callStack = new();

  public TreeEvaluator(SymbolResolution symbolResolution)
  {
    this.symbolResolution = symbolResolution;
  }

  public void Execute(Statement stmt) => this.Visit(stmt);

  public object Evaluate(Expression expr) => this.Visit(expr);

  // Statement related ///////////////////////////////////////////////////

  /// <inheritdoc/>
  protected void Visit(Statement.Var vars)
  {
    var symbol = this.symbolResolution.Symbols[vars];
    this.Bind(symbol, this.Visit(vars.Value));
  }

  /// <inheritdoc/>
  protected void Visit(Statement.Func func)
  { /* no-op */
  }

  /// <inheritdoc/>
  protected void Visit(Statement.Return ret) =>
      throw new Return(ret.Value == null ? null : this.Visit(ret.Value));

  /// <inheritdoc/>
  protected void Visit(Statement.If iff)
  {
    if ((bool)this.Visit(iff.Condition)) this.Visit(iff.Then);
    else if (iff.Else != null) this.Visit(iff.Else);
  }

  /// <inheritdoc/>
  protected void Visit(Statement.While whil)
  {
    while ((bool)this.Visit(whil.Condition)) this.Visit(whil.Body);
  }

  // Expression related //////////////////////////////////////////////////

  /// <inheritdoc/>
  protected object? Visit(Expression.Call call)
  {
    var func = this.Visit(call.Function);
    var args = call.Arguments.Select(this.Visit).ToArray();
    if (func is Statement.Func langFunc)
    {
      // Language function
      if (langFunc.Parameters.Count != args.Length)
      {
        throw new InvalidOperationException("argc mismatch");
      }
      // Push frame
      var frame = new StackFrame();
      this.callStack.Push(frame);
      // Bind arguments
      foreach (var (name, value) in langFunc.Parameters.Zip(args))
      {
        var symbol = this.symbolResolution.Symbols[(langFunc, name)];
        frame.Values[symbol] = value;
      }
      // Evaluate, return value
      object? returnValue = null;
      try
      {
        this.Visit(langFunc.Body);
      }
      catch (Return ret)
      {
        returnValue = ret.Value;
      }
      // Pop frame
      this.callStack.Pop();
      return returnValue;
    }
    else if (func is Func<object[], object> nativeFunc)
    {
      // Native function
      return nativeFunc(args);
    }
    else
    {
      throw new InvalidOperationException("tried to call non-function");
    }
  }

  /// <inheritdoc/>
  protected object Visit(Expression.Unary ury) => ury.Op switch
  {
    UnaryOp.Negate => -(int)this.Visit(ury.Subexpr),
    UnaryOp.Ponate => (int)this.Visit(ury.Subexpr),
    UnaryOp.Not => !(bool)this.Visit(ury.Subexpr),

    _ => throw new NotSupportedException(),
  };

  /// <inheritdoc/>
  protected object Visit(Expression.Binary bin) => bin.Op switch
  {
    BinOp.Add => PerformAdd(this.Visit(bin.Left), (int)this.Visit(bin.Right)),

    BinOp.Sub => (int)this.Visit(bin.Left) - (int)this.Visit(bin.Right),
    BinOp.Mul => (int)this.Visit(bin.Left) * (int)this.Visit(bin.Right),
    BinOp.Div => (int)this.Visit(bin.Left) / (int)this.Visit(bin.Right),
    BinOp.Mod => (int)this.Visit(bin.Left) % (int)this.Visit(bin.Right),

    BinOp.And => (bool)this.Visit(bin.Left) && (bool)this.Visit(bin.Right),
    BinOp.Or => (bool)this.Visit(bin.Left) || (bool)this.Visit(bin.Right),

    BinOp.Eq => EqualityComparer<object>.Default.Equals(this.Visit(bin.Left), this.Visit(bin.Right)),
    BinOp.Neq => !EqualityComparer<object>.Default.Equals(this.Visit(bin.Left), this.Visit(bin.Right)),

    BinOp.Greater => (int)this.Visit(bin.Left) > (int)this.Visit(bin.Right),
    BinOp.Less => (int)this.Visit(bin.Left) < (int)this.Visit(bin.Right),
    BinOp.GreaterEq => (int)this.Visit(bin.Left) >= (int)this.Visit(bin.Right),
    BinOp.LessEq => (int)this.Visit(bin.Left) <= (int)this.Visit(bin.Right),

    // TODO
    BinOp.Assign => throw new NotImplementedException(),

    _ => throw new NotSupportedException(),
  };

  /// <inheritdoc/>
  protected object? Visit(Expression.Ident ident)
  {
    var symbol = this.symbolResolution.Symbols[ident];
    if (symbol is VarSymbol)
    {
      // Variable
      if (this.callStack.TryPeek(out var top) && top.Values.TryGetValue(symbol, out var value)) return value;
      return this.globalFrame.Values[symbol];
    }
    else
    {
      // Constant
      return ((ConstSymbol)symbol).Value;
    }
  }

  /// <inheritdoc/>
  protected object Visit(Expression.StringLit strLit) => strLit.Value;

  /// <inheritdoc/>
  protected object Visit(Expression.IntLit intLit) => intLit.Value;

  // Runtime drivers /////////////////////////////////////////////////////

  private object Bind(ISymbol symbol, object value)
  {
    if (this.callStack.TryPeek(out var top))
    {
      return top.Values[symbol] = value;
    }
    else
    {
      return this.globalFrame.Values[symbol] = value;
    }
  }

  private static object PerformAdd(object left, object right)
  {
    if (left is string || right is string) return $"{left}{right}";
    return (int)left + (int)right;
  }
}
