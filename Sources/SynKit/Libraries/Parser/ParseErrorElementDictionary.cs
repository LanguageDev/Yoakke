// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.SynKit.Parser;

internal class ParseErrorElementDictionary : IReadOnlyDictionary<string, ParseErrorElement>
{
    private string? firstKey;
    private ParseErrorElement? firstItem;
    private string? secondKey;
    private ParseErrorElement? secondItem;
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

    public ParseErrorElementDictionary(string firstKey, ParseErrorElement firstValue, string secondKey, ParseErrorElement secondValue)
    {
        this.firstKey = firstKey;
        this.firstItem = firstValue;
        this.secondKey = secondKey;
        this.secondItem = secondValue;
    }

    public ParseErrorElement this[string key] => this.firstKey is null
        ? this.elements is null
            ? throw new KeyNotFoundException($"The key {key} was not found in the dictionary")
            : this.elements[key]
        : this.firstKey == key
            ? this.firstItem!
            : this.secondKey == key
            ? this.secondItem!
            : throw new KeyNotFoundException($"The key {key} was not found in the dictionary");

    public IEnumerable<string> Keys =>
        this.elements is not null
            ? this.elements.Keys
            : (this.secondKey is null) ? [this.firstKey!] : [this.firstKey!, this.secondKey];

    public IEnumerable<ParseErrorElement> Values =>
        this.elements is not null
            ? this.elements.Values
            : (this.secondKey is null) ? [this.firstItem!] : [this.firstItem!, this.secondItem!];

    public int Count => this.elements is null
        ? this.firstKey is null ? 0
            : this.secondKey is null ? 1 : 2
        : this.elements.Count;

    public bool ContainsKey(string key)
    {
        if (this.elements is not null)
        {
            return this.elements.ContainsKey(key);
        }
        else if (this.firstKey is null)
        {
            return false;
        }
        else
        {
            return this.firstKey == key || (this.secondKey is null ? false : this.firstKey == key);
        }
    }

    public IEnumerator<KeyValuePair<string, ParseErrorElement>> GetEnumerator() =>
        this.elements is null
            ? (secondKey is null)
                ? new Enumerator(this.firstKey!, this.firstItem!) : new TwoElementEnumerator(this.firstKey!, this.firstItem!, this.secondKey, this.secondItem)
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

            if (this.secondKey == key)
            {
                value = secondItem;
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
            if (other.elements is not null)
            {
                return new(new(other.elements));
            }

            if (this.firstKey == other.firstKey)
            {
                return new(other.firstKey!, other.firstItem!.CreateMergedElement(this.firstItem!));
            }
            else
            {
                if (this.secondKey is null)
                {
                    return new(this.firstKey!, this.firstItem!, other.firstKey!, other.firstItem!);
                }
                return new(new Dictionary<string, ParseErrorElement>
                {
                    { this.firstKey!, this.firstItem! },
                    { this.secondKey!, this.secondItem! },
                    { other.firstKey!, other.firstItem! },
                });
            }
        }

        var elements = this.elements.Values.ToDictionary(e => e.Context, e => new ParseErrorElement(e.Expected, e.Context));
        foreach (var element in other.Values)
        {
            if (elements.TryGetValue(element.Context, out var part))
            {
                foreach (var e in element.Expected) part.Expected.Add(e);
            }
            else
            {
                part = new(element.Expected, element.Context);
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

    private struct TwoElementEnumerator : IEnumerator<KeyValuePair<string, ParseErrorElement>>
    {
        private int consumed;
        private KeyValuePair<string, ParseErrorElement> _firstPair;
        private KeyValuePair<string, ParseErrorElement> _secondPair;
        public TwoElementEnumerator(string firstKey, ParseErrorElement firstValue, string secondKey, ParseErrorElement secondValue)
        {
            this._firstPair = new KeyValuePair<string, ParseErrorElement>(firstKey, firstValue);
            this._secondPair = new KeyValuePair<string, ParseErrorElement>(secondKey, secondValue);
        }

        public KeyValuePair<string, ParseErrorElement> Current
        {
            get
            {
                if (consumed > 2)
                {
                    throw new InvalidOperationException("The enumerator is not valid.");
                }

                return consumed == 1 ? _firstPair : _secondPair;
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose() { }
        public bool MoveNext()
        {
            if (this.consumed >= 2)
            {
                this.consumed++;
                return false;
            }
            else
            {
                this.consumed++;
                return true;
            }
        }
        public void Reset()
        {
            this.consumed = 0;
        }
    }
}
