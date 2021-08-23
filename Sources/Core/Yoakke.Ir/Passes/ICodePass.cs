// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Ir.Model;

namespace Yoakke.Ir.Passes
{
    /// <summary>
    /// Represents some pass that modifies the IR code.
    /// </summary>
    public interface ICodePass
    {
        /// <summary>
        /// True, if this pass can be applied to <see cref="IAssembly"/>s.
        /// </summary>
        public bool IsAssembly { get; }

        /// <summary>
        /// True, if this pass can be applied to <see cref="IProcedure"/>s.
        /// </summary>
        public bool IsProcedure { get; }

        /// <summary>
        /// True, if this pass can be applied to <see cref="IBasicBlock"/>s.
        /// </summary>
        public bool IsBasicBlock { get; }

        /// <summary>
        /// Does a pass on an <see cref="IAssembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="IAssembly"/> to do the pass on.</param>
        public void Pass(IAssembly assembly);

        /// <summary>
        /// Does a pass on an <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="procedure">The <see cref="IProcedure"/> to do the pass on.</param>
        public void Pass(IProcedure procedure);

        /// <summary>
        /// Does a pass on an <see cref="IBasicBlock"/>.
        /// </summary>
        /// <param name="block">The <see cref="IBasicBlock"/> to do the pass on.</param>
        public void Pass(IBasicBlock block);
    }
}
