using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool IsFirst => Value is T1;
        public bool IsSecond => Value is T2;

        public T1 AsFirst => (T1)Value;
        public T2 AsSecond => (T2)Value;

        protected Either(object value)
        {
            Value = value;
        }

        public Either(T1 value)
            : this((object)value!)
        {
        }

        public Either(T2 value)
            : this((object)value!)
        {
        }

        public static implicit operator Either<T1, T2>(T1 value) => new Either<T1, T2>(value);
        public static implicit operator Either<T1, T2>(T2 value) => new Either<T1, T2>(value);
    }

    [JsonConverter(typeof(EitherConverter))]
    public class Either<T1, T2, T3> : IEither
    {
        public object Value { get; }

        public bool IsFirst => Value is T1;
        public bool IsSecond => Value is T2;
        public bool IsThird => Value is T3;

        public T1 AsFirst => (T1)Value;
        public T2 AsSecond => (T2)Value;
        public T3 AsThird => (T3)Value;

        protected Either(object value)
        {
            Value = value;
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

        public static implicit operator Either<T1, T2, T3>(T1 value) => new Either<T1, T2, T3>(value);
        public static implicit operator Either<T1, T2, T3>(T2 value) => new Either<T1, T2, T3>(value);
        public static implicit operator Either<T1, T2, T3>(T3 value) => new Either<T1, T2, T3>(value);
    }
}
