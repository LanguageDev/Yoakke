// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model
{
    public interface IEither
    {
        public object Value { get; }
    }

    [JsonConverter(typeof(EitherConverter))]
    public class Either<T1, T2> : IEither
    {
        public object Value { get; }

        public bool IsFirst => this.Value is T1;

        public bool IsSecond => this.Value is T2;

        public T1 AsFirst => (T1)this.Value;

        public T2 AsSecond => (T2)this.Value;

        protected Either(object value)
        {
            this.Value = value;
        }

        public Either(T1 value)
            : this((object)value!)
        {
        }

        public Either(T2 value)
            : this((object)value!)
        {
        }

        public static implicit operator Either<T1, T2>(T1 value) => new(value);

        public static implicit operator Either<T1, T2>(T2 value) => new(value);
    }

    [JsonConverter(typeof(EitherConverter))]
    public class Either<T1, T2, T3> : IEither
    {
        public object Value { get; }

        public bool IsFirst => this.Value is T1;

        public bool IsSecond => this.Value is T2;

        public bool IsThird => this.Value is T3;

        public T1 AsFirst => (T1)this.Value;

        public T2 AsSecond => (T2)this.Value;

        public T3 AsThird => (T3)this.Value;

        protected Either(object value)
        {
            this.Value = value;
        }

        public Either(T1 value)
            : this((object)value!)
        {
        }

        public Either(T2 value)
            : this((object)value!)
        {
        }

        public Either(T3 value)
            : this((object)value!)
        {
        }

        public static implicit operator Either<T1, T2, T3>(T1 value) => new(value);

        public static implicit operator Either<T1, T2, T3>(T2 value) => new(value);

        public static implicit operator Either<T1, T2, T3>(T3 value) => new(value);
    }
}
