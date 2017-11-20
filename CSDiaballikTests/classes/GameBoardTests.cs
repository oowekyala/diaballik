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
    public class GameBoardTests
    {
        [TestMethod()]
        public void MovePieceTest()
        {
            NoobAiPlayer ai = new NoobAiPlayer();
            Piece p1 = new Piece(ai);
            GameBoard gb = new GameBoard();
        }
    }
}