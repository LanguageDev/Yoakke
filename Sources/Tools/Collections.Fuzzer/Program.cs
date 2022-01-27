using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Yoakke.Collections.Fuzzer;

internal class Program
{
    internal static void FuzzAvlTree(int maxElements)
    {
        var rnd = new Random();
        for (var epoch = 0; ; ++epoch)
        {
            if (epoch % 100 == 0) Console.WriteLine($"Epoch {epoch}...");

            var set = new AvlSet();
            var pulledNumbers = new HashSet<int>();
            Debug.Assert(set.IsValid(pulledNumbers));

            while (set.Count < maxElements)
            {
                var testCase = set.ToTestCaseString();

                var n = rnd.Next(0, maxElements * 4);
                var avlInsert = set.Insert(n);
                var oracleInsert = pulledNumbers.Add(n);
                if (avlInsert != oracleInsert || !set.IsValid(pulledNumbers))
                {
                    Console.WriteLine($"Error in AVL tree (avl: {avlInsert} vs {oracleInsert})");
                    Console.WriteLine("Test case:");
                    Console.WriteLine($"    Insert({n})");
                    Console.WriteLine($"    Tree: {testCase}");
                    return;
                }
            }

            while (set.Count > 0)
            {
                var testCase = set.ToTestCaseString();

                var n = rnd.Next(0, maxElements * 4);
                var avlDelete = set.Delete(n);
                var oracleDelete = pulledNumbers.Remove(n);
                if (avlDelete != oracleDelete || !set.IsValid(pulledNumbers))
                {
                    Console.WriteLine($"Error in AVL tree (avl: {avlDelete} vs {oracleDelete})");
                    Console.WriteLine("Test case:");
                    Console.WriteLine($"    Delete({n})");
                    Console.WriteLine($"    Tree: {testCase}");
                    return;
                }
            }
        }
    }

    internal static void Main(string[] args)
    {
        FuzzAvlTree(5);
    }
}
