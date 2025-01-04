using System;
using System.Linq;
using Ex02.ConsoleUtils;

namespace Ex02
{
    //כאן מנוהל הממשק הטקסטואלי מול המשתמש
    public class ConsoleGame
    {
        GameManager m_DamkaGame = null;
        
        public void StartGame()
        {
            string namePlayer1 = string.Empty;
            string namePlayer2 = string.Empty;
            int sizeBoard = 0;
            bool isPlayAgainstHuman = false;
            eGameState gameState = eGameState.Next;
            bool isFirstRound = true;

            Move playerMove = new Move();
            namePlayer1 = nameInputFromUser();
            sizeBoard = getSizeBoardFromUser();
            if (playAgainstHuman() == true)
            {
                Console.WriteLine("For another player:");
                namePlayer2 = nameInputFromUser();
                isPlayAgainstHuman = true;
            }
            else
            {
                isPlayAgainstHuman = false;
                namePlayer2 = "Computer";
            }

            //Console.WriteLine("{0}", namePlayer1);
            m_DamkaGame = new GameManager(namePlayer1, namePlayer2, isPlayAgainstHuman);

            while (gameState != eGameState.Exit) //!Q - The main loop of the whole game doesnt stop
            {
                m_DamkaGame.CreateNewSession(sizeBoard);
                isFirstRound = true;

                while (gameState == eGameState.Next)//single game run - as long there is't winners/equal/retirement
                {                   
                    printBoard(m_DamkaGame.Session);
                    printTheLastMoveTaken(isFirstRound, m_DamkaGame.Session);
                    printPlayerTurn(m_DamkaGame.Session.GetCurrentPlayer());
                    gameState = singlePlayTurn(m_DamkaGame.Session, playerMove, sizeBoard);                  
                    isFirstRound = false;
                    //Screen.Clear();
                }

                m_DamkaGame.UpdateWinnersScore();
                printGeneralGameSummery();

                if (isUserWantAnotherRound() == false)
                {
                    gameState = eGameState.Exit;
                } 
                else
                {
                    gameState = eGameState.Next;
                }
            }           
        }
        private string nameInputFromUser()
        {
            string userName = string.Empty;
            const int k_MaxLength = 20;
            bool inputIsValid = false;

            while (inputIsValid == false)
            {
                Console.WriteLine("Please type player name (maximum {0} characters):", k_MaxLength);
                userName = Console.ReadLine()?.Trim();

                if(string.IsNullOrEmpty(userName))
                {
                    Console.WriteLine("Name cannot be empty. Please try again.");
                }
                else if (userName.Length > k_MaxLength)
                {
                    Console.WriteLine("Name is too long. Please use up to {0} characters.", k_MaxLength);
                }
                else
                {
                    inputIsValid = true;
                }
            }

            return userName;
        }
        private int getSizeBoardFromUser()
        {            
            string userInput = string.Empty;
            int[] optionSizeBoard = { 6, 8, 10 };
            int sizeBoard = 0;
            bool inputIsValid = false;

            while (inputIsValid == false)
            {
                Console.Write("Please enter game size board: ");
                for (int i = 0; i < optionSizeBoard.Length; i++)
                {
                    Console.Write(optionSizeBoard[i]);
                    if (i < optionSizeBoard.Length - 1)
                    {
                        Console.Write(" / ");
                    }
                }
                Console.WriteLine(" ");
                userInput = Console.ReadLine();

                if(int.TryParse(userInput, out sizeBoard) && optionSizeBoard.Contains(sizeBoard))
                {
                    inputIsValid = true;
                }
                else
                {
                    Console.WriteLine("Wrong Input. Please choose again");
                }
            }

            return sizeBoard;
        }
        private bool playAgainstHuman()
        {
            string userInput = string.Empty;
            int userChoose = 0;
            bool isHuman = false;
            bool inputIsValid = false;

            while (inputIsValid == false)
            {
                Console.WriteLine("Please press (1) or (2):\n(1)   Player vs Player\n(2)   Player vs Computer");
                userInput = Console.ReadLine();

                if (int.TryParse(userInput, out userChoose) && (userChoose == 1 || userChoose == 2))
                {
                    if (userChoose == 1)
                    {
                        isHuman = true;
                    }
                    else if (userChoose == 2)
                    {
                        isHuman = false;
                    }
                    inputIsValid = true;
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong Input. Please choose again");
                }
            }

            return isHuman;
        }
        private bool checkIfInputIsValid(string i_InputFromUser, int i_SizeBoard)
        {
            bool isValid = false;
            const int k_LowerCaseA = (int)'a'; // 97
            int LowerCaseMaxSizeOfBoard = k_LowerCaseA + i_SizeBoard;
            const int k_UpperCaseA = (int)'A'; // 65
            int UpperCaseMaxSizeOfBoard = k_UpperCaseA + i_SizeBoard;
          
            if (i_InputFromUser.Length == 5)
            {
                for (int i = 0; i < i_InputFromUser.Length; i+=3)
                {
                    if (i_InputFromUser[i] >= k_UpperCaseA && i_InputFromUser[i] <= UpperCaseMaxSizeOfBoard &&
                        i_InputFromUser[i+1] >= k_LowerCaseA && i_InputFromUser[i+1] <= LowerCaseMaxSizeOfBoard)
                    {
                        isValid = true;
                    }                  
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            else if(i_InputFromUser == ((char)eGameState.Exit).ToString())
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }
        private Move stepTurnInput(int i_sizeBoard, GameSession i_Session, out bool i_isExitGame)
        {
            bool inputIsValid = false;
            string inputFromUser = string.Empty;
            Move playerMove = new Move();
            i_isExitGame = false;

            if (i_Session.GetCurrentPlayer().IsHuman == false)
            {
                playerMove = i_Session.CalculateComputerMove();
            }
            else
            {
                while (inputIsValid == false)
                {
                    Console.WriteLine("Please enter move step: ROWcol>ROWcol");
                    inputFromUser = Console.ReadLine();

                    if (checkIfInputIsValid(inputFromUser, i_sizeBoard) == true)
                    {
                        if (inputFromUser == ((char)eGameState.Exit).ToString())
                        {
                            i_isExitGame = true;
                            break;
                        }
                        else
                        {
                            playerMove = setPlayerMove(inputFromUser);
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong Input. Please choose again");
                    }
                }
            }

            return playerMove;
        }
        private Move setPlayerMove(string i_InputFromUser)
        {
            const int k_LowerCaseA = (int)'a'; // 97
            const int k_UpperCaseA = (int)'A'; // 65
            
            PointOnBoard start = new PointOnBoard();
            PointOnBoard end = new PointOnBoard();

            start.Row = (eRow)((int)i_InputFromUser[0] - k_UpperCaseA);
            start.Col = (eCol)((int)i_InputFromUser[1] - k_LowerCaseA);
            end.Row = (eRow)((int)i_InputFromUser[3] - k_UpperCaseA);
            end.Col = (eCol)((int)i_InputFromUser[4] - k_LowerCaseA);

            Move playerMove = new Move(start, end);

            return playerMove;
        }
        private eGameState singlePlayTurn(GameSession i_Session, Move i_PlayerMove, int i_BoardSIze)
        {
            bool isMoveValid = false;
            bool isExitGame = false;
            eGameState currentStateAfterMove;

            while (isMoveValid == false)
            {
                i_PlayerMove = stepTurnInput(i_BoardSIze, i_Session, out isExitGame);
                if (isExitGame == false)
                {
                    i_Session.Turn(i_PlayerMove, out isMoveValid);
                }
            }

            if (isExitGame == true)
            {
                 currentStateAfterMove = eGameState.Exit;
            }
            else
            {
                currentStateAfterMove = i_Session.GameStatus();

                switch (currentStateAfterMove)
                {
                    case eGameState.AnotherTurn:
                        currentStateAfterMove = playerGetAnotherTurn(i_Session.GetCurrentPlayer().Name);
                        break;
                    case eGameState.Win:
                        thereIsAWinnerInThisSession(i_Session.GetCurrentPlayer().Name);
                        break;
                    case eGameState.Lose:
                        thereIsAWinnerInThisSession(i_Session.GetCurrentPlayer().Name);
                        break;
                    case eGameState.Draw:
                        thisSessionEndWithDraw();
                        break;
                    default:
                        break;
                }
            }
            
            return currentStateAfterMove;
        }
        private eGameState playerGetAnotherTurn(string i_PlayerName)
        {
            Console.WriteLine("{0} has another turn", i_PlayerName);
            return eGameState.Next;
        }
        private void thereIsAWinnerInThisSession(string i_UserName) // call the function for the ניקוד
        {
            Console.WriteLine("{0} is the winner for this session!", i_UserName);

        }
        private void thisSessionEndWithDraw()
        {
            Console.WriteLine("The session ended in a draw");
        }
        private bool isUserWantAnotherRound()
        {
            string userInput = string.Empty;
            int userChoose = 0;
            bool returnValue = false;
            bool inputIsValid = false;

            while (inputIsValid == false)
            {
                Console.WriteLine("Would you like to play again?\nchoose:\n(1)  Yes\n(2)  No");
                userInput = Console.ReadLine();

                if (int.TryParse(userInput, out userChoose))
                {
                    if (userChoose == 1)
                    {
                        inputIsValid = true;
                        returnValue = true;
                    }
                    else if (userChoose == 2)
                    {
                        inputIsValid = true;
                        returnValue = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please choose again");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please choose again");
                }
            }

            return returnValue;
        }
        private void printBoard(GameSession i_Session)
        {           
            Coin[,] gameBoard = i_Session.GetGameBoardMatrix();
            int boardSize = gameBoard.GetLength(0);

            Console.Write("    ");
            foreach (eCol col in Enum.GetValues(typeof(eCol)))
            {
                if ((int)col < boardSize)
                {
                    Console.Write("{0}     ", col);
                }
            }
            Console.WriteLine();

            Console.WriteLine("  " + new string('=', boardSize * 6));

            foreach (eRow row in Enum.GetValues(typeof(eRow)))
            {
                if ((int)row < boardSize)
                {
                    Console.Write("{0}|  ", row);

                    for (int col = 0; col < boardSize; col++)
                    {
                        Coin coin = gameBoard[(int)row, col];

                        if (coin != null)
                        {
                            Console.Write("{0}  |  ", coin.GetSymbol());
                        }
                        else
                        {
                            Console.Write("   |  ");
                        }
                    }

                    Console.WriteLine();

                    if ((int)row < boardSize - 1)
                    {
                        Console.WriteLine("  " + new string('=', boardSize * 6));
                    }
                }
            }

            Console.WriteLine("  " + new string('=', boardSize * 6));
        }
        private void printPlayerTurn(Player i_Player)
        {
            string msg = string.Empty;

            if (i_Player.IsHuman == false)
            {
                msg = string.Format("Computer’s Turn (press ‘enter’ to see its move)");
                Console.WriteLine(msg);
                Console.ReadLine();
            }
            else
            {
                msg = string.Format("{0}'s Turn ({1}) :", i_Player.Name, (char)i_Player.Symbol);
                Console.WriteLine(msg);
            }
        }
        private void printTheLastMoveTaken(bool i_IsFirstRound, GameSession i_Session)
        {
            if (i_IsFirstRound == false)
            {
                Move lastMove = getLastMoveWasTaken(i_Session);
                Player lastPlayer = getLastPlayer(i_Session);

                string startPoint = string.Format("{0}{1}", lastMove.Start.Row, lastMove.Start.Col);
                string endPoint = string.Format("{0}{1}", lastMove.End.Row, lastMove.End.Col);
                string msg = string.Format("{0}'s move was ({1}) : {2}>{3}", lastPlayer.Name, (char)lastPlayer.Symbol, startPoint, endPoint);
                Console.WriteLine(msg);
            }           
        }
        private Move getLastMoveWasTaken(GameSession i_Session)
        {
            return i_Session.CurrentMove;
        }
        private Player getLastPlayer(GameSession i_Session)
        {
            if (i_Session.CurrentCoinForDoubleJump != null)
            {
                return i_Session.GetCurrentPlayer();
            }
            else
            {
                Player currentPlayer = i_Session.GetCurrentPlayer();
                Player player1 = i_Session.Player1;
                Player player2 = i_Session.Player2;

                return currentPlayer == player1 ? player2 : player1;
            }
        }
        private void printGeneralGameSummery()
        {
            string msg = string.Empty;

            msg = string.Format("Current points status:");
            msg = string.Format("Player 1:  {0}\nPlayer 2:  {1}", m_DamkaGame.GetPlayerScore(m_DamkaGame.Player1), m_DamkaGame.GetPlayerScore(m_DamkaGame.Player2));
            Console.WriteLine(msg);
        }
        private Move getComputerMove(GameSession i_Session)
        {
            return i_Session.CurrentMove;
        }
    }
}