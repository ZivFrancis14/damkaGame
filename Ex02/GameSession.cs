using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class GameSession
    {
        private Player m_Player1;
        private Player m_Player2;
        private Player m_CurrentPlayer;
        private GameBoard m_GameBoard;
        //private bool m_IsGameOver = false;

        public bool IsGameOver { get; set; }

        public GameSession(Player i_Player1, Player i_Player2, GameBoard i_GameBoard)
        {
            m_Player1 = i_Player1;
            m_Player2 = i_Player2;
            m_GameBoard = i_GameBoard;
            m_CurrentPlayer = m_Player1; 
            IsGameOver = false;
        }

        public void Turn(Coin i_Coin, int direction)
        {
            //move the coin in the direction 
            //update board
            //update score
            //switch turn
        }

        public bool ValidPlay(int row, int col, int direction)
        {
            Squre s = m_GameBoard.getBoard()[row, col];

            if(m_CurrentPlayer == m_Player1 && s  == Squre.AKing)
            {
                return true;
            }
        }
    }

}
