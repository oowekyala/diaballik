using System.Linq;
using NUnit.Framework;
using static Diaballik.Tests.MockUtil;

namespace Diaballik.Tests {
    // Not intended as extensive tests, mainly for debugging
    [TestFixture]
    public class TestUtilTest {
        [Test]
        public void TestOrderedPositions([Random(1, 50, 10)] int i) {
            var pool = OrderedPositionsPool(i).ToList();
            Assert.AreEqual(i * i, pool.Count);
            Assert.AreEqual(pool.Count, pool.Distinct().Count());
        }


        [Test]
        public void TestRandomPositionsNotOutOfRange([Random(1, 50, 10)] int i) {
            var pool = RandomPositions(i * i, i); // expect no exception
            // pool.ToList().Select(p => p.ToString()).ToList().ForEach(Console.WriteLine); // debug output
        }
    }
}