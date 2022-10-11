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
        }
    }


    class Board
    {
        string letters; //on board
        bool currentPlayer; // black/white
        public int blackOnBoard, whiteOnBoard; //figure count
        public Imove[][] board;

        public int idsCount; //for position setter
        ///test board constructor
        bool queen;
        bool delete;
        int moveCounter; //for game break

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

        public void createCheckerOnBoard(bool color, int row, int col, bool queen)
        {
            if (queen)
                board[row][col] = new Queen(color, this);
            else
                board[row][col] = new Checker(color, this);
            ((Figure)board[row][col]).setPosition();
            //  Console.WriteLine(board[row][col].ToString() + "row:" + ((Figure)board[row][col]).row + " col: " + ((Figure)board[row][col]).col);
        }
        void initiateBoard()
        {
            board = new Imove[8][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new Imove[8];
            }
        }
        //-CONSTRUCT BOARD
        void createCheckersBoard()
        {

            initiateBoard();

            //CREATE BOARD
            for (int i = 0; i < 8; i++)
            {
                int x;
                if (i == 0 || i == 2)
                {

                    for (x = 1; x < 8; x += 2)
                    {
                        createCheckerOnBoard(true, i, x, false);
                    }
                }
                else if (i == 7 || i == 5)
                {

                    for (x = 0; x < 8; x += 2)
                    {
                        createCheckerOnBoard(false, i, x, false);
                    }
                }
                if (i == 1)
                {

                    for (x = 0; x < 8; x += 2)
                    {
                        createCheckerOnBoard(true, i, x, false);
                    }
                }
                else if (i == 6)
                {

                    for (x = 1; x < 8; x += 2)
                    {
                        createCheckerOnBoard(false, i, x, false);
                    }
                }
            }
        } //default board



        bool byCheckersRule_legalPosition(int[] pos)
        {
            return (pos[0] % 2 == 0 && pos[1] % 2 != 0) || (pos[0] % 2 != 0 && pos[1] % 2 == 0);
        }
        public Board()
        {
            currentPlayer = false;
            letters = "     A    B    C    D    E    F    G    H";
            moveCounter = 15;
            idsCount = 0;
            queen = false;
            delete = false;
            bool start = false;
            do
            {
                Console.WriteLine("Create a Test board or a GamePlay board? (Test/Game)");

                string ans = Console.ReadLine();
                ans = ans.Trim().ToLower();
                switch (ans)
                {
                    default: Console.WriteLine("wrong input"); break;
                    case "game": start = true; createCheckersBoard(); break;
                    case "test":

                        initiateBoard();//sets arrays of objects

                        start = true;
                        printPrettyBoard(emptyBoard());
                        bool doneWithInput = false;
                        currentPlayer = false;
                        Console.WriteLine("\n* please enter positions (row/column) one by one (example: 1A , 3G):");
                        Console.WriteLine(" * REMEMBER! Black goes UP, White goes DOWN.\n");
                        string builderMsg;
                        string deleteMsg = "DELETE mode, enter position to delete: \n(type 'white' or 'black' to resume figure input or 'done' to start game play)";
                        string msg;
                        while (!doneWithInput)
                        {
                            builderMsg = $"{(currentPlayer ? "White" : "Black")} {(queen ? "Queen" : "Checker")} position: \n(type {(queen ? "'checker'" : "'queen'")} to switch figure ,type {(currentPlayer ? "'black'" : "'white'")} to continue entering {(currentPlayer ? "Black" : "White")} positions 'Done' to start game 'Delete' to delete mode)\n";
                            msg = delete ? deleteMsg : builderMsg;
                            int[] pos = legalPsitionOnBoard(msg, true);
                            if (pos == null)
                            {
                                printPrettyBoard(emptyBoard());
                            }

                            else if (pos.Length == 2 && byCheckersRule_legalPosition(pos) && !delete)
                            {
                                if (((currentPlayer && pos[0] == 7) || (!currentPlayer && pos[0] == 0)) && !queen)
                                {
                                    Console.WriteLine(" *** ILLEGAL:  you should select a queen figure to place in this position\n");
                                }
                                else
                                {
                                    createCheckerOnBoard(currentPlayer, pos[0], pos[1], queen);
                                    printPrettyBoard(emptyBoard());

                                }

                            }
                            else if (pos.Length == 2 && byCheckersRule_legalPosition(pos) && delete)
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
                                    switch (starts)
                                    {
                                        case "black": currentPlayer = false; doneWithInput = true; break;
                                        case "white": currentPlayer = true; doneWithInput = true; break;
                                        default: Console.WriteLine("ILLEGAL INPUT.\n"); break;
                                    }
                                }

                            }
                            else if (pos.Length == 2 && !byCheckersRule_legalPosition(pos))
                            {
                                Console.WriteLine("*** you can only place figures on the marked areas!\n");
                            }

                        }

                        break;
                }
            } while (!start);



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
            printPrettyBoard(prettyBoard());
            nextMove();
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
                            string inp = board[i][j] == null ? "XX" : ((Figure)board[i][j]).Unicode + " ";
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
                            string inp = board[i][j] == null ? "XX" : ((Figure)board[i][j]).Unicode;
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

                    if (board[i][j] != null) prettyB[i][j] = "| " + ((Figure)board[i][j]).Unicode + "  ";
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
            Console.WriteLine("Black:" + blackOnBoard + " white:" + whiteOnBoard + "\n");

        }

        //PRINTS END GAME summary
        public override string ToString()
        {
            return $"Black figures left on board: {blackOnBoard} , White figures left on board: {whiteOnBoard}";
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
                    if (board[i][j] != null && ((Figure)board[i][j]).color == currentPlayer)
                    {
                        if (board[i][j].canEat())
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
                    if (board[i][j] != null && ((Figure)board[i][j]).color == currentPlayer)
                    {
                        if (board[i][j].canMove())
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
        int[][] getPositionState()
        {


            bool legal = false;
            int[] figPos = new int[2];
            int[] movPos = new int[2];
            bool mustEat = false;
            if (testBoard())
            {

                while (!legal)
                {
                    //avail


                    //get player position input
                    string msg = $"{(currentPlayer ? "White" : "Black")} players turn! enter figure position to move (example - 2D):\nYou may end this game by typing 'resign' or ask your oponent for a tie by typing 'tie'. ";
                    figPos = legalPsitionOnBoard(msg, false);
                    //if returns null means game ended (tie/resign)!
                    if (figPos == null)
                    {
                        movPos = null; break;
                    }
                    else
                    {
                        Imove figRef = board[figPos[0]][figPos[1]];

                        mustEat = canEat_any();
                        //Console.WriteLine("MUST EAT: " + mustEat);

                        bool correctPlayer = belongsToPlayer(figPos, currentPlayer);
                        if (!correctPlayer)
                        {
                            illegalInput_print($"you did not select your {(currentPlayer ? "White" : "Black")} checker... ");
                        }
                        else if (mustEat && !figRef.canEat())
                        {
                            illegalInput_print($"you must eat opponents {(!currentPlayer ? "White" : "Black")} checker! select another checker... ");

                        }
                        else if (!mustEat && !figRef.canMove())
                        {
                            illegalInput_print($"this figure cant move anywhere,it's blocked by other figures. select another checker...");
                        }
                        else
                        {
                            // Console.WriteLine("CAN MOVE:" + figRef.checkMove());
                            // Console.WriteLine("CAN EAT:" + figRef.checkEat());
                            while (!legal)
                            {
                                bool transformed = false;

                                //!GENERATE AVAILABLE MOVES!!!!
                                int[][] availableMovesFull;
                                if (mustEat)
                                    availableMovesFull = figRef.available_toEat();
                                else
                                    availableMovesFull = figRef.available_toMove();
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
                                msg = ((Figure)board[figPos[0]][figPos[1]]).ToString() + " to where? :";
                                movPos = legalPsitionOnBoard(msg, false);
                                // }
                                // #endregion
                                //!returns null for resign or tie
                                if (movPos == null)
                                {
                                    legal = true;
                                    figPos = null;
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
                                            if (!mustEat)
                                            {
                                                board[figPos[0]][figPos[1]].move(movPos);
                                                moveCounter--; //used to count moves with no action or changes for figures amount on the board
                                                currentPlayer = !currentPlayer;
                                                printPrettyBoard(prettyBoard());
                                                legal = true;
                                                nextMove();

                                            }
                                            else
                                            {

                                                Console.WriteLine("You may eat your opponents Checker!");
                                                int delFig_r = figPos[0] < movPos[0] ? movPos[0] - 1 : movPos[0] + 1;
                                                int delFig_c = figPos[1] < movPos[1] ? movPos[1] - 1 : movPos[1] + 1;


                                                transformed = board[figPos[0]][figPos[1]].eat(movPos, new int[] { delFig_r, delFig_c });

                                                printPrettyBoard(prettyBoard());
                                                bool eatMore = board[movPos[0]][movPos[1]].canEat();
                                                moveCounter = 15;
                                                if (eatMore && !transformed)
                                                {
                                                    if (verify("Continue jumping over opponents checkers?"))
                                                    {
                                                        figPos = movPos;
                                                        figRef = board[figPos[0]][figPos[1]];
                                                    }
                                                    else
                                                    {
                                                        legal = true;

                                                        ((Figure)board[movPos[0]][movPos[1]]).deleteFigureFromBoard(new int[] { movPos[0], movPos[1] },true);
                                                        printPrettyBoard(prettyBoard());
                                                        Console.WriteLine("BURN!");
                                                        currentPlayer = !currentPlayer;

                                                        nextMove();
                                                    }
                                                }
                                                else
                                                {
                                                    legal = true;
                                                    currentPlayer = !currentPlayer;
                                                    nextMove();
                                                }
                                            }

                                            // } //uncomment to verifyMove with yes/no
                                        }
                                    }
                                    if (!legal && verifyMove) illegalInput_print("all available moves are marked...try again");
                                }

                            }
                        }
                    }
                }
            }
            int[] eat_or_move = mustEat ? new int[1] { 0 } : null;
            return new int[3][] { figPos, movPos, eat_or_move };

        }




        //!!ADD test func and move func
        public void nextMove()
        {
            //runs checkup for legal board moves (on board/players figure) and next move options
            int[][] moveSelected = getPositionState();
            if (moveSelected[0] == null)
            {
                Console.WriteLine(ToString());
            }

        }
        void illegalInput_print(string correction = "")
        {
            Console.WriteLine();
            Console.WriteLine("Illegal input! ");
            if (correction != "")
                Console.WriteLine(correction);
            Console.WriteLine();


        }
        //CHECKS IF FIGURE ON THE BOARD BELONGS TO SPECIFIED PLAYER 
        public bool belongsToPlayer(int[] pos, bool color)
        {
            return board[pos[0]][pos[1]] != null && ((Figure)board[pos[0]][pos[1]]).color == color;

        }


        //CHECKS If INPUT IS IN THE BOARD RANGE
        public int[] legalPsitionOnBoard(string msg, bool builder)
        {
            Console.WriteLine(msg);
            string pos = Console.ReadLine();


            pos = pos.Trim().ToLower();
            int column = 0;
            string correction = "try looking at  the board and use only the row|column characters...";
            int[] correct()
            {
                illegalInput_print(correction); return legalPsitionOnBoard(msg, builder);
            }

            if (pos == "queen" && builder)
            {
                queen = true;
                delete = false;
                return null;
            }
            else if (builder && (pos == "white" || pos == "black"))
            {
                currentPlayer = pos == "white" ? true : false;
                delete = false;
                return null;
            }
            else if (builder && pos == "done")
            {
                delete = false;
                return new int[1] { 1 };
            }
            else if (builder && pos == "delete")
            {
                delete = true;
                return null;
            }


            else if (pos == "resign")
            {
                if (!resign())
                    return new int[2] { board.Length, board[0].Length };
                else return null;
            }
            else if (pos == "tie")
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
                    default: return correct();
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
                else return correct();

            }
            else return correct();
        }



        //check for game end options
        public bool testBoard()
        {

            if (moveCounter == 0)
            {
                Console.WriteLine("15 moves and nothing happened! Game over, it's a tie!");
                return false;
            }
            else if (whiteOnBoard == 0 || blackOnBoard == 0)
            {
                string winner = whiteOnBoard == 0 ? "Black" : "White";
                Console.WriteLine($"Game over! {winner} wins!");
                return false;
            }
            else if (!canEat_any() && !canMove_any())
            {
                Console.WriteLine($"Game over! no More Moves! {(!currentPlayer ? "white" : "black")} wins!");

                return false;
            }
            return true;
        }

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


    class CheckersGame
    {

    }
    class Figure
    {

        protected Board currBoardObj;
        public bool color;
        public string Unicode;
        protected string figName;
        public int row;
        public int col;
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

        public Figure(bool player, string figureCode, string figureName, Board board)
        {
            color = player;
            figName = figureName;
            Unicode = figureCode;
            currBoardObj = board;
            figId = (color ? "W" : "B") + currBoardObj.idsCount;
            currBoardObj.idsCount++;
            if (color) currBoardObj.whiteOnBoard++;
            else currBoardObj.blackOnBoard++;
        }
        public override string ToString()
        {
            return $"{(color ? "White" : "Black")} {figName} on {currBoardObj.getPositionName(new int[] { row, col })}";
        }

        public void setPosition()
        {
            bool found = false;
            for (int i = 0; i < currBoardObj.board.Length && !found; i++)
            {
                for (int j = 0; j < currBoardObj.board[i].Length && !found; j++)
                {
                    Figure obj = currBoardObj.board[i][j] as Figure;
                    if (Equals(obj))
                    {

                        row = i;
                        col = j;
                        found = true;
                    }
                }
            }
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

        //THIS INSTEAD!


        //public int[] getPositionToDeleteFig(int[] movPos)
        //{
        //    int delFig_r = row < movPos[0] ? movPos[0] - 1 : movPos[0] + 1;
        //    int delFig_c = col < movPos[1] ? movPos[1] - 1 : movPos[1] + 1;
        //    return new int[] { delFig_r, delFig_c };
        //}
        public void deleteFigureFromBoard(int[] delPos , bool adjustCount)
        {
            if (adjustCount)
            {
            bool color = ((Figure)currBoardObj.board[delPos[0]][delPos[1]]).color;
            if (color) currBoardObj.whiteOnBoard--;
            else currBoardObj.blackOnBoard--;

            }
            currBoardObj.board[delPos[0]][delPos[1]] = null;


        }
        //protected void adjustFigureCount(int plus_minus,bool color_)
        //{
        //    if(color_) currBoardObj.whiteOnBoard+=plus_minus;
        //    else currBoardObj.blackOnBoard+=plus_minus;
        //}
        protected void moveFigToNewPlace(int[] movPos)
        {

            Imove fig = currBoardObj.board[row][col];
            currBoardObj.board[movPos[0]][movPos[1]] = fig;
            deleteFigureFromBoard(new int[] { row, col },false);
            ((Figure)currBoardObj.board[movPos[0]][movPos[1]]).setPosition();
        }

    }

    class Queen : Figure, Imove
    {


        public Queen(bool player, Board board) : base(player, (player ? "\u265B" : "\u2655"), "Queen", board) { }

        public int[][] available_toEat()
        {
            //! first elem returns as the figure to eat
            int[][] result;
            int[][] availableAroundX = getLegalCellsAround(1, false, row, col);
            string availableIndexes = "";
            Imove[][] b = currBoardObj.board;

            void loop(int[] rc, int upDon, int lR)
            {
                int r = rc[0]; int c = rc[1];
                if (b[r][c] != null && ((Figure)b[r][c]).color != color)
                {
                    bool inRange = (r + upDon >= 0 && r + upDon < b.Length) && (c + lR >= 0 && c + lR < b[0].Length);
                    if (inRange && b[r + upDon][c + lR] == null)
                    {
                        availableIndexes += (r + upDon) + "" + (c + lR) + "|";


                    }
                }
                else if (b[r][c] == null)
                {
                    bool inRange = (r + upDon >= 0 && r + upDon < b.Length) && (c + lR >= 0 && c + lR < b[0].Length);
                    if (inRange)
                        loop(new int[] { r + upDon, c + lR }, upDon, lR);
                }


            }//uplR \ DLR
            if (availableAroundX[0] != null)
                loop(availableAroundX[0], -1, -1);
            if (availableAroundX[1] != null)
                loop(availableAroundX[1], -1, 1);
            if (availableAroundX[2] != null)
                loop(availableAroundX[2], 1, -1);
            if (availableAroundX[3] != null)
                loop(availableAroundX[3], 1, 1);
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

        public int[][] available_toMove()
        {
            int[][] result;
            int[][] availablaAroundX = getLegalCellsAround(1, false, row, col);
            string availables = "";
            Imove[][] b = currBoardObj.board;
            void loop(int[] rc, int upDon, int lR)
            {
                int r = rc[0]; int c = rc[1];
                if ((r >= 0 && r < b.Length && c >= 0 && c < b[0].Length) && b[r][c] == null)
                {
                    availables += r + "" + c + "|";
                    if (r + upDon >= 0 && r + upDon <= 7 && c + lR >= 0 && c + lR <= 7)
                    {

                        loop(new int[] { r + upDon, c + lR }, upDon, lR);
                    }
                }


            }//uplR \ DLR
            if (availablaAroundX[0] != null)
                loop(availablaAroundX[0], -1, -1);
            if (availablaAroundX[1] != null)
                loop(availablaAroundX[1], -1, 1);
            if (availablaAroundX[2] != null)
                loop(availablaAroundX[2], 1, -1);
            if (availablaAroundX[3] != null)
                loop(availablaAroundX[3], 1, 1);
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

        public bool canEat()
        {
            int[][] arr = available_toEat();

            return arr[0] != null;
        }

        public bool canMove()
        {
            int[][] arr = available_toMove();
            return arr[0] != null;
        }

        public bool eat(int[] eatPos, int[] figDelete)
        {

            moveFigToNewPlace(eatPos);

            deleteFigureFromBoard(figDelete,true);
            return false;//needed for checker transformation (queen doesn't change)
        }

        public void move(int[] movPos)
        {
            moveFigToNewPlace(movPos);
        }
    }
    class Checker : Figure, Imove
    {
        public Checker(bool player, Board board) : base(player, (player ? "\u265C" : "\u2656"), "Checker", board) { }


        public bool canMove()
        {
            bool allNull = Array.TrueForAll(available_toMove(), (x) => x == null);
            return !allNull;
        }

        public bool canEat()
        {
            bool allNull = Array.TrueForAll(available_toEat(), (x) => x == null);
            return !allNull;
        }
        public int[][] available_toEat()
        {
            //RETURNS NULL FOR illegal moves (out of board) - or indexers //upL, upR, DownL, DownR
            int[][] availableJumps = getLegalCellsAround(2, true, row, col);
            int[][] availableEats = getLegalCellsAround(1, true, row, col);
            int[][] result = new int[2][];
            if (availableJumps != null && availableEats != null)
            {
                result = new int[2][];
                for (int i = 0; i < availableJumps.Length; i++)
                {
                    //if available not null it indicates space onBoard
                    if (availableJumps[i] != null && currBoardObj.belongsToPlayer(availableEats[i], !color) && currBoardObj.board[availableJumps[i][0]][availableJumps[i][1]] == null)
                    {
                        result[i] = availableJumps[i];
                    }
                }
            }
            return result;


        }
        public int[][] available_toMove()
        {
            //returns illegal squares within the board range
            int[][] available = getLegalCellsAround(1, true, row, col);
            int[][] result = new int[2][];
            if (available != null)
            {
                Imove obj = null;
                for (int i = 0; i < available.Length; i++)
                {
                    if (available[i] != null)
                        obj = currBoardObj.board[available[i][0]][available[i][1]];
                    if (obj == null)
                    {

                        result[i] = available[i];
                    }

                }
            }

            return result;
        }

        public bool eat(int[] movPos, int[] delPos)
        {
            bool transformed = false;

            if (movPos[0] == 0 || movPos[0] == currBoardObj.board.Length - 1)
            {
                currBoardObj.createCheckerOnBoard(color, movPos[0], movPos[1], true);
                
                transformed = true;
                deleteFigureFromBoard(new int[] { row, col },true);
            }
            else moveFigToNewPlace(movPos);

            deleteFigureFromBoard(delPos,true);
            return transformed;
        }

        public void move(int[] movPos)
        {
            if (movPos[0] == 0 || movPos[0] == currBoardObj.board.Length - 1)
            {
                deleteFigureFromBoard(new int[] { row, col },false);
                currBoardObj.createCheckerOnBoard(color, movPos[0], movPos[1], true);
            }
            else moveFigToNewPlace(movPos);


        }


    }

    interface Imove
    {
        bool canEat();
        bool canMove();

        int[][] available_toEat();
        int[][] available_toMove();
        bool eat(int[] eatPos, int[] delPos);
        void move(int[] movPos);
        //int[][] getLegalsAround( int steps, bool forwardOnly);

    }
}

