namespace Ex02
{
    public class Player
    {
        private readonly string r_Name = string.Empty;
        private readonly bool r_IsHuman = false;
        private readonly eSymbol r_Symbol = eSymbol.Player1;
        private int m_CurrentNumberOfCoins = 0;

        public Player(string i_Name, bool i_IsHuman, eSymbol i_Symbol)
        {
            r_Name = i_Name;
            r_IsHuman = i_IsHuman;
            r_Symbol = i_Symbol;
        }
        public string Name
        {
            get 
            { 
                return r_Name; 
            }
        }
        public bool IsHuman
        {
            get 
            { 
                return r_IsHuman; 
            }
        }  
        public eSymbol Symbol
        {
            get { return r_Symbol;}
        }
        public int CurrentNumberOfCoins
        {
            get
            {
                return m_CurrentNumberOfCoins;
            }
        }
        public void AddCoin(Coin i_Coin)
        {
            m_CurrentNumberOfCoins++;
        }
        public void RemoveCoin(Coin i_Coin)
        {
            m_CurrentNumberOfCoins--;  
        }
    }
}
