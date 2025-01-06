using System;
using System.Collections.Generic;

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

        public Move CurrentMove
        {
            get
            {
                return m_CurrentMove;
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
        public Coin[,] GameBoardMatrix
        {
            get
            {
                return m_GameBoard.Board;
            }
        }
        public Player CurrentPlayer
        {
            get
            {
                return m_CurrentPlayer;
            }
        }
        public PointOnBoard? CurrentCoinForDoubleJump
        {
            get
            {
                return m_TheCurrentCoinForDoubleJump;
            }
        }
        public GameSession(Player i_Player1, Player i_Player2, GameBoard i_GameBoard)
        {
            m_Player1 = i_Player1;
            m_Player2 = i_Player2;
            m_GameBoard = i_GameBoard;
            m_CurrentPlayer = m_Player1;
        }
        public void Turn(Move i_UsersMove, out bool o_IsMoveValid)
        {
            o_IsMoveValid = false;
            bool moveIsCapture = false;

            if (m_TheCurrentCoinForDoubleJump != null)
            {
                if (validateDoubleTurn(i_UsersMove))
                {
                    performMove(i_UsersMove);
                    o_IsMoveValid = true;
                }
            }
            else if (validateMove(i_UsersMove, out moveIsCapture))
            {
                i_UsersMove.IsCapture = moveIsCapture;
                performMove(i_UsersMove);
                o_IsMoveValid = true;
            }
        }
        private bool validateMove(Move i_UsersMove, out bool o_MoveIsCapture)
        {
            o_MoveIsCapture = false;
            bool moveIsPossible = false;
            bool hasPotentialCaptureMove = false;
            bool moveIsValid = false;
            
            updatePossibleMoves();
            foreach (Move move in m_PossibleMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    hasPotentialCaptureMove = true;
                }

                if (move.Start.Equals(i_UsersMove.Start) && move.End.Equals(i_UsersMove.End))
                {
                    moveIsPossible = true;
                    if (move.CheckIfTheMoveWasCaptureMove())
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
            bool isValid = false;

            if (m_TheCurrentCoinForDoubleJump != null && i_UsersMove.Start.Equals(m_TheCurrentCoinForDoubleJump))
            {
                List<Move> movesForCoin = m_GameBoard.calculatePossibleMovesForCoin(m_TheCurrentCoinForDoubleJump.Value);

                foreach (Move move in movesForCoin)
                {
                    if (move.Start.Equals(i_UsersMove.Start) && move.End.Equals(i_UsersMove.End) && move.CheckIfTheMoveWasCaptureMove()) 
                    {
                        isValid = true;
                        break;
                    }
                }
            }

            return isValid;
        }
        private void performMove(Move i_UsersMove)
        {
            m_GameBoard.UpdateBoard(i_UsersMove);
            if (i_UsersMove.CheckIfTheMoveWasCaptureMove())
            {
                if (hasAnotherCapture(i_UsersMove.End))
                {
                    m_TheCurrentCoinForDoubleJump = i_UsersMove.End;
                }
                else
                {
                    m_TheCurrentCoinForDoubleJump = null;
                }
            }
            else
            {
                m_TheCurrentCoinForDoubleJump = null;
            }

            checkIfCoinBecameKing(i_UsersMove.End);
            if (m_TheCurrentCoinForDoubleJump == null && GameStatus() == eGameState.Next)
            {
                SwitchPlayer();
            }
        }
        private void checkIfCoinBecameKing(PointOnBoard i_CoinPosition)
        {
            int row = (int)i_CoinPosition.Row;
            Coin coin = m_GameBoard.Board[row, (int)i_CoinPosition.Col];

            if (coin != null)
            {
                bool isPlayer1King = isSamePlayerSymbol(coin.m_Symbol, eSymbol.Player1) && row == m_GameBoard.Board.GetLength(0) - 1;
                bool isPlayer2King = isSamePlayerSymbol(coin.m_Symbol, eSymbol.Player2) && row == 0;

                if (isPlayer1King || isPlayer2King)
                {
                    coin.IsKing = true;
                    coin.SetSymbolByPlayerType(isPlayer1King ? eSymbol.KingPlayer1 : eSymbol.KingPlayer2);
                }
            }
        }
        private bool hasAnotherCapture(PointOnBoard i_CoinPosition)
        {
            bool hasCapture = false;
            List<Move> additionalMoves = m_GameBoard.calculatePossibleMovesForCoin(i_CoinPosition);

            foreach (Move move in additionalMoves)
            {
                if (move.CheckIfTheMoveWasCaptureMove())
                {
                    hasCapture = true;
                    break;
                }
            }

            return hasCapture;
        }
        private void SwitchPlayer()
        {
            m_CurrentPlayer = (m_CurrentPlayer == m_Player1) ? m_Player2 : m_Player1;
        }
        public eGameState GameStatus()
        {
            eGameState gameState = eGameState.Next;

            if (m_PossibleMoves.Count == 0 || getPiecesCountForPlayer(m_CurrentPlayer) == 0)
            {
                gameState = eGameState.Win;
            }
            else if (getOpponentPossibleMoves().Count == 0 || getPiecesCountForPlayer(getOpponentPlayer()) == 0)
            {
                gameState = eGameState.Win;
            }
            else if (m_TheCurrentCoinForDoubleJump != null)
            {
                gameState = eGameState.AnotherTurn;
            }

            return gameState;
        }
        private List<Move> getOpponentPossibleMoves()
        {
            List<Move> opponentPossibleMoves = new List<Move>();

            for (int row = 0; row < m_GameBoard.Board.GetLength(0); row++)
            {
                for (int col = 0; col < m_GameBoard.Board.GetLength(1); col++)
                {
                    Coin coin = m_GameBoard.Board[row, col];

                    if (coin != null && !isSamePlayerSymbol(coin.m_Symbol, m_CurrentPlayer.Symbol))
                    {
                        opponentPossibleMoves.AddRange(m_GameBoard.calculatePossibleMovesForCoin(new PointOnBoard((eRow)row, (eCol)col)));
                    }
                }
            }

            return opponentPossibleMoves;
        }
        private int getPiecesCountForPlayer(Player i_Player)
        {
            int piecesCount = 0;

            foreach (Coin coin in m_GameBoard.Board)
            {
                if (coin != null && isSamePlayerSymbol(coin.m_Symbol, i_Player.Symbol))
                {
                    piecesCount++;
                }
            }

            return piecesCount;
        }
        private void updatePossibleMoves()
        {
            m_PossibleMoves.Clear();

            for (int row = 0; row < m_GameBoard.Board.GetLength(0); row++)
            {
                for (int col = 0; col < m_GameBoard.Board.GetLength(1); col++)
                {
                    Coin coin = m_GameBoard.Board[row, col];

                    if (coin != null && isSamePlayerSymbol(coin.m_Symbol, m_CurrentPlayer.Symbol))
                    {
                        m_PossibleMoves.AddRange(m_GameBoard.calculatePossibleMovesForCoin(new PointOnBoard((eRow)row, (eCol)col)));
                    }
                }
            }
        }
        private bool isSamePlayerSymbol(eSymbol i_Symbol1, eSymbol i_symbol2)
        {
            bool isSameSymbol = false;

            if ((i_Symbol1 == eSymbol.Player1 || i_Symbol1 == eSymbol.KingPlayer1) &&
                (i_symbol2 == eSymbol.Player1 || i_symbol2 == eSymbol.KingPlayer1))
            {
                isSameSymbol = true;
            }
            else if ((i_Symbol1 == eSymbol.Player2 || i_Symbol1 == eSymbol.KingPlayer2) &&
                     (i_symbol2 == eSymbol.Player2 || i_symbol2 == eSymbol.KingPlayer2))
            {
                isSameSymbol = true;
            }

            return isSameSymbol;
        }
        private Player getOpponentPlayer()
        {
            return (m_CurrentPlayer == m_Player1) ? m_Player2 : m_Player1;
        }
        public int CalculatePointsDifference()
        {
            int player1Points = 0;
            int player2Points = 0;

            foreach (Coin coin in m_GameBoard.Board)
            {
                if (coin != null)
                {
                    if (isSamePlayerSymbol(coin.m_Symbol, m_Player1.Symbol))
                    {
                        player1Points += (coin.IsKing ? 4 : 1);
                    }
                    else if (isSamePlayerSymbol(coin.m_Symbol, m_Player2.Symbol))
                    {
                        player2Points += (coin.IsKing ? 4 : 1);
                    }
                }
            }

            return Math.Abs(player1Points - player2Points);
        }
        public Move CalculateComputerMove()
        {
            Move selectedMove = new Move();
            bool isCaptureMoveAvailable = false;
            int randomIndex = 0;

            updatePossibleMoves();

            foreach (Move possibleMove in m_PossibleMoves)
            {
                if (possibleMove.CheckIfTheMoveWasCaptureMove())
                {
                    selectedMove = possibleMove;
                    isCaptureMoveAvailable = true;
                    break;
                }
            }

            if (!isCaptureMoveAvailable && m_PossibleMoves.Count > 0)
            {
                randomIndex = new Random().Next(m_PossibleMoves.Count);
                selectedMove = m_PossibleMoves[randomIndex];
            }

            return selectedMove;
        }
    }
}
