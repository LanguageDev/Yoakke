// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Validation
{
    /// <summary>
    /// A cache for different validators.
    /// </summary>
    internal class ValidatorCache
    {
        private readonly Dictionary<Type, IInstructionValidator> instructionValidators = new();
        private readonly Dictionary<Type, IInstructionValidator> cachedValidators = new();

        /// <summary>
        /// Retrieves the <see cref="IInstructionValidator"/> appropriate for the given <see cref="IInstruction"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to get the validator for.</param>
        /// <returns>The appropriate validator for <paramref name="instruction"/>.</returns>
        public IInstructionValidator GetInstructionValidator(IInstruction instruction) =>
            this.GetInstructionValidator(instruction.GetType());

        /// <summary>
        /// Retrieves the <see cref="IInstructionValidator"/> appropriate for the given <see cref="IInstruction"/>.
        /// </summary>
        /// <param name="instructionType">The <see cref="Type"/> of the <see cref="IInstruction"/> to get the validator
        /// for.</param>
        /// <returns>The appropriate validator for the <paramref name="instructionType"/>.</returns>
        public IInstructionValidator GetInstructionValidator(Type instructionType)
        {
            if (!this.instructionValidators.TryGetValue(instructionType, out var validator))
            {
                // No validator for this instruction type yet, find what type of validator this instruction needs
                var validatorAttrib = instructionType.GetCustomAttribute<ValidatorAttribute>();
                if (validatorAttrib is null)
                {
                    throw new InvalidOperationException($"the instruction {instructionType.Name} has no associated validator");
                }

                // Check if this validator is already in the cache
                var validatorType = validatorAttrib.ValidatorType;
                if (!this.cachedValidators.TryGetValue(validatorType, out validator))
                {
                    // No instance of this validator yet, instantiate
                    var newValidator = Activator.CreateInstance(instructionType);
                    if (newValidator is null)
                    {
                        throw new InvalidOperationException($"could not instantiate validator {validatorType.Name}");
                    }
                    // Store it in the validator cache
                    validator = (IInstructionValidator)newValidator;
                    this.cachedValidators.Add(validatorType, validator);
                }

                // Now that we figured out the validator, cache it for this type
                this.instructionValidators.Add(instructionType, validator);
            }
            return validator;
        }
    }
}
