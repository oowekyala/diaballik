using System.Linq;
using NUnit.Framework;
using static CSDiaballik.Tests.TestUtil;

namespace CSDiaballik.Tests {
    // Not intended as extensive tests, mainly for debugging
    [TestFixture]
    public class TestUtilTest {

        [Test]
        public void TestOrderedPositions() {
            var testCases = RandomInts(10, 50);
            foreach (var i in testCases) {
                var pool = OrderedPositionsPool(i).ToList();
                Assert.AreEqual(i * i, pool.Count);
                Assert.AreEqual(pool.Count, pool.Distinct().Count());
            }
        }


        [Test]
        public void TestRandomPositionsNotOutOfRange() {
            var testCases = RandomInts(10, 50);
            foreach (var i in testCases) {
                var pool = RandomPositions(i * i, i); // expect no exception
                // pool.ToList().Select(p => p.ToString()).ToList().ForEach(Console.WriteLine); // debug output
            }
        }

    }
}
