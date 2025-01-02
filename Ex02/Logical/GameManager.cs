using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    //כאן מנוהלת הלוגיקה של המשחק הכללי 
    public class GameManager
    {
        private Player m_Player1 = null;
        private Player m_Player2 = null;
        private GameBoard m_Board = null;
        private GameSession m_Session = null;
        public GameManager(string i_NamePlayer1, string i_NamePlayer2, bool i_IsPlayer2Human, int i_sizeBoard)
        {
            m_Player1 = new Player(i_NamePlayer1, true, eSymbol.Player1);
            m_Player2 = new Player(i_NamePlayer2, i_IsPlayer2Human, eSymbol.Player2);
            m_Board = new GameBoard(i_sizeBoard);
        }

        public GameSession Session
        {
            get
            {
                return m_Session;
            }
            set
            {
                m_Session = value;
            }
        }
  
        public void CreateNewSession()
        {
            m_Session = new GameSession(m_Player1, m_Player2, m_Board);
        }
     

        
    }
}
