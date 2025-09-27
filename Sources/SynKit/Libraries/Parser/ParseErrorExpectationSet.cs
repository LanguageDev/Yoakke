// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Yoakke.SynKit.Parser;

internal class ParseErrorExpectationSet : ISet<object>
{
    private object? firstItem;
    private HashSet<object>? elements;

    public ParseErrorExpectationSet(ISet<object> expected)
    {
        if (expected.Count == 1)
        {
            this.firstItem = expected.First();
        }
        else
        {
            this.elements = [.. expected];
        }
    }

    public ParseErrorExpectationSet(object expected)
    {
        this.firstItem = expected;
    }

    public int Count => this.elements is null ? this.firstItem is null ? 0 : 1 : this.elements.Count;

    public bool IsReadOnly => true;

    public bool Add(object item)
    {
        if (this.elements is not null)
        {
            return this.elements.Add(item);
        }

        if (this.firstItem is null)
        {
            this.firstItem = item;
            return true;
        }
        else
        {
            if (this.firstItem == item) return false;
            this.elements = new() { this.firstItem, item };
            return true;
        }
    }
    public void Clear()
    {
        this.firstItem = null;
        this.elements = null;
    }
    public bool Contains(object item)
    {
        if (this.elements is not null)
        {
            return this.elements.Contains(item);
        }

        return this.firstItem == item;
    }
    public void CopyTo(object[] array, int arrayIndex) => throw new NotImplementedException();
    public void ExceptWith(IEnumerable<object> other) => throw new NotImplementedException();
    public IEnumerator<object> GetEnumerator() => (this.elements is not null) ? this.elements.GetEnumerator() : new Enumerator(this.firstItem);
    public void IntersectWith(IEnumerable<object> other) => throw new NotImplementedException();
    public bool IsProperSubsetOf(IEnumerable<object> other) => throw new NotImplementedException();
    public bool IsProperSupersetOf(IEnumerable<object> other) => throw new NotImplementedException();
    public bool IsSubsetOf(IEnumerable<object> other) => throw new NotImplementedException();
    public bool IsSupersetOf(IEnumerable<object> other) => throw new NotImplementedException();
    public bool Overlaps(IEnumerable<object> other) => throw new NotImplementedException();
    public bool Remove(object item) => throw new NotImplementedException();
    public bool SetEquals(IEnumerable<object> other) => throw new NotImplementedException();
    public void SymmetricExceptWith(IEnumerable<object> other) => throw new NotImplementedException();
    public void UnionWith(IEnumerable<object> other) => throw new NotImplementedException();

    internal ParseErrorExpectationSet Merge(ParseErrorExpectationSet other)
    {
        if (this.elements is not null)
        {
            var newExpected = this.ToHashSet();
            newExpected.UnionWith(other);
            return new ParseErrorExpectationSet(newExpected);
        }

        if (this.firstItem is null)
        {
            return other;
        }

        if (this.firstItem == other.firstItem)
        {
            return this;
        }

        {
            var newExpected = this.ToHashSet();
            newExpected.UnionWith(other);
            return new ParseErrorExpectationSet(newExpected);
        }
    }
    void ICollection<object>.Add(object item) => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    private struct Enumerator : IEnumerator<object>
    {
        private bool valid;
        private readonly object? _current;
        public Enumerator(object? value)
        {
            this._current = value;
        }

        public object Current
        {
            get
            {
                if (!valid)
                {
                    throw new InvalidOperationException("The enumerator is not valid.");
                }

                return _current ?? throw new InvalidOperationException("The enumerator is not valid.");
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose() { }
        public bool MoveNext()
        {
            if (!this.valid)
            {
                this.valid = true;
                return true;
            }
            else
            {
                this.valid = false;
                return false;
            }
        }
        public void Reset()
        {
            this.valid = false;
        }
    }
}
