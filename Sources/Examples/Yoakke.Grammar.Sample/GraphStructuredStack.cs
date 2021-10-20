// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Graphs;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.ParseTree;

namespace Yoakke.Grammar.Sample
{
    public class VertexLayer
    {
        public IDictionary<int, StateVertex> StateVertices { get; } = new Dictionary<int, StateVertex>();

        public void Clear()
        {
            this.StateVertices.Clear();
        }

        public StateVertex? Push(StateVertex prevState, IIncrementalTreeNode node, int state)
        {
            // We can only share a symbol vertex, if we share the state vertex
            // So first we check if we share a state vertex
            if (this.StateVertices.TryGetValue(state, out var nextState))
            {
                // There is a state vertex we can share, there's a possibility we can share the symbol vertex too
                if (nextState.PrevMap.TryGetValue(node, out var nextSymbol))
                {
                    // Everything is cached, nothing to do, just connect them up
                    nextSymbol.Prev.Add(prevState);
                }
                else
                {
                    // The next symbol does not exist yet
                    nextSymbol = new(prevState, node);
                    nextState.PrevMap.Add(node, nextSymbol);
                }
                // Anyway, the state exists
                return null;
            }
            else
            {
                // The state does not exist yet, we need to add it
                // In this case, we don't share the symbols either
                var nextSymbol = new SymbolVertex(prevState, node);
                nextSymbol.Prev.Add(prevState);
                nextState = new(nextSymbol, state);
                this.StateVertices.Add(state, nextState);
                return nextState;
            }
        }
    }

    public class GraphStructuredStack : INondetStack
    {
        public ILrParsingTable ParsingTable { get; }

        public int? CurrentState => this.shiftLayer.StateVertices.Count == 1
            ? this.shiftLayer.StateVertices.Keys.First()
            : null;

        public IEnumerable<IIncrementalTreeNode> Trees => this.shiftLayer.StateVertices.Values
            .Where(h => h.PrevMap.Count > 0)
            .SelectMany(h => h.PrevMap.Values.Select(v => v.ParseTree));

        // Layer caches
        private readonly VertexLayer reduceLayer = new();
        private readonly VertexLayer shiftLayer = new();

        // Temporary vertex heads
        private readonly HashSet<StateVertex> oldHeads = new();

        // Action tracking
        private readonly Stack<(StateVertex Vertex, Reduce Reduce)> remainingReduces = new();
        private readonly Stack<(StateVertex Vertex, Shift Shift)> remainingShifts = new();

        // Current terminal
        private IIncrementalTreeNode? currentNode;

        public GraphStructuredStack(ILrParsingTable table)
        {
            this.ParsingTable = table;
            // Initial state
            this.shiftLayer.StateVertices.Add(0, new());
        }

        public string ToDot()
        {
            var result = new StringBuilder();
            result.AppendLine("graph GSS {");
            result.AppendLine("  rankdir=RL;");

            var allHeads = this.oldHeads.Concat(this.shiftLayer.StateVertices.Values).ToHashSet();

            // We number each vertex
            var vertexNames = new Dictionary<Vertex, int>();
            foreach (var v in allHeads.Cast<Vertex>().SelectMany(n => BreadthFirst.Search(n, v => v.Prev)))
            {
                if (vertexNames.ContainsKey(v)) continue;
                vertexNames.Add(v, vertexNames.Count);
            }

            // For the old and current heads we create a subgraph to align them
            result.AppendLine("  subgraph Heads {");
            result.AppendLine($"    {{rank=same {string.Join(" ", allHeads.Select(h => vertexNames[h]))}}}");
            foreach (var h in allHeads)
            {
                var reductionsOnHead = this.remainingReduces
                    .Where(r => ReferenceEquals(r.Vertex, h))
                    .Select(r => r.Reduce);
                var shiftsOnHead = this.remainingShifts
                    .Where(s => ReferenceEquals(s.Vertex, h))
                    .Select(s => s.Shift);
                var opsOnHead = reductionsOnHead.Cast<Lr.Action>().Concat(shiftsOnHead);
                result.AppendLine($"    {vertexNames[h]}[label=\"{h.State}\", shape=circle, xlabel=\"{string.Join(@"\l", opsOnHead)}\"];");
            }
            result.AppendLine("  }");

            // Define all other nodes
            foreach (var vertex in vertexNames.Keys.Except(allHeads))
            {
                result.Append($"  {vertexNames[vertex]}[");
                if (vertex is StateVertex state)
                {
                    result.Append($"label=\"{state.State}\", shape=circle");
                }
                else
                {
                    var symbol = (SymbolVertex)vertex;
                    result.Append($"label=\"{symbol.Symbol}\", shape=square");
                }
                result.AppendLine("]");
            }

            // Connections
            foreach (var v1 in vertexNames.Keys)
            {
                foreach (var v2 in v1.Prev) result.AppendLine($"  {vertexNames[v1]} -- {vertexNames[v2]}");
            }

            result.Append("}");

            result.Replace("->", "→");

            return result.ToString();
        }

