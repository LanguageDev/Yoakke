// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Passes
{
    /// <summary>
    /// Base class to simplify implementing <see cref="ICodePass"/>.
    /// </summary>
    public abstract class CodePassBase : ICodePass
    {
        /// <inheritdoc/>
        public virtual bool IsAssembly => false;

        /// <inheritdoc/>
        public virtual bool IsProcedure => false;

        /// <inheritdoc/>
        public virtual bool IsBasicBlock => false;

        /// <inheritdoc/>
        public virtual void Pass(IAssembly assembly) => throw new NotSupportedException();

        /// <inheritdoc/>
        public virtual void Pass(IProcedure procedure) => throw new NotSupportedException();

        /// <inheritdoc/>
        public virtual void Pass(IBasicBlock block) => throw new NotSupportedException();
    }
}
