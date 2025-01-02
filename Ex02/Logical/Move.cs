namespace Ex02
{
    public struct Move
    {
        private PointOnBoard m_Start;
        private PointOnBoard m_End;
        private bool m_IsCapture;

        public Move(PointOnBoard start, PointOnBoard end, bool isCapture = false)
        {
            m_Start = start;
            m_End = end;
            m_IsCapture = isCapture;
        }

        public PointOnBoard Start
        {
            get
            { 
                return m_Start; 
            }
        }

        public PointOnBoard End
        {
            get 
            { 
                return m_End; 
            }
        }

        public bool CheckIfTheMoveWasCaptureMove()
        {
            return m_IsCapture;
        }
    }
}
