using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class ConsoleGameManager
    {
        private GameSession m_Session;
        private Player m_Player1;
        private Player m_Player2;

        public ConsoleGameManager()
        {
            //m_Session = new GameSession();

            m_Player1 = new Player("h", true,'c');
            m_Player1 = new Player("h", true, 'c');
        }

        public void StartGame()
        {
            m_Player1 = getPlayer();
            m_Player2 = getPlayer();

            m_Session = new GameSession(m_Player1, m_Player2,new GameBoard(6));


            gameLoop();
        }

        private void gameLoop()
        {
            while(!m_Session.IsGameOver)
            {
                //input from user
                //m_Session.Turn()
                //output to user (print board and score)
            }
        }

        private Player getPlayer()
        {
            //get player stat from user

            return new Player("h", true, 'c');
        }

        private void turn()
        {

        }

    }
}
