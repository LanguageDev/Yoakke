using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.Parser
{
    /// <summary>
    /// Represents a punctuated sequence of parsed values.
    /// </summary>
    /// <typeparam name="TValue">The punctuated value type.</typeparam>
    /// <typeparam name="TPunct">The punctuation element type.</typeparam>
    public class Punctuated<TValue, TPunct> : IReadOnlyList<PunctuatedValue<TValue, TPunct>>
    {
        private readonly IReadOnlyList<PunctuatedValue<TValue, TPunct>> underlying;

        /// <summary>
        /// The punctuated values.
        /// </summary>
        public IEnumerable<TValue> Values => this.underlying.Select(e => e.Value);

        /// <summary>
        /// The punctuations.
        /// </summary>
        public IEnumerable<TPunct> Punctuations => this.underlying
            .Select(e => e.Punctuation)
            .OfType<TPunct>();

        public int Count => this.underlying.Count;
        public PunctuatedValue<TValue, TPunct> this[int index] => this.underlying[index];

        public Punctuated()
            : this(Enumerable.Empty<PunctuatedValue<TValue, TPunct>>())
        {
        }

        public Punctuated(IReadOnlyList<PunctuatedValue<TValue, TPunct>> elements)
        {
            this.underlying = elements;
        }

        public Punctuated(IEnumerable<PunctuatedValue<TValue, TPunct>> elements)
        {
            this.underlying = elements.ToArray();
        }

        public IEnumerator<PunctuatedValue<TValue, TPunct>> GetEnumerator() => this.underlying.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        // 0 or more without trailing
        public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>)? elements)
        {
            if (elements == null) return new Punctuated<TValue, TPunct>();
            return elements.Value;
        }

        // 1 or more without trailing
        public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>) elements)
        {
            var result = new List<PunctuatedValue<TValue, TPunct>>();
            var prevValue = elements.Item1;
            foreach (var (punct, nextValue) in elements.Item2)
            {
                result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, punct));
                prevValue = nextValue;
            }
            result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, default));
            return new Punctuated<TValue, TPunct>(result);
        }

        // 0 or more with optional trailing separator
        public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>, TPunct)? elements)
        {
            if (elements == null) return new Punctuated<TValue, TPunct>();
            return elements.Value;
        }

        // 1 or more with optional trailing separator
        public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>, TPunct) elements)
        {
            var result = new List<PunctuatedValue<TValue, TPunct>>();
            var prevValue = elements.Item1;
            foreach (var (punct, nextValue) in elements.Item2)
            {
                result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, punct));
                prevValue = nextValue;
            }
            result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, elements.Item3));
            return new Punctuated<TValue, TPunct>(result);
        }
    }
}
