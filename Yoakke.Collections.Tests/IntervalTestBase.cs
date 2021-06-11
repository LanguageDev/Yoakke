using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    public abstract class IntervalTestBase
    {
        protected static Interval<int> Iv(string s)
        {
            if (s == "..") return Interval<int>.Full();
            if (s.StartsWith("..=")) return new Interval<int>(LowerBound<int>.Unbounded(), UpperBound<int>.Inclusive(int.Parse(s.Substring(3))));
            if (s.StartsWith("..")) return new Interval<int>(LowerBound<int>.Unbounded(), UpperBound<int>.Exclusive(int.Parse(s.Substring(2))));
            if (s.EndsWith("..")) return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(s.Substring(0, s.Length - 2))), UpperBound<int>.Unbounded());
            if (s.Contains("..="))
            {
                var parts = s.Split("..=");
                return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(parts[0])), UpperBound<int>.Inclusive(int.Parse(parts[1])));
            }
            if (s.Contains(".."))
            {
                var parts = s.Split("..");
                return new Interval<int>(LowerBound<int>.Inclusive(int.Parse(parts[0])), UpperBound<int>.Exclusive(int.Parse(parts[1])));
            }
            throw new NotImplementedException();
        }

        protected static IntervalRelation<int> Rel(string i1, string i2) => Iv(i1).RelationTo(Iv(i2), Comparer<int>.Default);

        protected static IList<Interval<int>> IvList(params string[] ivs) => ivs.Select(Iv).ToList();

        protected static IntervalSet<int> IvSet(params string[] ivs)
        {
            var result = new IntervalSet<int>();
            foreach (var iv in IvList(ivs)) result.Add(iv);
            return result;
        }

        protected static IntervalMap<int, T> IvMap<T>(Func<T, T, T> update, params (string, T)[] ivs)
        {
            var result = new IntervalMap<int, T>();
            foreach (var (iv, v) in ivs) result.AddAndUpdate(Iv(iv), v, update);
            return result;
        }
    }
}
