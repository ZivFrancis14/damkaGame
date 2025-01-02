namespace Ex02
{
    public struct PointOnBoard
    {
        private eRow m_Row;
        private eCol m_Col;

        public PointOnBoard(eRow row, eCol col)
        {
            m_Row = row;
            m_Col = col;
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
