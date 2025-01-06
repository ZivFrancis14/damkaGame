using System.Collections.Generic;

namespace Ex02
{
    public class GameBoard
    {
        private readonly int r_BoardSize = 6;
        private readonly Coin[,] r_MatrixBoard;
        private List<Move> m_possibleMoves = new List<Move>();

        public GameBoard(int i_BoardSize)
        {
            r_BoardSize = i_BoardSize;
            r_MatrixBoard = new Coin[r_BoardSize, r_BoardSize];
            initCoinsOnBoard();
        }
        public Coin[,] Board
        {
            get
            {
                return r_MatrixBoard;
            }
        }
        public List<Move> calculatePossibleMovesForCoin(PointOnBoard i_CoinPosition)
        {
            List<Move> possibleMovesForCoin = new List<Move>();

            calculatePossibleMovesForCoin(i_CoinPosition, possibleMovesForCoin);
            return possibleMovesForCoin;
        }
        private void initCoinsOnBoard()
        {
            int numOfRowsForEachPlayer = (r_BoardSize - 2) / 2;

            for (int row = 0; row < numOfRowsForEachPlayer; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        r_MatrixBoard[row, col] = new Coin(eSymbol.Player1, row, col);
                    }
                }
            }

            for (int row = r_BoardSize - numOfRowsForEachPlayer; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        r_MatrixBoard[row, col] = new Coin(eSymbol.Player2, row, col);
                    }
                }
            }
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

                r_MatrixBoard[capturedRow, capturedCol] = null;
            }

            r_MatrixBoard[endRow, endCol] = r_MatrixBoard[startRow, startCol];
            if (r_MatrixBoard[endRow, endCol] != null)
            {
                r_MatrixBoard[endRow, endCol].Row = endRow;
                r_MatrixBoard[endRow, endCol].Col = endCol;
            }

            r_MatrixBoard[startRow, startCol] = null;
        }
        private void calculatePossibleMovesForCoin(PointOnBoard i_CoinPosition, List<Move> i_PossibleMovesForCoin)
        {
            int[][] i_Directions;
            int i_Row = (int)i_CoinPosition.Row;
            int i_Col = (int)i_CoinPosition.Col;
            Coin i_Coin = r_MatrixBoard[i_Row, i_Col];

            if (i_Coin != null)
            {
                i_Directions = setDirectionsByCoinType(i_Coin);
                foreach (var i_Direction in i_Directions)
                {
                    int i_NewRow = i_Row + i_Direction[0];
                    int i_NewCol = i_Col + i_Direction[1];
                    if (!checkRegularMove(i_PossibleMovesForCoin, i_CoinPosition, i_NewRow, i_NewCol))
                    {
                        checkCaptureMove(i_PossibleMovesForCoin, i_CoinPosition, i_Coin, i_NewRow, i_NewCol, i_Direction[0], i_Direction[1]);
                    }
                }
            }

            m_possibleMoves.AddRange(i_PossibleMovesForCoin);
        }
        private int[][] setDirectionsByCoinType(Coin i_Coin)
        {
            int[][] directions = null;

            if (i_Coin.IsKing)
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
        private bool checkRegularMove(List<Move> i_PossibleMoves, PointOnBoard i_CoinPosition, int i_NewRow, int i_NewCol)
        {
            bool moveIsRegular = false;

            if (isWithinBounds(i_NewRow, i_NewCol) && r_MatrixBoard[i_NewRow, i_NewCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition, new PointOnBoard((eRow)i_NewRow, (eCol)i_NewCol), false));
                moveIsRegular = true;
            }

            return moveIsRegular;
        }
        private void checkCaptureMove(List<Move> i_PossibleMoves, PointOnBoard i_CoinPosition, Coin i_Coin, int i_NewRow, int i_NewCol, int i_DirectionRow, int i_DirectionCol)
        {
            int i_CaptureRow = i_NewRow + i_DirectionRow;
            int i_CaptureCol = i_NewCol + i_DirectionCol;

            if (isWithinBounds(i_CaptureRow, i_CaptureCol) &&  r_MatrixBoard[i_NewRow, i_NewCol] != null &&
                r_MatrixBoard[i_NewRow, i_NewCol].m_Symbol != i_Coin.m_Symbol && r_MatrixBoard[i_CaptureRow, i_CaptureCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition, new PointOnBoard((eRow)i_CaptureRow, (eCol)i_CaptureCol), true));
            }
        }
        private bool isWithinBounds(int i_Row, int i_Col)
        {
            bool isWithinBounds = i_Row >= 0 && i_Row < r_BoardSize && i_Col >= 0 && i_Col < r_BoardSize;

            return isWithinBounds;
        }
    }
}
