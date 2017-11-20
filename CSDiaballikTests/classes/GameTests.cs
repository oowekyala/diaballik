using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSDiaballik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDiaballik.Tests
{
    [TestClass()]
    public class GameTests
    {
        [TestMethod()]
        public void GameTest()
        {
            GameBoard gb;
            AiPlayer p1;
            AiPlayer p2;
            Game g = new Game(gb, p1, p2);
            Assert.AreEqual(p1, g.Player1);
        }

        [TestMethod()]
        public void GameTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UpdateTest()
        {
            Assert.Fail();
        }
    }
}