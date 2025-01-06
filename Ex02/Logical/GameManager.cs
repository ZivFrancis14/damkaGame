using System.Collections.Generic;

namespace Ex02
{
    public class GameManager
    {
        private Player m_Player1 = null;
        private Player m_Player2 = null;
        private GameBoard m_Board = null;
        private GameSession m_Session = null;
        private Dictionary<Player, int> m_TotalScore;

        public GameManager(string i_NamePlayer1, string i_NamePlayer2, bool i_IsPlayer2Human)
        {
            bool v_SetPlayerAsHuman = true;
            
            m_Player1 = new Player(i_NamePlayer1, v_SetPlayerAsHuman, eSymbol.Player1);
            m_Player2 = new Player(i_NamePlayer2, i_IsPlayer2Human, eSymbol.Player2);
            m_TotalScore = new Dictionary<Player, int>()
            {
                {m_Player1, 0}, {m_Player2, 0}
            };
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
        public Player Player1
        {
            get
            {
                return m_Player1;
            }
        }
        public Player Player2
        {
            get
            {
                return m_Player2;
            }
        }
        public void CreateNewSession(int i_sizeBoard)
        {
            m_Board = new GameBoard(i_sizeBoard);
            m_Session = new GameSession(m_Player1, m_Player2, m_Board);
        }     
        public void UpdateWinnersScore()
        {
            m_TotalScore[m_Session.CurrentPlayer] += m_Session.CalculatePointsDifference();
        }
        public int GetPlayerScore(Player player)
        {
            if (m_TotalScore.TryGetValue(player, out int score))
            {
                return score;
            }
            else
            {
                return 0;
            }
        }       
    }
}