        public void Feed(IIncrementalTreeNode currentNode)
        {
            // If there are remaining actions to perform, feeding another terminal is illegal
            if (this.remainingReduces.Count > 0 || this.remainingShifts.Count > 0) throw new InvalidOperationException("Not all actions are performed yet");

            // We add the old heads
            this.oldHeads.Clear();
            foreach (var h in this.shiftLayer.StateVertices.Values) this.oldHeads.Add(h);

            // Clear cache
            this.shiftLayer.Clear();
            this.reduceLayer.Clear();

            // We store the current node
            this.currentNode = currentNode;

            // Then we push each action for each head
            foreach (var head in this.oldHeads) this.PushActions(head);
        }

        public bool Step()
        {
            // We always reduce everywhere first
            if (this.remainingReduces.TryPop(out var r))
            {
                this.Reduce(r.Vertex, r.Reduce);
                return true;
            }
            // We do the shifts all at once
            var result = this.remainingShifts.Count > 0;
            while (this.remainingShifts.TryPop(out var s)) this.Shift(s.Vertex, s.Shift);
            // If there are no more shifts, we are done
            return result;
        }

        private bool IsActive(StateVertex vertex) =>
               this.remainingReduces.Any(r => ReferenceEquals(r.Vertex, vertex))
            || this.remainingShifts.Any(s => ReferenceEquals(s.Vertex, vertex));

        private void Reduce(StateVertex vertex, Reduce reduce)
        {
            // If the vertex is not a vertex anymore, we remove it from the old heads
            if (!this.IsActive(vertex))
            {
                this.oldHeads.Remove(vertex);
                this.reduceLayer.StateVertices.Remove(vertex.State);
            }
            // Now we need to pop off |b| amount of symbol vertices for an X -> b reduction
            // The thing is, nodes can contain multiple trees, so this all becomes a parallel mess very quickly
            var newRoots = PopAndCollect(vertex, reduce.Production.Right.Count)
                .ToList()
                .GroupBy(v => v.State)
                .ToDictionary(g => g.Key, g => g.Select(v => v.Nodes).ToList());
            // We have all the new roots, all of them get a symbol and state pushed on
            foreach (var (root, nodeLists) in newRoots)
            {
                // Check what state we result in
                var stateGoto = this.ParsingTable.Goto[root.State, reduce.Production.Left];
                // If nothing, we terminate this branch
                if (stateGoto is null) continue;
                // Otherwise we push on the symbol and the state
                var trees = nodeLists.Select(nodes =>
                    new ProductionIncrementalTreeNode(reduce.Production, stateGoto.Value, nodes)
                    {
                        IsReusable = this.CurrentState is not null,
                    });
                // Fir each tree we try to push
                foreach (var tree in trees)
                {
                    var pushedVertex = this.reduceLayer.Push(root, tree, stateGoto.Value);
                    // If the vertex existed, we don't do anything
                    if (pushedVertex is null) continue;
                    // For debugging we push on this node as an old head, will be deleted by a shift anyway
                    this.oldHeads.Add(pushedVertex);
                    // Now we add all actions that can be performed on this new state for the current terminal for further processing
                    this.PushActions(pushedVertex);
                }
            }
        }

        private void Shift(StateVertex vertex, Shift shift)
        {
            Debug.Assert(this.currentNode is not null, "The current node cannot be null.");
            // The vertex is surely out of the heads now
            this.oldHeads.Remove(vertex);
            // Now we try to push on the symbol and next state
            this.shiftLayer.Push(vertex, this.currentNode, shift.State);
        }

        private static IEnumerable<(StateVertex State, List<IIncrementalTreeNode> Nodes)> PopAndCollect(StateVertex initial, int count)
        {
            static IEnumerable<(StateVertex State, List<IIncrementalTreeNode>)> PopAndCollectImpl(
                StateVertex currentVertex,
                int count,
                Stack<IIncrementalTreeNode> currentPopped)
            {
                if (count == 0)
                {
                    // We have arived at the pop target
                    yield return (currentVertex, currentPopped.ToList());
                }
                else
                {
                    // We need to pop further
                    var groups = Pop(currentVertex).GroupBy(v => v.State);
                    foreach (var group in groups)
                    {
                        var nextVertex = group.Key;
                        foreach (var sym in group.Select(v => v.Symbol))
                        {
                            currentPopped.Push(sym.ParseTree);
                            foreach (var r in PopAndCollectImpl(nextVertex, count - 1, currentPopped)) yield return r;
                            currentPopped.Pop();
                        }
                    }
                }
            }

            return PopAndCollectImpl(initial, count, new());
        }

        private static IEnumerable<(StateVertex State, SymbolVertex Symbol)> Pop(StateVertex vertex) =>
            vertex.PrevMap.Values.SelectMany(prevSym => prevSym.Prev.Select(prevState => (prevState, prevSym)));

        private void PushActions(StateVertex vertex)
        {
            Debug.Assert(this.currentNode is not null, "The current node cannot be null.");
            var actions = this.ParsingTable.Action[vertex.State, this.currentNode.FirstTerminal];
            foreach (var action in actions)
            {
                if (action is Shift s) this.remainingShifts.Push((vertex, s));
                else if (action is Reduce r) this.remainingReduces.Push((vertex, r));
                // NOTE: else it's an accept
            }
        }
    }
}
