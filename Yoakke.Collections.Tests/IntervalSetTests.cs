using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class IntervalSetTests : IntervalTestBase
    {
        [TestMethod]
        public void EmptySet()
        {
            var set = new IntervalSet<int>();

            Assert.IsTrue(set.SequenceEqual(IvList()));
        }

        [TestMethod]
        public void InsertIntoEmptySet()
        {
            var set = new IntervalSet<int>();

            set.Add(Iv("2..3"));

            Assert.IsTrue(set.SequenceEqual(IvList("2..3")));
        }

        [TestMethod]
        public void InsertIntoSetDisjunctBefore()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("2..3"));

            Assert.IsTrue(set.SequenceEqual(IvList("2..3", "5..7", "12..15")));
        }

        [TestMethod]
        public void InsertIntoSetDisjunctAfter()
        {
            var set = IvSet("2..3", "5..7");

            set.Add(Iv("12..15"));

            Assert.IsTrue(set.SequenceEqual(IvList("2..3", "5..7", "12..15")));
        }

        [TestMethod]
        public void InsertIntoSetDisjunctBetween()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("8..11"));

            Assert.IsTrue(set.SequenceEqual(IvList("5..7", "8..11", "12..15")));
        }

        [TestMethod]
        public void InsertIntoSetTouchFirst()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("3..5"));

            Assert.IsTrue(set.SequenceEqual(IvList("3..7", "12..15")));
        }

        [TestMethod]
        public void InsertIntoSetIntersectFirst()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("3..6"));

            Assert.IsTrue(set.SequenceEqual(IvList("3..7", "12..15")));
        }

        [TestMethod]
        public void InsertIntoSetTouchBetween()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("7..12"));

            Assert.IsTrue(set.SequenceEqual(IvList("5..15")));
        }

        [TestMethod]
        public void InsertIntoSetOverextendAll()
        {
            var set = IvSet("5..7", "12..15");

            set.Add(Iv("3..16"));

            Assert.IsTrue(set.SequenceEqual(IvList("3..16")));
        }

        [TestMethod]
        public void InvertEmpty()
        {
            var set = new IntervalSet<int>();

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..")));
            Assert.IsTrue(set.First().Lower.Type == BoundType.Unbounded && set.First().Upper.Type == BoundType.Unbounded);
        }

        [TestMethod]
        public void InvertFull()
        {
            var set = IvSet("..");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList()));
            Assert.AreEqual(0, set.Count);
        }

        [TestMethod]
        public void InvertSingleBounded()
        {
            var set = IvSet("3..5");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..3", "5..")));
        }

        [TestMethod]
        public void InvertTwoBounded()
        {
            var set = IvSet("3..5", "7..9");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..3", "5..7", "9..")));
        }

        [TestMethod]
        public void InvertManyEvenBounded()
        {
            var set = IvSet("3..5", "7..8", "10..14", "18..21");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..3", "5..7", "8..10", "14..18", "21..")));
        }

        [TestMethod]
        public void InvertManyOddBounded()
        {
            var set = IvSet("3..5", "7..8", "10..14", "18..21", "24..26");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..3", "5..7", "8..10", "14..18", "21..24", "26..")));
        }

        [TestMethod]
        public void InvertSingleUnboundedLeft()
        {
            var set = IvSet("..5");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("5..")));
        }

        [TestMethod]
        public void InvertSingleUnboundedRight()
        {
            var set = IvSet("5..");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..5")));
        }

        [TestMethod]
        public void InvertManyEvenUnboundedRight()
        {
            var set = IvSet("2..4", "6..7", "8..11", "14..");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..2", "4..6", "7..8", "11..14")));
        }

        [TestMethod]
        public void InvertManyOddUnboundedRight()
        {
            var set = IvSet("2..4", "6..7", "8..11", "14..16", "18..");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("..2", "4..6", "7..8", "11..14", "16..18")));
        }

        [TestMethod]
        public void InvertManyEvenUnboundedLeft()
        {
            var set = IvSet("..2", "6..7", "8..11", "14..16");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("2..6", "7..8", "11..14", "16..")));
        }

        [TestMethod]
        public void InvertManyOddUnboundedLeft()
        {
            var set = IvSet("..2", "6..7", "8..11", "14..16", "18..21");

            set.Invert();

            Assert.IsTrue(set.SequenceEqual(IvList("2..6", "7..8", "11..14", "16..18", "21..")));
        }
    }
}
