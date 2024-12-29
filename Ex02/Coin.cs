using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class Coin
    {
        private int m_Row = -1;
        private char m_Symbol { get; }
        public int Row {
            get {  return m_Row; }
            set
            {
                if (m_Row == -1 && value > 0)
                {
                    m_Row = value;
                }
            }
        }
        private int m_Col { get; set; }
        private bool m_IsKing { get; set; }

        public Coin(char i_Symbol, int i_Row, int i_Col)
        {
            m_Symbol = i_Symbol;
            Row = i_Row;
            m_Col = i_Col;
            m_IsKing = false;
        }

        public void MoveTo(int i_NewRow, int i_NewCol)
        {
            Row = i_NewRow;
            m_Col = i_NewCol;
        }

        public void MakeKing()
        {
            m_IsKing = true;
        }


    }
}

