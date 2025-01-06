namespace Ex02
{
    public struct PointOnBoard
    {
        private eRow m_Row;
        private eCol m_Col;

        public PointOnBoard(eRow i_Row, eCol i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
        }
        public eRow Row
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
        public eCol Col
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
    }
}
