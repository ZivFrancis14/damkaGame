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
        private Move m_CurrentMove;
        private GameBoard m_GameBoard;
        private List<Move> m_PossibleMoves = new List<Move>();
        private PointOnBoard? m_TheCurrentCoinForDoubleJump = null;

        public GameSession(Player i_Player1, Player i_Player2, GameBoard i_GameBoard)
        {
            m_Player1 = i_Player1;
            m_Player2 = i_Player2;
            m_GameBoard = i_GameBoard;
            m_CurrentPlayer = m_Player1;
        }
        public Move CurrentMove
        {
            get
            {
                return m_CurrentMove;
            }
        }
        public PointOnBoard? CurrentCoinForDoubleJump
        {
            get
            {
                return m_TheCurrentCoinForDoubleJump;
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
        public Coin[,] GetGameBoardMatrix()
        {
            return m_GameBoard.getBoard();
        }
        public Player GetCurrentPlayer()
        {
            return m_CurrentPlayer;
        }
        public void Turn(Move i_UsersMove, out bool o_IsMoveValid)
        {
            bool moveIsCapture = false;
            o_IsMoveValid = false;

            if (m_TheCurrentCoinForDoubleJump != null)
            {
                if (validateDoubleTurn(i_UsersMove))
                {
                    performMove(i_UsersMove);
                    o_IsMoveValid = true;
                }
            }
            else
            {
                if (validateMove(i_UsersMove, out moveIsCapture))
                {
                    i_UsersMove.IsCapture = moveIsCapture;
                    performMove(i_UsersMove);
                    o_IsMoveValid = true;
                }
            }
        }
        private bool validateMove(Move i_UsersMove, out bool o_MoveIsCapture)
        {
            bool moveIsPossible = false;
            bool hasPotentialCaptureMove = false;            
            bool moveIsValid = false;
            o_MoveIsCapture = false;
            UpdatePossibleMoves();

            foreach (Move move in m_PossibleMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    hasPotentialCaptureMove = true;
                }

                if (move.Start.Equals(i_UsersMove.Start) && move.End.Equals(i_UsersMove.End))
                { 
                    moveIsPossible = true;
                    if(move.CheckIfTheMoveWasCaptureMove())
                    {
                        o_MoveIsCapture = true;
                    }
                }
            }
            if ((hasPotentialCaptureMove == o_MoveIsCapture) && moveIsPossible)
            {
                moveIsValid = true;
                m_CurrentMove = i_UsersMove;
            }

            return moveIsValid;
        }
        private bool validateDoubleTurn(Move i_UsersMove)
        {
            if (m_TheCurrentCoinForDoubleJump != null && i_UsersMove.Start.Equals(m_TheCurrentCoinForDoubleJump))
            {
                List<Move> movesForCoin = m_GameBoard.GetPossibleMovesForCoin(m_TheCurrentCoinForDoubleJump.Value);
                foreach (Move move in movesForCoin)
                {
                    if (move.Start.Equals(i_UsersMove.Start) &&
                        move.End.Equals(i_UsersMove.End) &&
                        move.CheckIfTheMoveWasCaptureMove())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private void performMove(Move i_UsersMove)
        {
            // Handle capture logic if it's a capture move
            if (i_UsersMove.CheckIfTheMoveWasCaptureMove())
            {              
                if (hasAnotherCapture(i_UsersMove.End))
                {
                    m_TheCurrentCoinForDoubleJump = i_UsersMove.End;
                }
                else
                {
                    m_TheCurrentCoinForDoubleJump = null;
                    switchPlayer();
                }
            }
            else
            {
                // Regular move, no capture
                m_TheCurrentCoinForDoubleJump = null;
                switchPlayer();
            }
            // Update the board with the new move
            m_GameBoard.UpdateBoard(i_UsersMove);

            // Check if the coin became a king after the move
            checkIfCoinBecameKing(i_UsersMove.End);
        }
        private void checkIfCoinBecameKing(PointOnBoard i_CoinPosition)
        {
            int row = (int)i_CoinPosition.Row;
            Coin coin = m_GameBoard.getBoard()[row, (int)i_CoinPosition.Col];

            if (coin != null)
            {
                // Check if the coin reaches the last row for Player 1 or Player 2
                if ((coin.m_Symbol == m_Player1.Symbol && row == m_GameBoard.getBoard().GetLength(0) - 1) ||
                    (coin.m_Symbol == m_Player2.Symbol && row == 0))
                {
                    coin.MakeKing();
                    if(coin.GetSymbol() == (char)eSymbol.Player1)
                    {
                        coin.SetSymbol(eSymbol.KingPlayer1);
                    }
                    else
                    { 
                        coin.SetSymbol(eSymbol.KingPlayer2);
                    }                  
                }
            }
        }
        private bool hasAnotherCapture(PointOnBoard i_CoinPosition)
        {
            bool coinHasAnotherCapture = false;

            List<Move> additionalMoves = m_GameBoard.GetPossibleMovesForCoin(i_CoinPosition);

            foreach (Move move in additionalMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    coinHasAnotherCapture = true;
                    break;
                }
            }

            return coinHasAnotherCapture;
        }
        private void switchPlayer()
        {
            m_CurrentPlayer = (m_CurrentPlayer == m_Player1) ? m_Player2 : m_Player1;
        }
        public eGameState GameStatus()
        {
            List<Move> opponentPossibleMoves = getOpponentPossibleMoves();
            int opponentPiecesCount = getOpponentPiecesCount();

            if (m_TheCurrentCoinForDoubleJump != null)
            {
                return eGameState.AnotherTurn;
            }

            if (opponentPossibleMoves.Count == 0 || opponentPiecesCount == 0)
            {
                return eGameState.Win;
            }

            if (m_PossibleMoves.Count == 0)
            {
                return eGameState.Draw;
            }

            return eGameState.Next;
        }
        private List<Move> getOpponentPossibleMoves()
        {
            List<Move> opponentPossibleMoves = new List<Move>();

            for (int row = 0; row < m_GameBoard.getBoard().GetLength(0); row++)
            {
                for (int col = 0; col < m_GameBoard.getBoard().GetLength(1); col++)
                {
                    Coin coin = m_GameBoard.getBoard()[row, col];

                    if (coin != null && coin.m_Symbol != m_CurrentPlayer.Symbol)
                    {
                        opponentPossibleMoves.AddRange(m_GameBoard.GetPossibleMovesForCoin(new PointOnBoard((eRow)row, (eCol)col)));
                    }
                }
            }

            return opponentPossibleMoves;
        }
        private int getOpponentPiecesCount()
        {
            int opponentPiecesCount = 0;

            foreach (Coin coin in m_GameBoard.getBoard())
            {
                if (coin != null && coin.m_Symbol != m_CurrentPlayer.Symbol)
                {
                    opponentPiecesCount++;
                }
            }

            return opponentPiecesCount;
        }
        public void UpdatePossibleMoves()
        {
            m_PossibleMoves.Clear();

            for (int row = 0; row < m_GameBoard.getBoard().GetLength(0); row++)
            {
                for (int col = 0; col < m_GameBoard.getBoard().GetLength(1); col++)
                {
                    Coin coin = m_GameBoard.getBoard()[row, col];
                    if (coin != null && coin.m_Symbol == m_CurrentPlayer.Symbol)
                    {
                        List<Move> coinMoves = m_GameBoard.GetPossibleMovesForCoin(new PointOnBoard((eRow)row, (eCol)col));
                        m_PossibleMoves.AddRange(coinMoves);
                    }
                }
            }
        }
    }
}
