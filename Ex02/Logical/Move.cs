namespace Ex02
{
    public struct Move
    {
        private readonly PointOnBoard r_Start;
        private readonly PointOnBoard r_End;
        private bool m_IsCapture;

        public Move(PointOnBoard start, PointOnBoard end, bool isCapture = false)
        {
            r_Start = start;
            r_End = end;
            m_IsCapture = isCapture;
        }
        public PointOnBoard Start
        {
            get
            { 
                return r_Start; 
            }
        }
        public PointOnBoard End
        {
            get 
            { 
                return r_End; 
            }
        }
        public bool IsCapture
        {
            set
            {
                m_IsCapture = value;
            }
        }
        public bool CheckIfTheMoveWasCaptureMove()
        {
            return m_IsCapture;
        }
    }
}
