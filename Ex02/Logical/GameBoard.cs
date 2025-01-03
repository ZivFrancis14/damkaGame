using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex02
{
    public class GameBoard
    {
        private int m_BoardSize = 6;
        private Coin[,] m_MatrixBoard;
        public List<Move> m_possibleMoves = new List<Move>();

        public GameBoard(int i_BoardSize)
        {
            if (i_BoardSize == 6 || i_BoardSize == 8 || i_BoardSize == 10)
            {
                m_BoardSize = i_BoardSize;
                m_MatrixBoard = new Coin[m_BoardSize, m_BoardSize];
                initCoinsOnBoard();
            }
            else
            {
                //error to user selection
            }
        }

        //check validation of coin row&col input
        private void initCoinsOnBoard()
        {
            // חישוב מספר השורות שבהן יניחו כלים לכל שחקן
            int numOfRowsForEachPlayer = (m_BoardSize - 2) / 2;

            // הנחת כלים עבור שחקן 1 (הנחת הכלים בשורות הראשונות של הלוח)
            for (int row = 0; row < numOfRowsForEachPlayer; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    // הנחת כלים רק על משבצות כהות
                    if ((row + col) % 2 != 0)
                    {
                        m_MatrixBoard[row, col] = new Coin(eSymbol.Player1, row, col);
                    }
                }
            }

            // הנחת כלים עבור שחקן 2 (הנחת הכלים בשורות האחרונות של הלוח)
            for (int row = m_BoardSize - numOfRowsForEachPlayer; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    // הנחת כלים רק על משבצות כהות
                    if ((row + col) % 2 != 0)
                    {
                        m_MatrixBoard[row, col] = new Coin(eSymbol.Player2, row, col);
                    }
                }
            }
        }
        public Coin[,] getBoard()
        {
            return m_MatrixBoard;
        }
        public void UpdateBoard(Move i_UsersMove)
        {
            int startRow = (int)i_UsersMove.Start.Row;
            int startCol = (int)i_UsersMove.Start.Col;
            int endRow = (int)i_UsersMove.End.Row;
            int endCol = (int)i_UsersMove.End.Col;

            // Handle capture move: dynamically calculate the captured position
            if (i_UsersMove.CheckIfTheMoveWasCaptureMove())
            {
                int directionRow = (endRow - startRow) > 0 ? 1 : -1; // Determine row direction
                int directionCol = (endCol - startCol) > 0 ? 1 : -1; // Determine column direction
                int capturedRow = startRow + directionRow;
                int capturedCol = startCol + directionCol;

                // Remove the captured coin
                m_MatrixBoard[capturedRow, capturedCol] = null;
            }

            // Move the coin to the new position
            m_MatrixBoard[endRow, endCol] = m_MatrixBoard[startRow, startCol];
            if (m_MatrixBoard[endRow, endCol] != null)
            {
                m_MatrixBoard[endRow, endCol].Row = endRow;
                m_MatrixBoard[endRow, endCol].Col = endCol;
            }

            // Remove the coin from the start position
            m_MatrixBoard[startRow, startCol] = null;

        }
        private void CalculatePossibleMovesForCoin(PointOnBoard i_CoinPosition, List<Move> i_PossibleMovesForCoin)
        {
            int[][] i_Directions;
            int i_Row = (int)i_CoinPosition.Row;
            int i_Col = (int)i_CoinPosition.Col;

            Coin i_Coin = m_MatrixBoard[i_Row, i_Col];
            if (i_Coin != null)
            {
                i_Directions = setDirectionsByCoinType(i_Coin);

                foreach (var i_Direction in i_Directions)
                {
                    int i_NewRow = i_Row + i_Direction[0];
                    int i_NewCol = i_Col + i_Direction[1];

                    if (checkRegularMove(i_PossibleMovesForCoin, i_CoinPosition, i_NewRow, i_NewCol) == false)
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

            // Define move directions based on king status and player
            if (i_Coin.IsKing())
            {
                directions = new int[][]
                {
                    new int[] { -1, -1 }, // Up-left
                    new int[] { -1, 1 },  // Up-right
                    new int[] { 1, -1 },  // Down-left
                    new int[] { 1, 1 }    // Down-right
                };
            }
            else if (i_Coin.m_Symbol == eSymbol.Player1) // Player 1 moves downward
            {
                directions = new int[][]
                {
                    new int[] { 1, -1 }, // Down-left
                    new int[] { 1, 1 }   // Down-right
                };
            }
            else if (i_Coin.m_Symbol == eSymbol.Player2) // Player 2 moves upward
            {
                directions = new int[][]
                {
                    new int[] { -1, -1 }, // Up-left
                    new int[] { -1, 1 }   // Up-right
                };
            }

            return directions;
        }
        private bool checkRegularMove(List<Move> i_PossibleMoves,PointOnBoard i_CoinPosition,int i_NewRow,int i_NewCol)
        {
            bool moveIsRegular = false;
            if (IsWithinBounds(i_NewRow, i_NewCol) && m_MatrixBoard[i_NewRow, i_NewCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition,new PointOnBoard((eRow)i_NewRow, (eCol)i_NewCol),false));
                moveIsRegular = true;
            }
            return moveIsRegular;
        }
        private void checkCaptureMove(List<Move> i_PossibleMoves, PointOnBoard i_CoinPosition, Coin i_Coin, int i_NewRow, int i_NewCol, int i_DirectionRow, int i_DirectionCol)
        {
            int i_CaptureRow = i_NewRow + i_DirectionRow;
            int i_CaptureCol = i_NewCol + i_DirectionCol;

            if (IsWithinBounds(i_CaptureRow, i_CaptureCol) &&
                m_MatrixBoard[i_NewRow, i_NewCol] != null &&
                m_MatrixBoard[i_NewRow, i_NewCol].m_Symbol != i_Coin.m_Symbol &&
                m_MatrixBoard[i_CaptureRow, i_CaptureCol] == null)
            {
                i_PossibleMoves.Add(new Move(i_CoinPosition,new PointOnBoard((eRow)i_CaptureRow, (eCol)i_CaptureCol),true));
            }
        }
        public List<Move> GetPossibleMovesForCoin(PointOnBoard i_CoinPosition)
        {
            List<Move> possibleMovesForCoin = new List<Move>();
            CalculatePossibleMovesForCoin(i_CoinPosition, possibleMovesForCoin);
            return possibleMovesForCoin;
        }
        //public List<Move> GetPossibleMovesForCoin(PointOnBoard i_CoinPosition)
        //{
        //    //Coin coin;

        //    //if (coin == null)
        //    //{
        //    //   return possibleMoves;
        //    //}

        //    //// Define move directions based on king status and player
        //    //if (coin.IsKing())
        //    //{
        //    //    directions = new int[][]
        //    //    {
        //    //        new int[] { -1, -1 }, // Up-left
        //    //        new int[] { -1, 1 },  // Up-right
        //    //        new int[] { 1, -1 },  // Down-left
        //    //        new int[] { 1, 1 }    // Down-right
        //    //    };
        //    //}
        //    //else if (coin.m_Symbol == eSymbol.Player1) // Player 1 moves downward
        //    //{
        //    //    directions = new int[][]
        //    //    {
        //    //        new int[] { 1, -1 }, // Down-left
        //    //        new int[] { 1, 1 }   // Down-right
        //    //    };
        //    //}
        //    //else if (coin.m_Symbol == eSymbol.Player2) // Player 2 moves upward
        //    //{
        //    //    directions = new int[][]
        //    //    {
        //    //        new int[] { -1, -1 }, // Up-left
        //    //        new int[] { -1, 1 }   // Up-right
        //    //    };
        //    //}
        //    //else
        //    //{
        //    //    return possibleMoves; // No valid moves if the coin has no valid symbol
        //    //}

        //    //int row = (int)i_CoinPosition.Row;
        //    //int col = (int)i_CoinPosition.Col;

        //    //foreach (var direction in directions)
        //    //{
        //    //    int newRow = row + direction[0];
        //    //    int newCol = col + direction[1];

        //    //    // Check for regular moves
        //    //    if (IsWithinBounds(newRow, newCol) && m_MatrixBoard[newRow, newCol] == null) 
        //    //    {
        //    //        possibleMoves.Add(new Move(
        //    //            i_CoinPosition,
        //    //            new PointOnBoard((eRow)newRow, (eCol)newCol),
        //    //            false));
        //    //    }

        //    //    // Check for capture moves
        //    //    int captureRow = newRow + direction[0];
        //    //    int captureCol = newCol + direction[1];
        //    //    if (IsWithinBounds(captureRow, captureCol)&&
        //    //                        m_MatrixBoard[newRow, newCol] != null &&
        //    //                        m_MatrixBoard[newRow, newCol].m_Symbol != coin.m_Symbol &&
        //    //                        //IsWithinBounds(captureRow, captureCol) &&
        //    //                        m_MatrixBoard[captureRow, captureCol] == null)
        //    //    {
        //    //        possibleMoves.Add(new Move(i_CoinPosition, new PointOnBoard((eRow)captureRow, (eCol)captureCol), true));
        //    //    }
        //    //}

        //    return possibleMoves;
        //}
        private bool IsWithinBounds(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < m_BoardSize && i_Col >= 0 && i_Col < m_BoardSize;
        }
    }

}



