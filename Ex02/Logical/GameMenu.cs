using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class GameMenu
    {
        int m_BoardSize = 6;
        Player m_Player1;
        Player m_Player2;

        public GameMenu(int i_BoardSize, Player i_player1, Player i_player2 = null)
        {
            m_BoardSize = i_BoardSize;
            m_Player1 = i_player1;
            m_Player2 = i_player2;
        }
    }



}
