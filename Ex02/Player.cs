using System;
using System.Collections.Generic;
using System.Text;

namespace Ex02
{
    public class Player
    {
        private readonly string r_Name = string.Empty;
        private readonly bool m_IsHuman = false;
        private readonly char m_Symbol = 'O';
        private List<Coin> m_Coins = new List<Coin>();

        public Player(string i_Name, bool i_IsHuman, char i_Symbol)
        {
            r_Name = i_Name;
            m_IsHuman = i_IsHuman;
            m_Symbol = i_Symbol;
        }

        public string Name
        {
            get { return r_Name; }
        }

        public bool IsHuman
        {
            get { return m_IsHuman; }
        }
        
        public char Symbol
        {
            get { return m_Symbol;}
        }

        public void AddCoin(Coin i_Coin)
        {
            m_Coins.Add(i_Coin);
        }

        public void RemoveCoin(Coin i_Coin)
        {
            m_Coins.Remove(i_Coin);  
        }

    }
}
