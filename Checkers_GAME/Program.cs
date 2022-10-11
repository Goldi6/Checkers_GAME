using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers_GAME
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Board board = new Board();
            board.play();
        }
    }//TODO setup program to different board games
    
    //class Checkers : Board
    //{
    //}

    class Board
    {
        string letters; //on board
        bool currentPlayer; // black/white
        public int blackOnBoard, whiteOnBoard; //figure count
        public Figure[][] board;

        public int idsCount; //for position setter
        ///test board constructor
        bool builder_isQueen;
        bool builderDeleteMode;
        int moveCounterTillEndgame; //for game break


        public void createPieceOnBoard(bool color, int row, int col, string pieceType)
        {
            if (pieceType=="queen")
                board[row][col] = new CheckersQueen(color);
            else
                board[row][col] = new Checker(color);
            idsCount++;
            if (color) whiteOnBoard++;
            else blackOnBoard++;
            (board[row][col]).setPosition_RowCol(this);
            //  Console.WriteLine(board[row][col].ToString() + "row:" + (board[row][col]).row + " col: " + (board[row][col]).col);
        } //TODO *** INTERFACE Igame
        void createGameBoard()
        {

            //CREATE BOARD
            for (int y = 0; y < 8; y++)
            {
                int x;
                if (y == 0 || y == 2)
                {

                    for (x = 1; x < 8; x += 2)
                    {
                        createPieceOnBoard(true, y, x, "");
                    }
                }
                else if (y == 7 || y == 5)
                {

                    for (x = 0; x < 8; x += 2)
                    {
                        createPieceOnBoard(false, y, x, "");
                    }
                }
                if (y == 1)
                {

                    for (x = 0; x < 8; x += 2)
                    {
                        createPieceOnBoard(true, y, x, "");
                    }
                }
                else if (y == 6)
                {

                    for (x = 1; x < 8; x += 2)
                    {
                        createPieceOnBoard(false, y, x, "");
                    }
                }
            }
        } //TODO default board *** INTERFACE IGAME

        void checkers_designerInput()
        {
            string builderMsg;
            string deleteMsg = "DELETE mode, enter position to delete: \n(type 'white' or 'black' to resume figure input or 'done' to start game play)";

            bool doneWithInput = false;
            string msg;
            while (!doneWithInput)
            {
                builderMsg = $"{(currentPlayer ? "White" : "Black")} {(builder_isQueen ? "Queen" : "Checker")} position: \n(type {(builder_isQueen ? "'checker'" : "'queen'")} to switch figure ,type {(currentPlayer ? "'black'" : "'white'")} to continue entering {(currentPlayer ? "Black" : "White")} positions 'Done' to start game 'Delete' to delete mode)\n";
                msg = builderDeleteMode ? deleteMsg : builderMsg;
                int[] pos = legalInput_Handler(msg, true);
                if (pos == null)
                {
                    printPrettyBoard(emptyBoard());
                }

                else if (pos.Length == 2 && CheckersRule_legalPosition(pos) && !builderDeleteMode)
                {
                    if (((currentPlayer && pos[0] == 7) || (!currentPlayer && pos[0] == 0)) && !builder_isQueen)
                    {
                        Console.WriteLine(" *** ILLEGAL:  you should select a queen figure to place in this position\n");
                    }
                    else
                    {
                        string piece = builder_isQueen ? "queen" : "";
                        createPieceOnBoard(currentPlayer, pos[0], pos[1], piece);
                        printPrettyBoard(emptyBoard());

                    }

                }
                else if (pos.Length == 2 && CheckersRule_legalPosition(pos) && builderDeleteMode)
                {

                    board[pos[0]][pos[1]] = null;
                    printPrettyBoard(emptyBoard());
                }
                else if (pos.Length == 1)
                {
                    while (!doneWithInput)
                    {
                        Console.WriteLine("Who starts? (type 'black'/'white')");
                        string starts;
                        starts = Console.ReadLine();
                        starts = starts.Trim().ToLower();
                        switch (starts)
                        {
                            case "black": currentPlayer = false; doneWithInput = true; break;
                            case "white": currentPlayer = true; doneWithInput = true; break;
                            default: Console.WriteLine("ILLEGAL INPUT.\n"); break;
                        }
                    }

                }
                else if (pos.Length == 2 && !CheckersRule_legalPosition(pos))
                {
                    Console.WriteLine("*** you can only place figures on the marked areas!\n");
                }

            }
        }
        void DesignBoard()
        {

            printPrettyBoard(emptyBoard());
            currentPlayer = false;
            Console.WriteLine("\n* please enter positions (row/column) one by one (example: 1A , 3G):");
            Console.WriteLine(" * REMEMBER! Black goes UP, White goes DOWN.\n");
            checkers_designerInput();
        }

        bool CheckersRule_legalPosition(int[] pos)
        {
            return (pos[0] % 2 == 0 && pos[1] % 2 != 0) || (pos[0] % 2 != 0 && pos[1] % 2 == 0);
        }
        void gameLoop()
        {


            bool moveMade = false;
            int[] figPos ;
            int[] movPos ;
            bool mustCapture;
            if (!endGame())
            {

                while (!moveMade)
                {
                    //avail


                    //get player position input
                    string msg = $"{(currentPlayer ? "White" : "Black")} players turn! enter figure position to move (example - 2D):\nYou may end this game by typing 'resign' or ask your oponent for a tie by typing 'tie'. ";
                    figPos = legalInput_Handler(msg, false);
                    //if returns null means game ended (tie/resign)!
                    if (figPos == null)
                    {
                        movPos = null; break;
                    }
                    else
                    {
                        Figure figRef = board[figPos[0]][figPos[1]];

                        mustCapture = canEat_any();
                        //Console.WriteLine("MUST EAT: " + mustEat);

                        bool correctPlayer = pieceBelongsToPlayer(figPos, currentPlayer);
                        if (!correctPlayer)
                        {
                            illegalInput_print($"you did not select your {(currentPlayer ? "White" : "Black")} checker... ");
                        }
                        else if (mustCapture && !figRef.canCapture(this))
                        {
                            illegalInput_print($"you must eat opponents {(!currentPlayer ? "White" : "Black")} checker! select another checker... ");

                        }
                        else if (!mustCapture && !figRef.canMove(this))
                        {
                            illegalInput_print($"this figure cant move anywhere,it's blocked by other figures. select another checker...");
                        }
                        else
                        {
                            // Console.WriteLine("CAN MOVE:" + figRef.checkMove());
                            // Console.WriteLine("CAN EAT:" + figRef.checkEat());
                            while (!moveMade)
                            {
                                bool transformed;

                                //!GENERATE AVAILABLE MOVES!!!!
                                int[][] availableMovesFull;
                                if (mustCapture)
                                    availableMovesFull = figRef.availablePositions_toCapture(this);
                                else
                                    availableMovesFull = figRef.availablePositions_toMove(this);
                                //? not necessary
                                //!generates new array without nulls from Array.availableMovesFull to Array.availableMoves  //pretty board uses this new array
                                int count = 0;
                                for (int i = 0; i < availableMovesFull.Length; i++)
                                {
                                    if (availableMovesFull[i] != null) count++;

                                }
                                int[][] availableMoves = new int[count][];
                                int index = 0;
                                for (int i = 0; i < availableMovesFull.Length; i++)
                                {
                                    if (availableMovesFull[i] != null) availableMoves[index++] = availableMovesFull[i];
                                }


                                /////////////////////////////////
                                //#region uncomment for auto move if there is only one option
                                //if (availableMoves.Length == 1)
                                //{
                                //    movPos = availableMoves[0];
                                //}
                                //else
                                //{

                                printPrettyBoard(prettyBoard(availableMoves));
                                msg = figRef.ToString() + " to where? :";
                                movPos = legalInput_Handler(msg, false);
                                // }
                                // #endregion
                                //!returns null for resign or tie
                                if (movPos == null)
                                {
                                    moveMade = true;
                                    break;
                                }
                                else
                                {

                                    bool verifyMove = true;
                                    for (int i = 0; i < availableMoves.Length; i++)//!checks if user typed available move to the figure
                                    {
                                        if (availableMoves[i] != null && availableMoves[i][0] == movPos[0] && availableMoves[i][1] == movPos[1])
                                        {
                                            //uncomment to verifyMove with yes/no (for figure re-selection option)
                                            // verifyMove = verify(((Figure)board[figPos[0]][figPos[1]]).ToString() + " to " + getPositionName(movPos));
                                            // if (verifyMove)
                                            //{
                                            if (!mustCapture)
                                            {
                                                figRef.move(movPos,this);
                                                moveCounterTillEndgame--; //used to count moves with no action or changes for figures amount on the board
                                                currentPlayer = !currentPlayer;
                                                printPrettyBoard(prettyBoard());
                                                moveMade = true;
                                                gameLoop();
                                            }
                                            else
                                            {

                                                Console.WriteLine("You may capture your opponents Checker!");

                                                int[] delFigPos = figRef.getCell_figToDelete(movPos);
                                                transformed = figRef.capture(movPos, new int[] { delFigPos[0], delFigPos[1] },this);

                                                printPrettyBoard(prettyBoard());

                                                Figure afterMov_figRef = board[movPos[0]][movPos[1]];

                                                bool captureMore = afterMov_figRef.canCapture(this);
                                                moveCounterTillEndgame = 15; //unset counter if capture was made to initial state

                                                if (captureMore && !transformed)
                                                {
                                                    if (verify("Continue jumping over opponents checkers?"))
                                                        figRef = afterMov_figRef;
                                                    else
                                                    {
                                                        moveMade = true;

                                                        afterMov_figRef.deleteFigureFromCell(new int[] { movPos[0], movPos[1] }, true,this);

                                                        printPrettyBoard(prettyBoard());
                                                        Console.WriteLine("BURN!");

                                                        currentPlayer = !currentPlayer;
                                                        gameLoop();
                                                    }
                                                }
                                                else
                                                {
                                                    moveMade = true;
                                                    currentPlayer = !currentPlayer;
                                                    gameLoop();
                                                }
                                            }

                                            // } //uncomment to verifyMove with yes/no
                                        }
                                    }
                                    if (!moveMade && verifyMove) illegalInput_print("all available moves are marked...try again");
                                }

                            }
                        }
                    }
                }
            }


        }
        public void play()
        {
            bool start = false;
            do
            {
                Console.WriteLine("Create a Test board or a GamePlay board? (Test/Game)");

                string ans = Console.ReadLine();
                ans = ans.Trim().ToLower();
                switch (ans)
                {
                    default: Console.WriteLine("wrong input"); break;
                    case "game": start = true; createGameBoard(); break;
                    case "test": start = true; builder_isQueen = false;
                        builderDeleteMode = false; DesignBoard(); break;
                }
            } while (!start);
            
            printRules();
            printPrettyBoard(prettyBoard());
            gameLoop();
        }
        void printRules()
        {

            Console.WriteLine("GAME STARTS!!!");
            Console.WriteLine();
            Console.WriteLine("RULES:");
            Console.WriteLine("* Men can only go or eat forwards.");
            Console.WriteLine("* Black goes DOWN, White goes UP.");
            Console.WriteLine("* Your soldier will burn if you wont eat the opponents checker when available.");
            Console.WriteLine("* When Men reaches the board end it turns into a King,  the turn ends.");
            Console.WriteLine("* It's a Game Over for the one who loses all Figures or if there is no available move to make.");
            Console.WriteLine("* If you decide to Resign, You Lose.");
            Console.WriteLine("* You may suggest your opponent a Tie for no winners or losers.");
            Console.WriteLine();
            Console.WriteLine();
        }//TODO interface IgameRules
        public Board()//TODO add size option
        {
            currentPlayer = false;
            letters = "     A    B    C    D    E    F    G    H";
            moveCounterTillEndgame = 15;
            idsCount = 0;
          
            initiateBoard();


        }
        public string getPositionName(int[] figPos)
        {
            string colLetter = "";
            switch (figPos[1])
            {
                case 0: colLetter = "A"; break;
                case 1: colLetter = "B"; break;
                case 2: colLetter = "C"; break;
                case 3: colLetter = "D"; break;
                case 4: colLetter = "E"; break;
                case 5: colLetter = "F"; break;
                case 6: colLetter = "G"; break;
                case 7: colLetter = "H"; break;
            }
            return figPos[0] + 1 + "" + colLetter;
        }
        //-CONSTRUCT BOARD display
        void initiateBoard()
        {
            board = new Figure[8][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new Figure[8];
            }
        }
        string[][] emptyBoard()
        {
            string[][] emptyB = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                emptyB[i] = new string[8];
                if (i % 2 == 0)
                {

                    for (int j = 0; j < 8; j++)
                    {
                        if (j % 2 == 0)
                        {
                            emptyB[i][j] = "|    ";

                        }
                        else
                        {
                            string inp = board[i][j] == null ? "XX" : board[i][j].Unicode + " ";
                            emptyB[i][j] = "| " + inp + " ";

                        }
                    }

                }
                else
                {

                    for (int j = 0; j < 8; j++)
                    {
                        if (j % 2 != 0)
                        {
                            emptyB[i][j] = "|    ";

                        }
                        else
                        {
                            string inp = board[i][j] == null ? "XX" : board[i][j].Unicode;
                            emptyB[i][j] = "| " + inp + " ";

                        }
                    }
                }
            }
            return emptyB;
        }
        string[][] prettyBoard()
        {
            string[][] prettyB = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                prettyB[i] = new string[8];
                for (int j = 0; j < 8; j++)
                {

                    if (board[i][j] != null) prettyB[i][j] = "| " + board[i][j].Unicode + "  ";
                    else prettyB[i][j] = "|    ";
                }
            }
            return prettyB;
        }
        string[][] prettyBoard(int[][] availablePos)
        {
            string[][] prettyB = prettyBoard();
            for (int a = 0; a < availablePos.Length; a++)
            {
                if (availablePos[a] != null)
                {
                    int row = availablePos[a][0]; int col = availablePos[a][1];
                    char[] ch = prettyB[row][col].ToCharArray();
                    ch[1] = ch[4] = 'X';
                    prettyB[row][col] = new string(ch);
                }
            }
            return prettyB;
        }
        void printPrettyBoard(string[][] boardP)
        {
            Console.WriteLine(letters);
            for (int i = 0; i < 8; i++)
            {
                Console.Write(i + 1 + "  ");
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(boardP[i][j]);
                }
                Console.Write("|");
                Console.WriteLine();
                Console.WriteLine("    ---- ---- ---- ---- ---- ---- ---- ----");
            }
            Console.WriteLine(ToString() + "\n");

        }

        //PRINTS GAME summary
        public override string ToString()
        {
            return $"Black on board: {blackOnBoard} , White on board: {whiteOnBoard}";
        }
        //VERIFY YES/NO : question
        bool verify(string msg)
        {
            Console.WriteLine(" type Y/N : " + msg);
            string ans;
            ans = Console.ReadLine();
            switch (ans.Trim().ToLower())
            {
                case "y": return true;
                case "n": return false;
                default:
                    illegalInput_print(); return verify(msg);
            }
        }

        bool canEat_any()
        {
            int checkersCount = currentPlayer ? whiteOnBoard : blackOnBoard;
            int counter = 0;


            for (int i = 0; i < 8 && counter < checkersCount; i++)
            {
                for (int j = 0; j < 8 && counter < checkersCount; j++)
                {
                    if (board[i][j] != null && board[i][j].color == currentPlayer)
                    {
                        if (board[i][j].canCapture(this))
                        {
                            return true;
                        }
                        counter++;
                    }
                }

            }
            return false;
        }
        bool canMove_any()
        {
            int checkersCount = currentPlayer ? whiteOnBoard : blackOnBoard;

            int counter = 0;


            for (int i = 0; i < 8 && counter < checkersCount; i++)
            {
                for (int j = 0; j < 8 && counter < checkersCount; j++)
                {
                    if (board[i][j] != null && board[i][j].color == currentPlayer)
                    {
                        if (board[i][j].canMove(this))
                        {
                            return true;
                        }
                        counter++;
                    }
                }

            }
            return false;
        }
        //GAME PLAY


        //CHECKS IF FIGURE ON THE BOARD BELONGS TO SPECIFIED PLAYER 
        public bool pieceBelongsToPlayer(int[] pos, bool color)
        {
            return board[pos[0]][pos[1]] != null && board[pos[0]][pos[1]].color == color;

        }


        int[] checkBuilderInputOptions(string input)
        {
            //game.builderInputOptions(input);

            if ((input == "queen" || input == "checker"))
            {
                if (input == "queen")
                    builder_isQueen = true;
                else
                    builder_isQueen = false;
                builderDeleteMode = false;
                return null;
            }

            else return new int[1];
        }  //TODO ***INTERFASE Igame
        //CHECKS If INPUT IS IN THE BOARD RANGE or tie/resign
        int[] legalInput_Handler(string msg, bool builder )
        {
            Console.WriteLine(msg);
            string pos = Console.ReadLine();


            pos = pos.Trim().ToLower();
            int column;
            string correction = "try looking at  the board and use only the row|column characters...or use instruction options";
            int[] corrector()
            {
                illegalInput_print(correction); return legalInput_Handler(msg, builder);
            }

            if (builder)
            {
                int[] builderResult = checkBuilderInputOptions(pos);
                if (builderResult == null)
                    return null;
            }

             if ((pos == "white" || pos == "black") && builder)
            {
                currentPlayer = pos == "white" ? true : false;
                builderDeleteMode = false;
                return null;
            }
            else if (pos == "done" && builder)
            {
                builderDeleteMode = false;
                return new int[1] { 1 };
            }
            else if (pos == "delete" && builder)
            {
                builderDeleteMode = true;
                return null;
            }

            else if (pos == "resign" && !builder)
            {
                if (!resign())
                    return new int[2] { board.Length, board[0].Length };
                else return null;
            }
            else if (pos == "tie" && !builder)
            {
                if (!suggestTie())
                    return new int[2] { board.Length, board[0].Length };
                else return null;
            }

            else if (pos.Length == 2)
            {
                //TODO: adjust for bigger board
                switch (pos[1])
                {
                    default: return corrector();
                    case 'a': column = 0; break;
                    case 'b': column = 1; break;
                    case 'c': column = 2; break;
                    case 'd': column = 3; break;
                    case 'e': column = 4; break;
                    case 'f': column = 5; break;
                    case 'g': column = 6; break;
                    case 'h': column = 7; break;
                }
                if (Char.IsDigit(pos[0]) && pos[0] != '0' && pos[0] != '9')
                    return new int[2] { int.Parse(pos[0] + "") - 1, column };
                else return corrector();

            }
            else return corrector();
        }

        void illegalInput_print(string correction = "")
        {
            Console.WriteLine();
            Console.WriteLine("Illegal input! ");
            if (correction != "")
                Console.WriteLine(correction);
            Console.WriteLine();


        }


        public bool endGame()
        {

            if (moveCounterTillEndgame == 0)
            {
                Console.WriteLine("15 moves and nothing happened! Game over, it's a tie!");
                return true;
            }
            else if (whiteOnBoard == 0 || blackOnBoard == 0)
            {
                string winner = whiteOnBoard == 0 ? "Black" : "White";
                Console.WriteLine($"Game over! {winner} wins!");
                return true;
            }
            else if (!canEat_any() && !canMove_any())
            {
                Console.WriteLine($"Game over! no More Moves! {(!currentPlayer ? "white" : "black")} wins!");

                return true;
            }
            return false;
        }//TODO check for game end options  ***INTERFACE Igame

        bool resign()
        {
            string player = currentPlayer ? "White" : "Black";
            string oponent = !currentPlayer ? "White" : "Black";
            if (verify("are you sure you want to resign?"))
            {
                Console.WriteLine($"Game ended. {(player)} resigned! {oponent} wins!");
                return true;
            }
            else return false;
        }
        //? set player change??(on-line)
        bool suggestTie()
        {
            if (verify("your opponent suggests a tie, what do you think?"))
            {
                Console.WriteLine("It's a tie!"); return true;
            }
            else return false;
        }

    }



    class Figure
    {
        public virtual bool canCapture(Board board) { return true; }
        public virtual bool canMove(Board board) { return true; }

        public virtual int[][] availablePositions_toCapture(Board board) { return null; }
        public virtual int[][] availablePositions_toMove(Board board) { return null; }
        public virtual bool capture(int[] capPos, int[] delPos, Board board) { return true; }
        public virtual void move(int[] movPos, Board board) {  }
        public bool color;
        public string Unicode;
        protected string figName;
        public int row;
        public int col;
        public string prettyPosition;
        public string figId;

        public override bool Equals(object obj)
        {
            if (obj is Figure)
            {
                Figure fig = obj as Figure;
                string id = fig.figId;
                return id == this.figId;
            }
            else { return false; }
        }
        public override string ToString()
        {
            return $"{(color ? "White" : "Black")} {figName} on {prettyPosition}";
        }


        public Figure(bool player, string figureCode, string figureName)
        {
            color = player;
            figName = figureName;
            Unicode = figureCode;
            
        }

        public void setPosition_RowCol(Board boardObj)
        {
            figId = (color ? "W" : "B") + boardObj.idsCount;
            bool found = false;
            for (int i = 0; i < boardObj.board.Length && !found; i++)
            {
                for (int j = 0; j < boardObj.board[i].Length && !found; j++)
                {
                    if (Equals(boardObj.board[i][j]))
                    {

                        row = i;
                        col = j;
                        found = true;
                    }
                }
            }
            prettyPosition = boardObj.getPositionName(new int[] { row, col });
        }

        protected int[][] getLegalCellsAround(int steps, bool forwardOnly, int row, int col)
        {
            //int row = position[0];
            //int col = position[1];
            bool upLeftB = row - steps >= 0 && col - steps >= 0;
            bool upRightB = row - steps >= 0 && col + steps <= 7;
            bool downLeftB = row + steps <= 7 && col - steps >= 0;
            bool downRightB = row + steps <= 7 && col + steps <= 7;


            int[] upLeft = upLeftB ? new int[2] { row - steps, col - steps } : null;
            int[] upRight = upRightB ? new int[2] { row - steps, col + steps } : null;
            int[] downLeft = downLeftB ? new int[2] { row + steps, col - steps } : null;
            int[] downRight = downRightB ? new int[2] { row + steps, col + steps } : null;
            int[][] arr;
            if (!upLeftB && !upRightB && !downLeftB && !downRightB)
            {
                arr = null;
            }
            if (!forwardOnly)
                arr = new int[4][] { upLeft, upRight, downLeft, downRight };
            else
            {
                if (color && (downLeftB || downRightB)) arr = new int[2][] { downRight, downLeft };
                else if (!color && (upRightB || upLeftB)) arr = new int[2][] { upLeft, upRight };
                else arr = null;
            }
            return arr;


        }


        public int[] getCell_figToDelete(int[] movPos)
        {
            int delFig_r = row < movPos[0] ? movPos[0] - 1 : movPos[0] + 1;
            int delFig_c = col < movPos[1] ? movPos[1] - 1 : movPos[1] + 1;
            return new int[] { delFig_r, delFig_c };
        }
        public void deleteFigureFromCell(int[] delPos, bool adjustCount, Board boardObj)
        {
            if (adjustCount)
            {
                bool color = (boardObj.board[delPos[0]][delPos[1]]).color;
                if (color) boardObj.whiteOnBoard--;
                else boardObj.blackOnBoard--;

            }
            boardObj.board[delPos[0]][delPos[1]] = null;


        }
        protected void moveFigureToNewCell(int[] movPos , Board boardObj)
        {
            Figure[][] board= boardObj.board;
            Figure fig = board[row][col];
            board[movPos[0]][movPos[1]] = fig;
            deleteFigureFromCell(new int[] { row, col }, false,boardObj);
            board[movPos[0]][movPos[1]].setPosition_RowCol(boardObj);
        }


    }

    class CheckersQueen  : Figure
    {


        public CheckersQueen(bool player) : base(player, (player ? "\u265B" : "\u2655"), "Queen") { }

        public override int[][] availablePositions_toCapture(Board boardObj)
        {
            Figure[][] board= boardObj.board;
            int[][] result;
            int[][] availableCellsAroundFig = getLegalCellsAround(1, false, row, col);
            string availableIndexes = "";
            
            //adds all available Jumps from the position to a string that will contain all directions positions
            void addIndexes_loop(int[] rc, int upDon, int lR)
            {
                int r = rc[0]; int c = rc[1];
                if (board[r][c] != null && board[r][c].color != color)
                {
                    bool inRange = (r + upDon >= 0 && r + upDon < board.Length) && (c + lR >= 0 && c + lR < board[0].Length);
                    if (inRange && board[r + upDon][c + lR] == null)
                    {
                        availableIndexes += (r + upDon) + "" + (c + lR) + "|";


                    }
                }
                else if (board[r][c] == null)
                {
                    bool inRange = (r + upDon >= 0 && r + upDon < board.Length) && (c + lR >= 0 && c + lR < board[0].Length);
                    if (inRange)
                        addIndexes_loop(new int[] { r + upDon, c + lR }, upDon, lR);
                }


            }//upLeft, upRight , downLeft, downRight
            if (availableCellsAroundFig[0] != null)
                addIndexes_loop(availableCellsAroundFig[0], -1, -1);
            if (availableCellsAroundFig[1] != null)
                addIndexes_loop(availableCellsAroundFig[1], -1, 1);
            if (availableCellsAroundFig[2] != null)
                addIndexes_loop(availableCellsAroundFig[2], 1, -1);
            if (availableCellsAroundFig[3] != null)
                addIndexes_loop(availableCellsAroundFig[3], 1, 1);
            // Console.WriteLine($"availableMove : {available}");
            if (availableIndexes != "")
            {


                availableIndexes = availableIndexes.Remove(availableIndexes.Length - 1);
                string[] arr = availableIndexes.Split('|');
                // Console.WriteLine($"availableMove : {arr.Length}");
                result = new int[arr.Length][];
                for (int i = 0; i < arr.Length; i++)
                {
                    result[i] = new int[] { int.Parse(arr[i][0] + ""), int.Parse(arr[i][1] + "") };
                }
            }
            else
            {
                result = new int[1][]; result[0] = null;
            }
            return result;
        }

        public override int[][] availablePositions_toMove(Board boardObj)
        {
            int[][] result;
            int[][] availableCellsAroundFig = getLegalCellsAround(1, false, row, col);
            string availables = "";
            Figure[][] board = boardObj.board;
            void loop(int[] rc, int upDon, int lR)
            {
                int r = rc[0]; int c = rc[1];
                if ((r >= 0 && r < board.Length && c >= 0 && c < board[0].Length) && board[r][c] == null)
                {
                    availables += r + "" + c + "|";
                    if (r + upDon >= 0 && r + upDon <= 7 && c + lR >= 0 && c + lR <= 7)
                    {

                        loop(new int[] { r + upDon, c + lR }, upDon, lR);
                    }
                }


            }//uplR \ DLR
            if (availableCellsAroundFig[0] != null)
                loop(availableCellsAroundFig[0], -1, -1);
            if (availableCellsAroundFig[1] != null)
                loop(availableCellsAroundFig[1], -1, 1);
            if (availableCellsAroundFig[2] != null)
                loop(availableCellsAroundFig[2], 1, -1);
            if (availableCellsAroundFig[3] != null)
                loop(availableCellsAroundFig[3], 1, 1);
            if (availables != "")
            {


                availables = availables.Remove(availables.Length - 1);
                string[] arr = availables.Split('|');
                // Console.WriteLine($"available_toMove : {arr.Length}");
                result = new int[arr.Length][];
                for (int i = 0; i < arr.Length; i++)
                {
                    result[i] = new int[] { int.Parse(arr[i][0] + ""), int.Parse(arr[i][1] + "") };
                }
            }
            else
            {
                result = new int[1][]; result[0] = null;
            }
            return result;
        }

        public override bool canCapture(Board boardObj)
        {
            int[][] arr = availablePositions_toCapture(boardObj);

            return arr[0] != null;
        }

        public override bool canMove(Board boardObj)
        {
            int[][] arr = availablePositions_toMove(boardObj);
            return arr[0] != null;
        }

        public override bool capture(int[] capPos, int[] figDelete , Board boardObj)
        {

            moveFigureToNewCell(capPos,boardObj);

            deleteFigureFromCell(figDelete, true,boardObj);
            return false;//needed for checker transformation (queen doesn't change)
        }

        public override void move(int[] movPos, Board boardObj)
        {
            moveFigureToNewCell(movPos, boardObj);
        }
    }
    class Checker : Figure
    {
        public Checker(bool player) : base(player, (player ? "\u265C" : "\u2656"), "Checker") { }


        public override bool canMove(Board boardObj)
        {
            bool allNull = Array.TrueForAll(availablePositions_toMove(boardObj), (x) => x == null);
            return !allNull;
        }

        public override bool canCapture(Board boardObj)
        {
            bool allNull = Array.TrueForAll(availablePositions_toCapture(boardObj), (x) => x == null);
            return !allNull;
        }
        public override int[][] availablePositions_toCapture(Board boardObj)
        {
            //RETURNS NULL FOR illegal moves (out of board) - or indexers //upL, upR, DownL, DownR
            int[][] available_JumpPositions = getLegalCellsAround(2, true, row, col);
            int[][] available_CapturePositions = getLegalCellsAround(1, true, row, col);
            int[][] result = new int[2][];
            if (available_JumpPositions != null && available_CapturePositions != null)
            {
                result = new int[2][];
                for (int i = 0; i < available_JumpPositions.Length; i++)
                {
                    //if available_ not null it indicates space onBoard
                    if (available_JumpPositions[i] != null && boardObj.pieceBelongsToPlayer(available_CapturePositions[i], !color) && boardObj.board[available_JumpPositions[i][0]][available_JumpPositions[i][1]] == null)
                    {
                        result[i] = available_JumpPositions[i];
                    }
                }
            }
            return result;


        }
        public override int[][] availablePositions_toMove(Board boardObj)
        {
            //returns legal move cells within the board range
            int[][] available_MovePositions = getLegalCellsAround(1, true, row, col);
            int[][] result = new int[2][];
            if (available_MovePositions != null)
            {
                Figure obj = null;
                for (int i = 0; i < available_MovePositions.Length; i++)
                {
                    if (available_MovePositions[i] != null)
                        obj = boardObj.board[available_MovePositions[i][0]][available_MovePositions[i][1]];
                    if (obj == null)
                    {

                        result[i] = available_MovePositions[i];
                    }

                }
            }

            return result;
        }
        
        public override bool capture(int[] movPos, int[] delPos, Board boardObj)
        {
            bool transformed = false;

            if (movPos[0] == 0 || movPos[0] == boardObj.board.Length - 1)
            {
                boardObj.createPieceOnBoard(color, movPos[0], movPos[1], "queen");

                transformed = true;
                deleteFigureFromCell(new int[] { row, col }, true,boardObj);
            }
            else moveFigureToNewCell(movPos,boardObj);

            deleteFigureFromCell(delPos, true,boardObj);
            return transformed;
        }

        public override void move(int[] movPos , Board boardObj)
        {
            if (movPos[0] == 0 || movPos[0] == boardObj.board.Length - 1)
            {
                deleteFigureFromCell(new int[] { row, col }, false,boardObj);
                boardObj.createPieceOnBoard(color, movPos[0], movPos[1], "queen");
            }
            else moveFigureToNewCell(movPos, boardObj);


        }


    }

}

