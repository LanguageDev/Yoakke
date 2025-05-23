// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;

namespace Yoakke.SynKit.Parser;

internal class ParseErrorElementDictionary : IReadOnlyDictionary<string, ParseErrorElement>
{
    private string? firstKey;
    private ParseErrorElement? firstItem;
    private Dictionary<string, ParseErrorElement>? elements;

    private ParseErrorElementDictionary()
    {
    }

    private ParseErrorElementDictionary(Dictionary<string, ParseErrorElement> elements)
    {
        this.elements = elements;
    }

    public ParseErrorElementDictionary(string key, ParseErrorElement value)
    {
        this.firstKey = key;
        this.firstItem = value;
    }

    public ParseErrorElement this[string key] => this.firstKey is null
        ? this.elements is null
            ? throw new KeyNotFoundException($"The key {key} was not found in the dictionary")
            : this.elements[key]
        : this.firstKey == key
            ? this.firstItem!
            : throw new KeyNotFoundException($"The key {key} was not found in the dictionary");

    public IEnumerable<string> Keys => this.firstKey is null ? this.elements!.Keys : new[] { this.firstKey };

    public IEnumerable<ParseErrorElement> Values => this.firstKey is null ? this.elements!.Values : new[] { this.firstItem! };

    public int Count => this.firstKey is null ? this.elements is null ? 0 : this.elements.Count : 1;

    public bool ContainsKey(string key) => this.firstKey is null ? this.elements is null ? false : this.elements.ContainsKey(key) : this.firstKey == key;
    public IEnumerator<KeyValuePair<string, ParseErrorElement>> GetEnumerator() =>
        this.elements is null
            ? new Enumerator(this.firstKey!, this.firstItem!)
            : this.elements.GetEnumerator();
    public bool TryGetValue(string key, out ParseErrorElement value)
    {
        if (this.elements is null)
        {
            if (this.firstKey == key)
            {
                value = firstItem;
                return true;
            }

            value = default;
            return false;
        }

        return this.elements.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public ParseErrorElementDictionary Merge(ParseErrorElementDictionary other)
    {
        if (this.elements is null)
        {
            if (other.elements is null)
            {
                if (this.firstKey == other.firstKey)
                {
                    var newExpected = other.firstItem!.Expected.ToHashSet();
                    newExpected.UnionWith(this.firstItem!.Expected);
                    return new(other.firstKey!, new (newExpected, this.firstItem.Context));
                }
                else
                {
                    return new(new Dictionary<string, ParseErrorElement>
                    {
                        { this.firstKey!, this.firstItem! },
                        { other.firstKey!, other.firstItem! },
                    });
                }
            }
            else
            {
                return new(new (other.elements));
            }
        }

        var elements = this.elements.Values.ToDictionary(e => e.Context, e => new ParseErrorElement(e.Expected.ToHashSet(), e.Context));
        foreach (var element in other.Values)
        {
            if (elements.TryGetValue(element.Context, out var part))
            {
                foreach (var e in element.Expected) part.Expected.Add(e);
            }
            else
            {
                part = new(element.Expected.ToHashSet(), element.Context);
                elements.Add(element.Context, part);
            }
        }

        return new (elements);
    }

    private struct Enumerator : IEnumerator<KeyValuePair<string, ParseErrorElement>>
    {
        private bool valid;
        private KeyValuePair<string, ParseErrorElement> _current;
        public Enumerator(string key, ParseErrorElement value)
        {
            this._current = new KeyValuePair<string, ParseErrorElement>(key, value);
        }

        public KeyValuePair<string, ParseErrorElement> Current
        {
            get
            {
                if (!valid)
                {
                    throw new InvalidOperationException("The enumerator is not valid.");
                }

                return _current;
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose() {}
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
