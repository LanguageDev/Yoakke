
namespace Yoakke.Collections.Compatibility
{
    internal struct HashCode
    {
        public static int Combine(params object[] values)
        {
            var h = new HashCode();
            foreach (var item in values) h.Add(item);
            return h.ToHashCode();
        }

        private int code;

        public void Add(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                code = CombineHashCodes(code, 0);
                return;
            }
            code = CombineHashCodes(code, obj.GetHashCode());
        }

        public int ToHashCode() => code;

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

