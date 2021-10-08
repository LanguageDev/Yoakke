// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.ParseTree;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// A simple, deterministic LR parser.
    /// </summary>
    public sealed class LrParser
    {
        /// <summary>
        /// The table that drives this parser.
        /// </summary>
        public ILrParsingTable Table { get; }

        /// <summary>
        /// The stack of result elements.
        /// </summary>
        public IReadOnlyCollection<IParseTreeNode> ResultStack => this.resultStack;

        /// <summary>
        /// The stack of states.
        /// </summary>
        public IReadOnlyCollection<int> StateStack => this.stateStack;

        private readonly Stack<IParseTreeNode> resultStack = new();
        private readonly Stack<int> stateStack = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LrParser"/> class.
        /// </summary>
        /// <param name="table">The parsing thable that drives this stack.</param>
        public LrParser(ILrParsingTable table)
        {
            this.Table = table;
        }

        /// <summary>
        /// Parses an input.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="values">The input values to parse.</param>
        /// <returns>The sequence of actions taken.</returns>
        public IEnumerable<Action> Parse<T>(IEnumerable<T> values) => this.Parse(values, o => new(o!));

        /// <summary>
        /// Parses an input.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="values">The input values to parse.</param>
        /// <param name="toTerminal">The function to turn the value into a terminal.</param>
        /// <returns>The sequence of actions taken.</returns>
        public IEnumerable<Action> Parse<T>(IEnumerable<T> values, Func<T, Terminal> toTerminal)
        {
            this.resultStack.Clear();
            this.stateStack.Clear();

            var inputEnumerator = values.GetEnumerator();
            Terminal NextInput() => inputEnumerator.MoveNext()
                ? toTerminal(inputEnumerator.Current!)
                : Terminal.EndOfInput;

            // Initial state
            this.stateStack.Push(0);
            var a = NextInput();

            while (true)
            {
                // Act on the current state
                var s = this.stateStack.Peek();
                var currentActions = this.Table.Action[s, a];
                if (currentActions.Count != 1) throw new InvalidOperationException();
                var currentAction = currentActions.First();

                if (currentAction is Shift shift)
                {
                    this.stateStack.Push(shift.State);
                    this.resultStack.Push(new LeafParseTreeNode(a, a.Value));
                    a = NextInput();

                    yield return shift;
                }
                else if (currentAction is Reduce reduce)
                {
                    var children = new List<IParseTreeNode>();
                    for (var i = 0; i < reduce.Production.Right.Count; ++i)
                    {
                        this.stateStack.Pop();
                        children.Add(this.resultStack.Pop());
                    }
                    children.Reverse();
                    var t = this.stateStack.Peek();
                    this.stateStack.Push(this.Table.Goto[t, reduce.Production.Left]!.Value);
                    this.resultStack.Push(new ProductionParseTreeNode(reduce.Production, children));

                    yield return reduce;
                }
                else if (currentAction is Accept accept)
                {
                    yield return accept;
                    break;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
