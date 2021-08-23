// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// Represents a single parameter of a procedure.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="Parameter"/>.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Ir.Type"/> of the parameter.</param>
        public Parameter(Type type)
        {
            this.Type = type;
        }
    }
}
