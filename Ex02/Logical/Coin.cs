using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class Coin
    {
        private int m_Row = -1;
        public eSymbol m_Symbol = eSymbol.Player1;
        private bool m_IsKing = false;
        public int Row { get; set; }
        public int Col { get; set; }
        public Coin(eSymbol i_Symbol, int i_Row, int i_Col)
        {
            m_Symbol = i_Symbol;
            Row = i_Row;
            Col = i_Col;
        }
        public char GetSymbol()
        {
            return (char)m_Symbol;
        }
        public void SetSymbol(eSymbol i_Symbol)
        {
            m_Symbol = i_Symbol;
        }
        public bool IsKing()
        {
            return m_IsKing;
        }
        public void MoveTo(int i_NewRow, int i_NewCol)
        {
            Row = i_NewRow;
            Col = i_NewCol;
        }
        public void MakeKing()
        {
            m_IsKing = true;
        }
    }
}

