using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Ex02
{
    //כאן מנוהל הממשק הטקסטואלי מול המשתמש
    public class ConsoleGame
    {
        GameManager gameManager = null;
        
        public void StartGame()
        {
            string namePlayer1 = string.Empty;
            string namePlayer2 = string.Empty;
            int sizeBoard = 0;
            bool isPlayAgainstHuman = false;
            eGameState gameState = eGameState.Next;
            Move playerMove = new Move();
            bool isFirstRound = true;

            namePlayer1 = nameInputFromUser();
            sizeBoard = getSizeBoardFromUser();
            if (playAgainstHuman() == true)
            {
                Console.WriteLine("For another player:");
                namePlayer2 = nameInputFromUser();
                isPlayAgainstHuman = true;
            }

            Console.WriteLine("{0}", namePlayer1);
            GameManager damkaGame = new GameManager(namePlayer1, namePlayer2, isPlayAgainstHuman, sizeBoard);

            while (gameState != eGameState.Exit) //!Q - The main loop of the whole game doesnt stop
            {
                damkaGame.CreateNewSession();
                isFirstRound = true;

                while (gameState == eGameState.Next)//single game run - as long there is't winners/equal/retirement
                {                   
                    printBoard(damkaGame.Session);
                    printTheLastMoveTaken(isFirstRound, damkaGame.Session.GetCurrentPlayer(), playerMove);
                    printPlayerTurn(damkaGame.Session.GetCurrentPlayer());
                    gameState = singlePlayTurn(damkaGame.Session, playerMove, sizeBoard);                  
                    isFirstRound = false;
                }

                if (isUserWantAnotherRound() == false)
                {
                    gameState = eGameState.Exit;
                }               
            }
            printGeneralGameSummery();
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
            //else if(i_InputFromUser == eGameState.Exit))
            else
            {
                isValid = false;
            }

            return isValid;
        }
        //private Move getAndCheckStepValidation(GameSession i_Session, int i_SizeBoard)
        //{
        //    Move playerMove = new Move();
        //    playerMove = stepTurnInput(i_SizeBoard);

        //    while (i_Session.IsValidMove(playerMove) == false)
        //    {
        //        playerMove = stepTurnInput(i_SizeBoard);
        //    }

        //    return playerMove;
        //}
        private Move stepTurnInput(int i_sizeBoard)
        {
            bool inputIsValid = false;
            string inputFromUser = string.Empty;
            Move playerMove = new Move();

            while (inputIsValid == false)
            {
                Console.WriteLine("Please enter move step: ROWcol>ROWcol");
                inputFromUser = Console.ReadLine();

                if (checkIfInputIsValid(inputFromUser, i_sizeBoard) == true)
                {
                    playerMove = setPlayerMove(inputFromUser);
                    break;
                }
                else
                {
                    Console.WriteLine("Wrong Input. Please choose again");
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

            while(isMoveValid == false)
            {
                i_PlayerMove = stepTurnInput(i_BoardSIze);
                i_Session.Turn(i_PlayerMove, out isMoveValid);
            }

            eGameState currentStateAfterMove = i_Session.GameStatus();

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
            
            return currentStateAfterMove;
        }
        private eGameState playerGetAnotherTurn(string i_PlayerName)
        {
            Console.WriteLine("{0} has another turn", i_PlayerName);
            return eGameState.Next;
        }
        private void thereIsAWinnerInThisSession(string i_UserName)
        {
            Console.WriteLine("{0} is the winner for this session!", i_UserName);
        }
        private void thereIsALoserInThisSession()
        {

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
        private void finishGeneralGame()
        {

        }
        private void printBoard(GameSession i_Session)
        {
            Coin[,] gameBoard = i_Session.GetGameBoardMatrix();
            int boardSize = gameBoard.GetLength(0);

            // הדפסת כותרת עם אותיות העמודות מה-Enum eCol
            Console.Write("    ");
            foreach (eCol col in Enum.GetValues(typeof(eCol)))
            {
                if ((int)col < boardSize) // מתחשב בגודל הלוח
                {
                    Console.Write("{0}     ", col);
                }
            }
            Console.WriteLine();

            // הדפסת שורת "=" מתחת לאותיות העמודות
            Console.WriteLine("  " + new string('=', boardSize * 6));

            foreach (eRow row in Enum.GetValues(typeof(eRow)))
            {
                if ((int)row < boardSize) // מתחשב בגודל הלוח
                {
                    // הדפסת אותיות השורות
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

                    // הדפסת קו מפריד בין השורות (למעט אחרי השורה האחרונה)
                    if ((int)row < boardSize - 1)
                    {
                        Console.WriteLine("  " + new string('=', boardSize * 6));
                    }
                }
            }

            // הוספת שורת "=" בסוף הלוח
            Console.WriteLine("  " + new string('=', boardSize * 6));
        }
        private void printPlayerTurn(Player i_Player)
        {
            Console.WriteLine("{0}'s Turn ({1}) :", i_Player.Name, (char)i_Player.Symbol);
        }
        private void printTheLastMoveTaken(bool i_IsFirstRound, Player i_Player, Move i_lastMove)
        {
            if (i_IsFirstRound == false)
            {
                Console.WriteLine("{0}'s move was ({1}) : {2}>{3}", i_Player.Name, (char)i_Player.Symbol, i_lastMove.Start, i_lastMove.End);
            }
        }
        private void printGeneralGameSummery()
        {

        }
    }
}







