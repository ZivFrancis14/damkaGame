using System.Collections.Generic;

namespace Ex02
{
    public class GameBoard
    {
        private int m_BoardSize = 6;
        private Coin[,] m_MatrixBoard;
        public List<Move> m_possibleMoves = new List<Move>();

        public GameBoard(int i_BoardSize)
        {
            //if (i_BoardSize == 6 || i_BoardSize == 8 || i_BoardSize == 10)
            //{
                m_BoardSize = i_BoardSize;
                m_MatrixBoard = new Coin[m_BoardSize, m_BoardSize];
                InitCoinsOnBoard();
           // }
        }

        private void InitCoinsOnBoard()
        {
            int numOfRowsForEachPlayer = (m_BoardSize - 2) / 2;

            for (int row = 0; row < numOfRowsForEachPlayer; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        m_MatrixBoard[row, col] = new Coin(eSymbol.Player1, row, col);
                    }
                }
            }

            for (int row = m_BoardSize - numOfRowsForEachPlayer; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        m_MatrixBoard[row, col] = new Coin(eSymbol.Player2, row, col);
                    }
                }
            }
        }

        public Coin[,] GetBoard()
        {
            return m_MatrixBoard;
        }

        public void UpdateBoard(Move i_UsersMove)
        {
            int startRow = (int)i_UsersMove.Start.Row;
            int startCol = (int)i_UsersMove.Start.Col;
            int endRow = (int)i_UsersMove.End.Row;
            int endCol = (int)i_UsersMove.End.Col;

            if (i_UsersMove.CheckIfTheMoveWasCaptureMove())
            {
                int directionRow = (endRow - startRow) > 0 ? 1 : -1;
                int directionCol = (endCol - startCol) > 0 ? 1 : -1;
                int capturedRow = startRow + directionRow;
                int capturedCol = startCol + directionCol;

                m_MatrixBoard[capturedRow, capturedCol] = null;
            }

            m_MatrixBoard[endRow, endCol] = m_MatrixBoard[startRow, startCol];
            if (m_MatrixBoard[endRow, endCol] != null)
            {
                m_MatrixBoard[endRow, endCol].Row = endRow;
                m_MatrixBoard[endRow, endCol].Col = endCol;
            }

            m_MatrixBoard[startRow, startCol] = null;
        }

        public List<Move> GetPossibleMovesForCoin(PointOnBoard i_CoinPosition)
        {
            List<Move> possibleMovesForCoin = new List<Move>();
            CalculatePossibleMovesForCoin(i_CoinPosition, possibleMovesForCoin);
            return possibleMovesForCoin;
        }

        private void CalculatePossibleMovesForCoin(PointOnBoard i_CoinPosition, List<Move> i_PossibleMovesForCoin)
        {
            int[][] i_Directions;
            int i_Row = (int)i_CoinPosition.Row;
            int i_Col = (int)i_CoinPosition.Col;

            Coin i_Coin = m_MatrixBoard[i_Row, i_Col];
            if (i_Coin != null)
            {
                i_Directions = SetDirectionsByCoinType(i_Coin);

                foreach (var i_Direction in i_Directions)
                {
                    int i_NewRow = i_Row + i_Direction[0];
                    int i_NewCol = i_Col + i_Direction[1];

                    if (!CheckRegularMove(i_PossibleMovesForCoin, i_CoinPosition, i_NewRow, i_NewCol))
                    {
                        CheckCaptureMove(i_PossibleMovesForCoin, i_CoinPosition, i_Coin, i_NewRow, i_NewCol, i_Direction[0], i_Direction[1]);
                    }
                }
            }
            m_possibleMoves.AddRange(i_PossibleMovesForCoin);
        }

        private int[][] SetDirectionsByCoinType(Coin i_Coin)
        {
            int[][] directions = null;

            if (i_Coin.IsKing())
            {
                directions = new int[][]
                {
                    new int[] { -1, -1 },
                    new int[] { -1, 1 },
                    new int[] { 1, -1 },
                    new int[] { 1, 1 }
                };
            }
            else if (i_Coin.m_Symbol == eSymbol.Player1)
            {
                directions = new int[][]
                {
                    new int[] { 1, -1 },
                    new int[] { 1, 1 }
                };
            }
            else if (i_Coin.m_Symbol == eSymbol.Player2)
            {
                directions = new int[][]
                {
                    new int[] { -1, -1 },
                    new int[] { -1, 1 }
                };
            }

            return directions;
        }

        private bool CheckRegularMove(List<Move> i_PossibleMoves, PointOnBoard i_CoinPosition, int i_NewRow, int i_NewCol)
        {
            bool moveIsRegular = false;

            if (IsWithinBounds(i_NewRow, i_NewCol) && m_MatrixBoard[i_NewRow, i_NewCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition, new PointOnBoard((eRow)i_NewRow, (eCol)i_NewCol), false));
                moveIsRegular = true;
            }

            return moveIsRegular;
        }

        private void CheckCaptureMove(List<Move> i_PossibleMoves, PointOnBoard i_CoinPosition, Coin i_Coin, int i_NewRow, int i_NewCol, int i_DirectionRow, int i_DirectionCol)
        {
            int i_CaptureRow = i_NewRow + i_DirectionRow;
            int i_CaptureCol = i_NewCol + i_DirectionCol;

            if (IsWithinBounds(i_CaptureRow, i_CaptureCol) &&
                m_MatrixBoard[i_NewRow, i_NewCol] != null &&
                m_MatrixBoard[i_NewRow, i_NewCol].m_Symbol != i_Coin.m_Symbol &&
                m_MatrixBoard[i_CaptureRow, i_CaptureCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition, new PointOnBoard((eRow)i_CaptureRow, (eCol)i_CaptureCol), true));
            }
        }

        private bool IsWithinBounds(int i_Row, int i_Col)
        {
            bool isWithinBounds = i_Row >= 0 && i_Row < m_BoardSize && i_Col >= 0 && i_Col < m_BoardSize;
            return isWithinBounds;
        }
    }
}
