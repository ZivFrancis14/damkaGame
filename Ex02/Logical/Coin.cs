namespace Ex02
{
    public class Coin
    {
        private int m_Row = -1;
        private int m_Col =-1;
        public eSymbol m_Symbol = eSymbol.Player1;
        private bool m_IsKing = false;

        public Coin(eSymbol i_Symbol, int i_Row, int i_Col)
        {
            m_Symbol = i_Symbol;
            Row = i_Row;
            Col = i_Col;
        }
        public int Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = value;
            }
        }
        public int Col
        {
            get
            {
                return m_Col;
            }
            set
            {
                m_Col = value;
            }
        }
        public char Symbol
        {
            get
            {
                return (char)m_Symbol;
            }
        }
        public bool IsKing
        {
            get
            {
                return m_IsKing;
            }
            set
            {
                m_IsKing = value;
            }
        }
        public void SetSymbolByPlayerType(eSymbol i_Symbol)
        {
            m_Symbol = i_Symbol;
        }
    }
}

