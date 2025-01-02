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
        private List<Move> possibleMoves = new List<Move>();
        private PointOnBoard? m_TheCurrentCoinForDoubleJump = null;

        public GameSession(Player i_Player1, Player i_Player2, GameBoard i_GameBoard)
        {
            m_Player1 = i_Player1;
            m_Player2 = i_Player2;
            m_GameBoard = i_GameBoard;
            m_CurrentPlayer = m_Player1;
        }
        public Coin[,] GetGameBoardMatrix()
        {
            return m_GameBoard.getBoard();
        }

        public Player GetCurrentPlayer()
        {
            return m_CurrentPlayer;
        }

        public void Turn(Move i_UsersMove, out bool isMoveValid)
        {
            isMoveValid = false;
            if (m_TheCurrentCoinForDoubleJump != null)
            {
                if (ValidateDoubleTurn(i_UsersMove))
                {
                    performMove(i_UsersMove);
                    isMoveValid = true;
                }
            }
            else
            {
                if (ValidateMove(i_UsersMove))
                {
                    performMove(i_UsersMove);
                    isMoveValid = true;
                }
            }
        }

        private bool ValidateMove(Move i_UsersMove)
        {
            bool moveIsPossible = false;
            bool hasPotentialCaptureMove = false;
            bool userMoveIsCapture = false;
            bool moveIsValid = false;
            UpdatePossibleMoves();
            foreach (Move move in possibleMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    hasPotentialCaptureMove = true;
                    Console.WriteLine("{0},{1}", move.Start.Row, move.Start.Col, move.End.Row, move.End.Col);//check
                }

                if (move.Start.Equals(i_UsersMove.Start) && move.End.Equals(i_UsersMove.End))
                { 
                    moveIsPossible = true;
                    if(move.CheckIfTheMoveWasCaptureMove())
                    {
                        userMoveIsCapture = true;
                    }
                }
            }
            if ((hasPotentialCaptureMove == userMoveIsCapture) && moveIsPossible)
            {
                moveIsValid = true;
            }


            return moveIsValid;
        }

        private bool ValidateDoubleTurn(Move i_UsersMove)
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
                
                if (HasAnotherCapture(i_UsersMove.End))
                {
                    m_TheCurrentCoinForDoubleJump = i_UsersMove.End;
                }
                else
                {
                    m_TheCurrentCoinForDoubleJump = null;
                    SwitchPlayer();
                }
            }
            else
            {
                // Regular move, no capture
                m_TheCurrentCoinForDoubleJump = null;
                SwitchPlayer();
            }
            // Update the board with the new move
            m_GameBoard.UpdateBoard(i_UsersMove);

            // Check if the coin became a king after the move
            CheckIfCoinBecameKing(i_UsersMove.End);
        }


        private void CheckIfCoinBecameKing(PointOnBoard coinPosition)
        {
            int row = (int)coinPosition.Row;
            Coin coin = m_GameBoard.getBoard()[row, (int)coinPosition.Col];

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


        private bool HasAnotherCapture(PointOnBoard coinPosition)
        {
            List<Move> additionalMoves = m_GameBoard.GetPossibleMovesForCoin(coinPosition);

            foreach (Move move in additionalMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    return true;
                }
            }

            return false;
        }

        private void SwitchPlayer()
        {
            m_CurrentPlayer = (m_CurrentPlayer == m_Player1) ? m_Player2 : m_Player1;
        }

        public eGameState GameStatus()
        {
            List<Move> opponentPossibleMoves = GetOpponentPossibleMoves();
            int opponentPiecesCount = GetOpponentPiecesCount();

            if (m_TheCurrentCoinForDoubleJump != null)
            {
                return eGameState.AnotherTurn;
            }

            if (opponentPossibleMoves.Count == 0 || opponentPiecesCount == 0)
            {
                return eGameState.Win;
            }

            if (possibleMoves.Count == 0)
            {
                return eGameState.Draw;
            }

            return eGameState.Next;
        }

        private List<Move> GetOpponentPossibleMoves()
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

        private int GetOpponentPiecesCount()
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
            possibleMoves.Clear();

            for (int row = 0; row < m_GameBoard.getBoard().GetLength(0); row++)
            {
                for (int col = 0; col < m_GameBoard.getBoard().GetLength(1); col++)
                {
                    Coin coin = m_GameBoard.getBoard()[row, col];
                    if (coin != null && coin.m_Symbol == m_CurrentPlayer.Symbol)
                    {
                        List<Move> coinMoves = m_GameBoard.GetPossibleMovesForCoin(new PointOnBoard((eRow)row, (eCol)col));

                        foreach (Move move in coinMoves)
                        {
                            if (move.CheckIfTheMoveWasCaptureMove())
                            {
                                int capturedRow = ((int)move.Start.Row + (int)move.End.Row) / 2;
                                int capturedCol = ((int)move.Start.Col + (int)move.End.Col) / 2;

                                //כאן זה מוחק את המטבע שאנחנו רוצים לאכול ואז האוכל השני לא מזהה שהוא קיים
                                m_GameBoard.getBoard()[capturedRow, capturedCol] = null;
                            }
                        }

                        possibleMoves.AddRange(coinMoves);
                    }
                }
            }
        }
    }
}
