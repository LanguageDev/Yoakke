namespace Yoakke.Parser
{
    /// <summary>
    /// A single, punctuated element.
    /// </summary>
    /// <typeparam name="TValue">The punctuated element type.</typeparam>
    /// <typeparam name="TPunct">The punctuation element type.</typeparam>
    public readonly struct PunctuatedValue<TValue, TPunct>
    {
        /// <summary>
        /// The punctuated element.
        /// </summary>
        public readonly TValue Value;
        /// <summary>
        /// The punctuation that follows the element.
        /// </summary>
        public readonly TPunct? Punctuation;

        /// <summary>
        /// Initializes a new <see cref="PunctuatedValue{TElement, TPunctuation}"/>.
        /// </summary>
        /// <param name="element">The element that is punctuated.</param>
        /// <param name="punctuation">The punctuation element that follows the punctuated one.</param>
        public PunctuatedValue(TValue element, TPunct? punctuation)
        {
            Value = element;
            Punctuation = punctuation;
        }
    }
}
