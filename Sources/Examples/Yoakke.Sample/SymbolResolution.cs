// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.Symbols;

namespace Yoakke.Sample
{
    public class SymbolResolution
    {
        // Define scopes
        private class ScopePass : AstNode.PassVisitor
        {
            public Dictionary<AstNode, IScope> Scopes { get; } = new();

            private readonly SymbolTable symbolTable;

            public ScopePass(SymbolTable symbolTable)
            {
                this.symbolTable = symbolTable;
            }

            public void Pass(Statement statement) => this.Visit(statement);

            protected override void Visit(Statement statement)
            {
                this.Scopes[statement] = this.symbolTable.CurrentScope;
                base.Visit(statement);
            }

            protected override void Visit(Expression expression)
            {
                this.Scopes[expression] = this.symbolTable.CurrentScope;
                base.Visit(expression);
            }

            protected override void Visit(Statement.List list)
            {
                this.symbolTable.PushScope(parent => new Scope<ScopeKind>(parent, ScopeKind.Local));
                base.Visit(list);
                this.symbolTable.PopScope();
            }

            protected override void Visit(Statement.Func list)
            {
                this.symbolTable.PushScope(parent => new Scope<ScopeKind>(parent, ScopeKind.Function));
                base.Visit(list);
                this.symbolTable.PopScope();
            }
        }

        // Define order-independent constants (so far only functions)
        private class ConstPass : AstNode.PassVisitor
        {
            public Dictionary<AstNode, ISymbol> Symbols { get; } = new();

            private readonly Dictionary<AstNode, IScope> scopes;

            public ConstPass(Dictionary<AstNode, IScope> scopes)
            {
                this.scopes = scopes;
            }

            public void Pass(Statement statement) => this.Visit(statement);

            protected override void Visit(Statement.Func func)
            {
                var scope = this.scopes[func];
                if (scope.Symbols.ContainsKey(func.Name))
                {
                    throw new InvalidOperationException($"{func.Name} already defined in scope");
                }
                var symbol = new ConstSymbol(scope, func.Name, func);
                this.Symbols[func] = symbol;
                scope.DefineSymbol(symbol);
                base.Visit(func);
            }
        }

        // Define order-dependent variables
        private class VarPass : AstNode.PassVisitor
        {
            public Dictionary<object, ISymbol> Symbols { get; } = new();

            private readonly Dictionary<AstNode, IScope> scopes;

            public VarPass(Dictionary<AstNode, IScope> scopes)
            {
                this.scopes = scopes;
            }

            public void Pass(Statement statement) => this.Visit(statement);

            protected override void Visit(Statement.Var var)
            {
                var scope = this.scopes[var];
                var symbol = new VarSymbol(scope, var.Name);
                scope.DefineSymbol(symbol);
                this.Symbols[var] = symbol;
                base.Visit(var);
            }

            protected override void Visit(Statement.Func func)
            {
                var innerScope = this.scopes[func.Body];
                foreach (var param in func.Parameters)
                {
                    var symbol = new VarSymbol(innerScope, param);
                    innerScope.DefineSymbol(symbol);
                    this.Symbols[(func, param)] = symbol;
                }
                this.Visit(func.Body);
            }

            protected override void Visit(Expression.Ident ident)
            {
                var scope = this.scopes[ident];
                var symbol = scope.ReferenceSymbol(ident.Name);
                if (symbol == null)
                {
                    throw new InvalidOperationException($"unknown symbol {ident.Name}");
                }
                if (symbol is VarSymbol && !symbol.Scope.IsGlobal)
                {
                    // Symbol is a variable and not a global one
                    // That means it can't cross function boundlaries
                    var lookup = scope.ReferenceSymbol(ident.Name, s => ((Scope<ScopeKind>)s).Tag != ScopeKind.Function);
                    if (lookup == null)
                    {
                        throw new InvalidOperationException($"variable {ident.Name} is not global or function-local");
                    }
                }
                this.Symbols[ident] = symbol;
            }
        }

        public SymbolTable SymbolTable { get; private set; } = new SymbolTable(new Scope<ScopeKind>(ScopeKind.Global));

        public IReadOnlyDictionary<AstNode, IReadOnlyScope> Scopes => this.scopes;

        public IReadOnlyDictionary<object, ISymbol> Symbols => this.symbols;

        private readonly Dictionary<AstNode, IReadOnlyScope> scopes = new();
        private readonly Dictionary<object, ISymbol> symbols = new();

        public void Resolve(Statement program)
        {
            var pass1 = new ScopePass(this.SymbolTable);
            pass1.Pass(program);
            var pass2 = new ConstPass(pass1.Scopes);
            pass2.Pass(program);
            var pass3 = new VarPass(pass1.Scopes);
            pass3.Pass(program);

            foreach (var (k, v) in pass1.Scopes) this.scopes[k] = v;
            foreach (var (k, v) in pass2.Symbols) this.symbols[k] = v;
            foreach (var (k, v) in pass3.Symbols) this.symbols[k] = v;
        }
    }
}
