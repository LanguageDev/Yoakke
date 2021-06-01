using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model
{
    [JsonConverter(typeof(BoolOrObjectConverter))]
    public readonly struct BoolOrObject<T>
    {
        public readonly object Value;

        public bool IsBool => Value is bool;
        public bool IsObject => Value is T;

        public bool AsBool => (bool)Value;
        public T AsObject => (T)Value;

        private BoolOrObject(object value)
        {
            Value = value;
        }

        public BoolOrObject(bool value)
            : this((object)value)
        {
        }

        public BoolOrObject(T value)
            : this((object)value!)
        {
        }
    }
}
