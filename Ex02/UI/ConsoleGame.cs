using System;
using System.Linq;
using Ex02.ConsoleUtils;

namespace Ex02
{
    public class ConsoleGame
    {
        GameManager m_DamkaGame = null;

        public void StartGame()
        {
            initializePlayers(out string o_NamePlayer1, out string o_NamePlayer2, out int o_SizeBoard, out bool o_IsPlayAgainstHuman);
            runGameLoop(o_NamePlayer1, o_NamePlayer2, o_SizeBoard, o_IsPlayAgainstHuman);
        }
        private void initializePlayers(out string o_NamePlayer1, out string o_NamePlayer2, out int o_SizeBoard, out bool o_IsPlayAgainstHuman)
        {
            string msg = string.Empty;

            o_NamePlayer1 = nameInputFromUser();
            o_SizeBoard = getSizeBoardFromUser();
            o_IsPlayAgainstHuman = playAgainstHuman();

            if (o_IsPlayAgainstHuman)
            {
                msg = string.Format("For another player:");
                Console.WriteLine(msg);
                o_NamePlayer2 = nameInputFromUser();
            }
            else
            {
                o_NamePlayer2 = "Computer";
            }
        }
        private void runGameLoop(string i_NamePlayer1, string i_NamePlayer2, int i_SizeBoard, bool i_IsPlayAgainstHuman)
        {
            m_DamkaGame = new GameManager(i_NamePlayer1, i_NamePlayer2, i_IsPlayAgainstHuman);
            eGameState gameState = eGameState.Next;

            while (gameState != eGameState.Exit)
            {
                startNewSession(i_SizeBoard);
                gameState = playRounds(i_SizeBoard);
                m_DamkaGame.UpdateWinnersScore();
                printGeneralGameScore();
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
        private void startNewSession(int i_SizeBoard)
        {
            m_DamkaGame.CreateNewSession(i_SizeBoard);
        }
        private eGameState playRounds(int i_SizeBoard)
        {
            bool isFirstRound = true;
            eGameState gameState = eGameState.Next;
            Move playerMove = new Move();

            while (gameState == eGameState.Next)
            {
                Screen.Clear();
                printBoard(m_DamkaGame.Session);
                printTheLastMoveTaken(isFirstRound, m_DamkaGame.Session);
                printPlayerTurn(m_DamkaGame.Session.CurrentPlayer);
                gameState = singlePlayTurn(m_DamkaGame.Session, playerMove, i_SizeBoard);
                isFirstRound = false;
            }

            return gameState;
        }
        private string nameInputFromUser()
        {
            string userName = string.Empty;
            const int k_MaxLength = 20;
            bool inputIsValid = false;
            string msg = string.Empty;

            while (inputIsValid == false)
            {
                msg = string.Format("Please type player name (maximum {0} characters):", k_MaxLength);
                Console.WriteLine(msg);
                userName = Console.ReadLine();
                if(string.IsNullOrEmpty(userName) == true)
                {
                    msg = string.Format("Name cannot be empty. Please try again.");
                    Console.WriteLine();
                }
                else if (userName.Contains(' '))
                {
                    msg = string.Format("Name cannot contain spaces. Please try again.");
                    Console.WriteLine(msg);
                }
                else if (userName.Length > k_MaxLength)
                {
                    msg = string.Format("Name is too long. Please use up to {0} characters.", k_MaxLength);
                    Console.WriteLine(msg);
                }
                else if (userName.Any(char.IsDigit) == true)
                {
                    msg = string.Format("Name cannot contain numbers. Please try again.");
                    Console.WriteLine(msg);
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
            string msg = string.Empty;

            while (inputIsValid == false)
            {
                msg = "Please enter game size board: ";
                Console.Write(msg);
                for (int i = 0; i < optionSizeBoard.Length; i++)
                {
                    Console.Write(optionSizeBoard[i]);
                    if (i < optionSizeBoard.Length - 1)
                    {
                        Console.Write(" / ");
                    }
                }
                Console.WriteLine();
                userInput = Console.ReadLine();
                if(int.TryParse(userInput, out sizeBoard) && optionSizeBoard.Contains(sizeBoard))
                {
                    inputIsValid = true;
                }
                else
                {
                    msg = string.Format("Wrong Input. Please choose again");
                    Console.WriteLine(msg);
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
            string msg = string.Empty;

            while (inputIsValid == false)
            {
                msg = string.Format("Please press (1) or (2):\n(1)   Player vs Player\n(2)   Player vs Computer");
                Console.WriteLine(msg);
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
                    msg = string.Format("Wrong Input. Please choose again");
                    Console.WriteLine(msg);
                }
            }

            return isHuman;
        }
        private bool checkIfInputIsValid(string i_InputFromUser, int i_SizeBoard)
        {
            bool isValid = false;
            const int k_LowerCaseA = (int)'a';
            int LowerCaseMaxSizeOfBoard = k_LowerCaseA + i_SizeBoard;
            const int k_UpperCaseA = (int)'A';
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
        private Move stepTurnInput(int i_sizeBoard, GameSession i_Session, out bool o_isExitGame)
        {
            bool inputIsValid = false;
            string inputFromUser = string.Empty;
            Move playerMove = new Move();
            o_isExitGame = false;
            string msg = string.Empty;

            if (i_Session.CurrentPlayer.IsHuman == false)
            {
                playerMove = i_Session.CalculateComputerMove();
            }
            else
            {
                while (inputIsValid == false)
                {
                    msg = string.Format("Please enter move step: ROWcol>ROWcol");
                    Console.WriteLine(msg);
                    inputFromUser = Console.ReadLine();

                    if (checkIfInputIsValid(inputFromUser, i_sizeBoard) == true)
                    {
                        if (inputFromUser == ((char)eGameState.Exit).ToString())
                        {
                            o_isExitGame = true;
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
                        msg = string.Format("Wrong Input. Please choose again");
                        Console.WriteLine(msg);
                    }
                }
            }

            return playerMove;
        }
        private Move setPlayerMove(string i_InputFromUser)
        {
            const int k_LowerCaseA = (int)'a';
            const int k_UpperCaseA = (int)'A';            
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
            string msg = string.Empty;
            eGameState currentStateAfterMove;

            while (isMoveValid == false)
            {
                i_PlayerMove = stepTurnInput(i_BoardSIze, i_Session, out isExitGame);
                if (isExitGame == false)
                {
                    i_Session.Turn(i_PlayerMove, out isMoveValid);
                }
                else
                {
                    isMoveValid = true;
                }

                if (isMoveValid == false)
                {
                    msg = string.Format("This move invalid. Please try again");
                    Console.WriteLine(msg);
                }
            }

            if (isExitGame == true)
            {
                currentStateAfterMove = eGameState.Exit;
                thereIsAWinnerInThisSession(getWinnerPlayerName(i_Session));
            }
            else
            {
                currentStateAfterMove = i_Session.GameStatus();

                switch (currentStateAfterMove)
                {
                    case eGameState.AnotherTurn:
                        currentStateAfterMove = playerGetAnotherTurn(i_Session.CurrentPlayer.Name);
                        break;
                    case eGameState.Win:
                        thereIsAWinnerInThisSession(i_Session.CurrentPlayer.Name);
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
        private string getWinnerPlayerName(GameSession i_Session)
        {
            return i_Session.CurrentPlayer.Symbol == eSymbol.Player1 ? i_Session.Player2.Name : i_Session.Player1.Name;
        }
        private eGameState playerGetAnotherTurn(string i_PlayerName)
        {
            string msg = string.Empty;

            msg = string.Format("{0} has another turn", i_PlayerName);
            Console.WriteLine(msg);

            return eGameState.Next;
        }
        private void thereIsAWinnerInThisSession(string i_UserName)
        { 
            string msg = string.Empty;

            msg = string.Format("{0} is the winner for this session!", i_UserName);
            Console.WriteLine(msg);
        }
        private void thisSessionEndWithDraw()
        {
            string msg = string.Empty;

            msg = string.Format("The session ended in a draw");
            Console.WriteLine(msg);
        }
        private bool isUserWantAnotherRound()
        {
            string userInput = string.Empty;
            int userChoose = 0;
            bool returnValue = false;
            bool inputIsValid = false;
            string msg = string.Empty;

            while (inputIsValid == false)
            {
                msg = string.Format("Would you like to play again?\nchoose:\n(1)  Yes\n(2)  No");
                Console.WriteLine(msg);
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
                        msg = string.Format("Invalid input. Please choose again");
                        Console.WriteLine(msg);
                    }
                }
                else
                {
                    msg = string.Format("Invalid input. Please choose again");
                    Console.WriteLine(msg);
                }
            }

            return returnValue;
        }
        private void printBoard(GameSession i_Session)
        {
            Coin[,] gameBoard = i_Session.GameBoardMatrix;
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
                            Console.Write("{0}  |  ", coin.Symbol);
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
                return i_Session.CurrentPlayer;
            }
            else
            {
                Player currentPlayer = i_Session.CurrentPlayer;
                Player player1 = i_Session.Player1;
                Player player2 = i_Session.Player2;

                return currentPlayer == player1 ? player2 : player1;
            }
        }
        private void printGeneralGameScore()
        {
            string msg = string.Empty;

            msg = string.Format("Current points status:");
            msg = string.Format("{0}:  {1}\n{2}:  {3}",m_DamkaGame.Player1.Name, m_DamkaGame.GetPlayerScore(m_DamkaGame.Player1), m_DamkaGame.Player2.Name, m_DamkaGame.GetPlayerScore(m_DamkaGame.Player2));
            Console.WriteLine(msg);
        }
        private Move getComputerMove(GameSession i_Session)
        {
            return i_Session.CurrentMove;
        }
    }
}