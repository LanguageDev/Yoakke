
namespace Yoakke.Collections.Compatibility
{
    internal struct HashCode
    {
        public static int Combine(params object?[] values)
        {
            var h = new HashCode();
            foreach (var item in values) h.Add(item);
            return h.ToHashCode();
        }

        private int code;

        public void Add(object? obj)
        {
            if (obj is null)
            {
                this.code = CombineHashCodes(this.code, 0);
                return;
            }
            this.code = CombineHashCodes(this.code, obj.GetHashCode());
        }

        public int ToHashCode() => this.code;

        private static int CombineHashCodes(int code1, int code2)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + code1;
                hash = hash * 31 + code2;
                return hash;
            }
        }
    }
}

