using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class GameBoard
    {
        private int m_BoardSize = 6;
        private Squre[,] m_MatrixBoard;
        private List<Coin> allCoins = new List<Coin>();

        public GameBoard(int i_BoardSize)
        {
            if (i_BoardSize == 6 || i_BoardSize == 8 || i_BoardSize == 10)
            {
                m_BoardSize = i_BoardSize;
                m_MatrixBoard = new Squre[m_BoardSize, m_BoardSize];
            }
            else
            {
                //error to user selection
            }
        }

        public Squre[,] getBoard()
        {
            return m_MatrixBoard;
        }



    }

    public enum Squre
    {
        None,
        A,
        AKing,
        B,
        BKing
    }
}
