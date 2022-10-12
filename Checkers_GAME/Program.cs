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
            Board checkers = new CheckersGame();
            checkers.Play();
        }
    }//TODO setup program to different board games

    class CheckersGame : Board
    {
        bool builder_isQueen; //for builder

        public CheckersGame(bool standardBoardSize = true) : base(false, 15, standardBoardSize) { }
        protected override void _GameLoop()
        {


            bool moveMade = false;
            int[] figPos;
            int[] movPos;
            bool mustCapture;
            if (!EndGame())
            {

                while (!moveMade)
                {
                    //avail


                    //get player position input
                    string msg = $"{(_currPlayerColor ? "White" : "Black")} players turn! enter figure position to move (example - 2D):\nYou may end this game by typing 'resign' or ask your oponent for a tie by typing 'tie'. ";
                    figPos = _legalInput_Handler(msg, false);
                    //if returns null means game ended (tie/resign)!
                    if (figPos == null)
                    {
                        movPos = null; break;
                    }
                    else
                    {
                        Figure figRef = board[figPos[0]][figPos[1]];

                        mustCapture = _CanAnyFigCapture();
                        //Console.WriteLine("MUST EAT: " + mustEat);


                        if (figRef.Color != _currPlayerColor)
                        {
                            _Print_illegalInput($"you did not select your {(_currPlayerColor ? "White" : "Black")} checker... ");
                        }
                        else if (mustCapture && !figRef.CanCapture(this))
                        {
                            _Print_illegalInput($"you must eat opponents {(!_currPlayerColor ? "White" : "Black")} checker! select another checker... ");

                        }
                        else if (!mustCapture && !figRef.CanMove(this))
                        {
                            _Print_illegalInput($"this figure cant move anywhere,it's blocked by other figures. select another checker...");
                        }
                        else
                        {
                            // Console.WriteLine("CAN MOVE:" + figRef.checkMove());
                            // Console.WriteLine("CAN EAT:" + figRef.checkEat());
                            while (!moveMade)
                            {
                                bool transformed;

                                //!GENERATE AVAILABLE MOVES!!!!
                                int[][] availableMoves;
                                if (mustCapture)
                                    availableMoves = figRef.AvailablePositions_toCapture(this);
                                else
                                    availableMoves = figRef.AvailablePositions_toMove(this);
                                //? not necessary



                                /////////////////////////////////
                                //#region uncomment for auto move if there is only one option
                                //if (availableMoves.Length == 1)
                                //{
                                //    movPos = availableMoves[0];
                                //}
                                //else
                                //{

                                _PrintPrettyBoard(_PrettyBoard_construct(availableMoves));
                                msg = figRef.ToString() + " to where? :";
                                movPos = _legalInput_Handler(msg, false);
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
                                                figRef.Move(movPos, this);
                                                _movesCountTillEndgame--; //used to count moves with no action or changes for figures amount on the board
                                                _currPlayerColor = !_currPlayerColor;
                                                _PrintPrettyBoard(_PrettyBoard_construct());
                                                moveMade = true;
                                                _GameLoop();
                                            }
                                            else
                                            {

                                                Console.WriteLine("You may capture your opponents Checker!");

                                                int[] delFigPos = figRef.getCell_ofFigToDelete(movPos);
                                                transformed = figRef.Capture(movPos, new int[] { delFigPos[0], delFigPos[1] }, this);

                                                _PrintPrettyBoard(_PrettyBoard_construct());

                                                Figure afterMov_figRef = board[movPos[0]][movPos[1]];

                                                bool captureMore = afterMov_figRef.CanCapture(this);
                                                _movesCountTillEndgame = 15; //unset counter if capture was made to initial state

                                                if (captureMore && !transformed)
                                                {
                                                    if (_Verify("Continue jumping over opponents checkers?"))
                                                        figRef = afterMov_figRef;
                                                    else
                                                    {
                                                        moveMade = true;

                                                        deleteFigureFromCell(new int[] { movPos[0], movPos[1] }, true);

                                                        _PrintPrettyBoard(_PrettyBoard_construct());
                                                        Console.WriteLine("BURN!");

                                                        _currPlayerColor = !_currPlayerColor;
                                                        _GameLoop();
                                                    }
                                                }
                                                else
                                                {
                                                    moveMade = true;
                                                    _currPlayerColor = !_currPlayerColor;
                                                    _GameLoop();
                                                }
                                            }

                                            // } //uncomment to verifyMove with yes/no
                                        }
                                    }
                                    if (!moveMade && verifyMove) _Print_illegalInput("all available moves are marked...try again");
                                }

                            }
                        }
                    }
                }
            }


        }
        public override void placePieceOnBoard(bool isWhite, int row, int col, string pieceType)
        {
            if (pieceType == "queen")
                board[row][col] = new CheckersQueen(isWhite);
            else
                board[row][col] = new Checker(isWhite);
            base.placePieceOnBoard(isWhite, row, col, pieceType);
        }
        protected override void _AssembleStandardGame()
        {
            //CREATE BOARD
            for (int y = 0; y < _boardSize; y++)
            {
                int x;
                if (y == 0 || y == 2)
                {

                    for (x = 1; x < _boardSize; x += 2)
                    {
                        placePieceOnBoard(true, y, x, "");
                    }
                }
                else if (y == 7 || y == 5)
                {

                    for (x = 0; x < _boardSize; x += 2)
                    {
                        placePieceOnBoard(false, y, x, "");
                    }
                }
                if (y == 1)
                {

                    for (x = 0; x < _boardSize; x += 2)
                    {
                        placePieceOnBoard(true, y, x, "");
                    }
                }
                else if (y == 6)
                {

                    for (x = 1; x < _boardSize; x += 2)
                    {
                        placePieceOnBoard(false, y, x, "");
                    }
                }
            }
        }
        bool isBlackCellSelected(int[] pos)
        {
            return (pos[0] % 2 == 0 && pos[1] % 2 != 0) || (pos[0] % 2 != 0 && pos[1] % 2 == 0);
        }
        protected override void _DesignerGetsInput()
        {
            builder_isQueen = false;

            string builderMsg;
            string deleteMsg = "DELETE mode, enter position to delete: \n(type 'white' or 'black' to resume figure input or 'done' to start game play)";

            bool doneWithInput = false;
            string msg;
            while (!doneWithInput)
            {
                builderMsg = $"{(_currPlayerColor ? "White" : "Black")} {(builder_isQueen ? "Queen" : "Checker")} position: \n(type {(builder_isQueen ? "'checker'" : "'queen'")} to switch figure ,type {(_currPlayerColor ? "'black'" : "'white'")} to continue entering {(_currPlayerColor ? "Black" : "White")} positions 'Done' to start game 'Delete' to delete mode)\n";
                msg = _builderDeleteMode ? deleteMsg : builderMsg;
                int[] pos = _legalInput_Handler(msg, true);
                if (pos == null)
                {
                    _PrintPrettyBoard(_EmptyBoard_construct());
                } //user types setting input

                else if (pos.Length == 2 && isBlackCellSelected(pos) && !_builderDeleteMode)
                {
                    if (((_currPlayerColor && pos[0] == board.Length) || (!_currPlayerColor && pos[0] == 0)) && !builder_isQueen)
                    {
                        Console.WriteLine(" *** ILLEGAL:  you should select a queen figure to place in this position\n");
                    }
                    else
                    {
                        string piece = builder_isQueen ? "queen" : "";
                        placePieceOnBoard(_currPlayerColor, pos[0], pos[1], piece);
                        _PrintPrettyBoard(_EmptyBoard_construct());

                    }

                }// add fig
                else if (pos.Length == 2 && isBlackCellSelected(pos) && _builderDeleteMode)
                {

                    board[pos[0]][pos[1]] = null;
                    _PrintPrettyBoard(_EmptyBoard_construct());
                }// delete fig
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
                            case "black": _currPlayerColor = false; doneWithInput = true; break;
                            case "white": _currPlayerColor = true; doneWithInput = true; break;
                            default: Console.WriteLine("ILLEGAL INPUT.\n"); break;
                        }
                    }

                } //user types 'Done' to start the game
                else if (pos.Length == 2 && !isBlackCellSelected(pos))
                {
                    Console.WriteLine("*** you can only place figures on the marked areas!\n");
                }

            }
        }

        protected override int[] _addBuilderUserInputOptions(string input)
        {

            if ((input == "queen" || input == "checker"))
            {
                if (input == "queen")
                    builder_isQueen = true;
                else
                    builder_isQueen = false;
                _builderDeleteMode = false;
                return null;
            }

            else return new int[1];
        }

    }
    /*class DuoGames
    {
        protected virtual void printRules()
        {
            Console.WriteLine("Game Rules:");
        }
    }*/
    class Board /*: DuoGames*/
    {
        /// <summary>
        ///??? Designer : AssemblestandardGame >>>||| Interface I_practice 
        /// </summary>
        string _letters; //on board
        protected bool _currPlayerColor; // black/white
        public int blackOnBoard, whiteOnBoard; //figure count
        public Figure[][] board;
        protected int _boardSize;

        public int idsCount; //for position setter
        ///test board constructor
        protected bool _builderDeleteMode;
        protected int _movesCountTillEndgame; //for game break
        void _InitiateBoard()
        {
            board = new Figure[_boardSize][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new Figure[_boardSize];
            }
        }
        public Board(bool playerThatStartsIsWhite, int moveCounterForMovesWithNoChange, bool standardBoardSize)//TODO add size option
        {
            _currPlayerColor = playerThatStartsIsWhite;
            _letters = "     A    B    C    D    E    F    G    H";
            _boardSize = 8;
            if (!standardBoardSize)
            {
                _letters += "....I....G";
                _boardSize += 2;
            }
            _movesCountTillEndgame = moveCounterForMovesWithNoChange;
            idsCount = 0;

            _InitiateBoard();


        }

        protected /*override*/ void printRules()
        {

            // Console.WriteLine("GAME STARTS!!!");
            /*base.printRules();*/
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
        }

        public virtual void placePieceOnBoard(bool isWhite, int row, int col, string pieceType)
        {

            idsCount++;
            if (isWhite) whiteOnBoard++;
            else blackOnBoard++;
            (board[row][col]).setPosition_RowCol(this);
            //  Console.WriteLine(board[row][col].ToString() + "row:" + board[row][col].row + " col: " + board[row][col].col);
        }
        //-CONSTRUCT BOARD display
        protected string[][] _EmptyBoard_construct()
        {
            string[][] emptyB = new string[_boardSize][];
            for (int i = 0; i < _boardSize; i++)
            {
                emptyB[i] = new string[_boardSize];
                if (i % 2 == 0)
                {

                    for (int j = 0; j < _boardSize; j++)
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

                    for (int j = 0; j < _boardSize; j++)
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
        protected string[][] _PrettyBoard_construct()
        {
            string[][] prettyB = new string[_boardSize][];
            for (int i = 0; i < _boardSize; i++)
            {
                prettyB[i] = new string[_boardSize];
                for (int j = 0; j < _boardSize; j++)
                {

                    if (board[i][j] != null) prettyB[i][j] = "| " + board[i][j].Unicode + "  ";
                    else prettyB[i][j] = "|    ";
                }
            }
            return prettyB;
        }
        protected string[][] _PrettyBoard_construct(int[][] availablePos)
        {
            string[][] prettyB = _PrettyBoard_construct();
            for (int a = 0; a < availablePos.Length; a++)
            {

                int row = availablePos[a][0]; int col = availablePos[a][1];
                char[] ch = prettyB[row][col].ToCharArray();
                ch[1] = ch[4] = 'X';
                prettyB[row][col] = new string(ch);

            }
            return prettyB;
        }
        protected void _PrintPrettyBoard(string[][] prettyBoard)
        {
            Console.WriteLine(_letters);
            for (int i = 0; i < _boardSize; i++)
            {
                Console.Write(i + 1 + "  ");
                for (int j = 0; j < _boardSize; j++)
                {
                    Console.Write(prettyBoard[i][j]);
                }
                Console.Write("|");
                Console.WriteLine();
                Console.WriteLine("    ---- ---- ---- ---- ---- ---- ---- ----");
            }
            Console.WriteLine(ToString() + "\n");

        }
        //place Pieces on the board
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
        protected virtual void _AssembleStandardGame()
        { }

        protected void _DesignBoard()
        {

            _PrintPrettyBoard(_EmptyBoard_construct());
            Console.WriteLine("\n* please enter positions (row/column) one by one (example: 1A , 3G):");
            Console.WriteLine(" * REMEMBER! Black goes UP, White goes DOWN.\n");
            _DesignerGetsInput();
        }
        protected virtual void _DesignerGetsInput()
        {
            // asks user for input and verifies according to game rules
        }
        protected virtual int[] _addBuilderUserInputOptions(string input)
        {
            
            return null;
        }   //Game specified designer options {switch figure types}
         
        //CHECKS If INPUT IS IN THE BOARD RANGE or tie/resign
        protected int[] _legalInput_Handler(string msg, bool builder)
        {
            Console.WriteLine(msg);
            string pos = Console.ReadLine();


            pos = pos.Trim().ToLower();
            int column;
            string correction = "try looking at  the board and use only the row|column characters...or use instruction options";
            int[] corrector()
            {
                _Print_illegalInput(correction); return _legalInput_Handler(msg, builder);
            }

            if (builder)
            {
                int[] builderResult = _addBuilderUserInputOptions(pos);
                if (builderResult == null)
                    return null;
            }

            if ((pos == "white" || pos == "black") && builder)
            {
                _currPlayerColor = pos == "white" ? true : false;
                _builderDeleteMode = false;
                return null;
            }
            else if (pos == "done" && builder)
            {
                _builderDeleteMode = false;
                return new int[1] { 1 };
            }
            else if (pos == "delete" && builder)
            {
                _builderDeleteMode = true;
                return null;
            }

            else if (pos == "resign" && !builder)
            {
                if (!Resign())
                    return new int[2] { board.Length, board[0].Length };
                else return null;
            }
            else if (pos == "tie" && !builder)
            {
                if (!SuggestTie())
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
        protected void _Print_illegalInput(string correction = "")
        {
            Console.WriteLine($"\nIllegal input!\n{correction}\n");
        }



        public void Play()
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
                    case "game": start = true; _AssembleStandardGame(); break;
                    case "test":
                        start = true;
                        _builderDeleteMode = false; _DesignBoard(); break;
                }
            } while (!start);

            printRules();
            _PrintPrettyBoard(_PrettyBoard_construct());
            _GameLoop();
        }
        protected virtual void _GameLoop()
        {
            //GamePlay
        }
        public bool EndGame()
        {

            if (_movesCountTillEndgame == 0)
            {
                Console.WriteLine("Nothing happened For too Long... Game over, it's a tie!");
                return true;
            }
            else if (whiteOnBoard == 0 || blackOnBoard == 0)
            {
                string winner = whiteOnBoard == 0 ? "Black" : "White";
                Console.WriteLine($"Game over! {winner} wins!");
                return true;
            }
            else if (!_CanAnyFigCapture() && !_CanAnyFigMove())
            {
                Console.WriteLine($"Game over! no More Moves! {(!_currPlayerColor ? "white" : "black")} wins!");

                return true;
            }
            return false;
        }//TODO check for game end options  ***INTERFACE Igame
        bool SuggestTie()
        {
            if (_Verify("your opponent suggests a tie, what do you think?"))
            {
                Console.WriteLine("It's a tie!"); return true;
            }
            else return false;
        }
        bool Resign()
        {
            string player = _currPlayerColor ? "White" : "Black";
            string oponent = !_currPlayerColor ? "White" : "Black";
            if (_Verify("are you sure you want to resign?"))
            {
                Console.WriteLine($"Game ended. {(player)} resigned! {oponent} wins!");
                return true;
            }
            else return false;
        }
        //VERIFY YES/NO : question
        protected bool _Verify(string msg)
        {
            Console.WriteLine(" type Y/N : " + msg);
            string ans;
            ans = Console.ReadLine();
            switch (ans.Trim().ToLower())
            {
                case "y": return true;
                case "n": return false;
                default:
                    _Print_illegalInput(); return _Verify(msg);
            }
        }


        //play Methods
        bool _ifBoardCanEatOrCapture(bool askCanMove)
        {
            int checkersCount = _currPlayerColor ? whiteOnBoard : blackOnBoard;

            foreach (Figure[] boardRow in board)
            {
                foreach (Figure fig in boardRow)
                {
                    if (fig != null && fig.Color == _currPlayerColor)
                    {
                        bool tester = askCanMove ? fig.CanMove(this) : fig.CanCapture(this);
                        checkersCount--;
                        if (tester)
                        {
                            return true;
                        }
                        if (checkersCount == 0) return false;
                    }
                }
            }

            return false;
        }
        protected bool _CanAnyFigCapture()
        {
            return _ifBoardCanEatOrCapture(false);
        }
        protected bool _CanAnyFigMove()
        {
            return _ifBoardCanEatOrCapture(true);
        }



        public void deleteFigureFromCell(int[] delPos, bool needToAdjustCount)//adjustment to figure count made after deleting figure and not moving
        {
            if (needToAdjustCount)
            {
                bool isWhite = board[delPos[0]][delPos[1]].Color;
                if (isWhite) whiteOnBoard--;
                else blackOnBoard--;

            }
            board[delPos[0]][delPos[1]] = null;


        }
        //PRINTS GAME summary
        public override string ToString()
        {
            return $"Black on board: {blackOnBoard} , White on board: {whiteOnBoard}";
        }
        ////CHECKS IF FIGURE ON THE BOARD BELONGS TO SPECIFIED PLAYER 
        //public bool pieceBelongsToPlayer(int[] pos, bool isWhite)
        //{
        //    return board[pos[0]][pos[1]] != null && board[pos[0]][pos[1]].Color == isWhite;
        //}
    }



    class Figure
    {
        //? WHY USE INTERFACE: FOR DIFFERENT GAME TYPES LIKE CARD GAMES THAT HAS SIMILAR METHODS
        public bool CanCapture(Board boardObj)
        {
            int[][] arr = AvailablePositions_toCapture(boardObj);
            return arr.Length > 0;
        }

        public bool CanMove(Board boardObj)
        {
            int[][] arr = AvailablePositions_toMove(boardObj);
            return arr.Length > 0;
        }

        public virtual int[][] AvailablePositions_toCapture(Board board) { return null; }
        public virtual int[][] AvailablePositions_toMove(Board board) { return null; }
        public virtual bool Capture(int[] capPos, int[] delPos, Board board) { return true; }
        public virtual bool Move(int[] movPos, Board board) { return false; }


        public string Unicode;
        public bool Color;
        string _FigName;
        string _PrettyPosition;
        string _FigId;
        protected int _Row;
        protected int _Col;

        public override bool Equals(object obj)
        {
            if (obj is Figure)
            {
                Figure fig = obj as Figure;
                string id = fig._FigId;
                return id == this._FigId;
            }
            else { return false; }
        }
        public override string ToString()
        {
            return $"{(Color ? "White" : "Black")} {_FigName} on {_PrettyPosition}";
        }


        public Figure(bool isWhitePlayer, string figureCode, string figureName)
        {
            Color = isWhitePlayer;
            _FigName = figureName;
            Unicode = figureCode;

        }

        public void setPosition_RowCol(Board boardObj)
        {
            _FigId = (Color ? "W" : "B") + boardObj.idsCount;
            bool found = false;
            for (int i = 0; i < boardObj.board.Length && !found; i++)
            {
                for (int j = 0; j < boardObj.board[i].Length && !found; j++)
                {
                    if (Equals(boardObj.board[i][j]))
                    {

                        _Row = i;
                        _Col = j;
                        found = true;
                    }
                }
            }
            _PrettyPosition = boardObj.getPositionName(new int[] { _Row, _Col });
        }

        protected int[][] _getLegalCellsAround(int steps, bool forwardOnly, int row, int col)
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
                if (Color && (downLeftB || downRightB)) arr = new int[2][] { downRight, downLeft };
                else if (!Color && (upRightB || upLeftB)) arr = new int[2][] { upLeft, upRight };
                else arr = null;
            }
            return arr;


        }


        public int[] getCell_ofFigToDelete(int[] movPos)
        {
            int delFig_r = _Row < movPos[0] ? movPos[0] - 1 : movPos[0] + 1;
            int delFig_c = _Col < movPos[1] ? movPos[1] - 1 : movPos[1] + 1;
            return new int[] { delFig_r, delFig_c };
        }

        protected void _moveFigureToNewCell(int[] movPos, Board boardObj)
        {
            Figure[][] board = boardObj.board;
            Figure fig = board[_Row][_Col];
            board[movPos[0]][movPos[1]] = fig;
            boardObj.deleteFigureFromCell(new int[] { _Row, _Col }, false);
            board[movPos[0]][movPos[1]].setPosition_RowCol(boardObj);
        }


    }

    class CheckersQueen : Figure
    {

        public CheckersQueen(bool player) : base(player, (player ? "\u265B" : "\u2655"), "Queen") { }

        /// ////////////////
        int[][] _StringToArray(string availables)
        {

            int[][] result;
            availables = availables.Remove(availables.Length - 1);
            string[] arr = availables.Split('|');
            // Console.WriteLine($"available_toMove : {arr.Length}");
            result = new int[arr.Length][];
            for (int i = 0; i < arr.Length; i++)
            {
                result[i] = new int[] { int.Parse(arr[i][0] + ""), int.Parse(arr[i][1] + "") };
            }
            return result;
        }
        string _GetAllAvailableIndexesToTheTurn(Func<int[], int, int, string> loopFunc)
        {
            int[][] availableCellsAroundFig = _getLegalCellsAround(1, false, _Row, _Col);

            string availableIndexes = "";
            string str = "";
            if (availableCellsAroundFig[0] != null)
            {
                str = (loopFunc(availableCellsAroundFig[0], -1, -1));
                availableIndexes += str;
            }
            if (availableCellsAroundFig[1] != null)
            {
                str = loopFunc(availableCellsAroundFig[1], -1, 1);
                availableIndexes += str;

            }
            if (availableCellsAroundFig[2] != null)
            {
                str = loopFunc(availableCellsAroundFig[2], 1, -1);
                availableIndexes += str;

            }
            if (availableCellsAroundFig[3] != null)
            {
                str = loopFunc(availableCellsAroundFig[3], 1, 1);
                availableIndexes += str;

            }
            return availableIndexes;
        }
        public override int[][] AvailablePositions_toCapture(Board boardObj)
        {
            Figure[][] board = boardObj.board;
            int[][] result;
            string availableIndexes = "";

            //adds all available Jumps from the position to a string that will contain all directions positions
            string getAvailableIndexesToString(int[] rc_, int upDon_, int lR_)
            {

                string availableIn = "";
                void loop(int[] rc, int upDon, int lR)
                {

                    int r = rc[0]; int c = rc[1];
                    if (board[r][c] != null && board[r][c].Color != Color)
                    {
                        bool inRange = (r + upDon >= 0 && r + upDon < board.Length) && (c + lR >= 0 && c + lR < board[0].Length);
                        if (inRange && board[r + upDon][c + lR] == null)
                        {
                            availableIn += (r + upDon) + "" + (c + lR) + "|";


                        }
                    }
                    else if (board[r][c] == null)
                    {
                        bool inRange = (r + upDon >= 0 && r + upDon < board.Length) && (c + lR >= 0 && c + lR < board[0].Length);
                        if (inRange)
                            loop(new int[] { r + upDon, c + lR }, upDon, lR);
                    }
                }
                loop(rc_, upDon_, lR_);
                return availableIn;

            }

            availableIndexes = _GetAllAvailableIndexesToTheTurn(getAvailableIndexesToString);

            if (availableIndexes != "")
                result = _StringToArray(availableIndexes);
            else
                result = new int[0][];
            return result;
        }
        public override int[][] AvailablePositions_toMove(Board boardObj)
        {
            int[][] result;
            string availables = "";
            Figure[][] board = boardObj.board;
            string gatAvailableIndexesToString(int[] rc_, int upDon_, int lR_)
            {
                string availablesStr = "";
                void loop(int[] rc, int upDon, int lR)
                {
                    int r = rc[0]; int c = rc[1];
                    if ((r >= 0 && r < board.Length && c >= 0 && c < board[0].Length) && board[r][c] == null)
                    {
                        availablesStr += r + "" + c + "|";
                        if (r + upDon >= 0 && r + upDon <= 7 && c + lR >= 0 && c + lR <= 7)
                        {

                            loop(new int[] { r + upDon, c + lR }, upDon, lR);
                        }
                    }

                }
                loop(rc_, upDon_, lR_);
                return availablesStr;
            }
            //uplR \ DLR

            availables = _GetAllAvailableIndexesToTheTurn(gatAvailableIndexesToString);
            if (availables != "")
                result = _StringToArray(availables);
            else
                result = new int[0][];
            return result;
        }

        /// /////////////////

        public override bool Capture(int[] capPos, int[] figDelete, Board boardObj)
        {

            _moveFigureToNewCell(capPos, boardObj);

            boardObj.deleteFigureFromCell(figDelete, true);
            return false;//needed for checker transformation (queen doesn't change)
        }

        public override bool Move(int[] movPos, Board boardObj)
        {
            _moveFigureToNewCell(movPos, boardObj);
            return false;//needed for checker transformation (queen doesn't change)

        }
    }
    class Checker : Figure
    {
        public Checker(bool player) : base(player, (player ? "\u265C" : "\u2656"), "Checker") { }


        public override int[][] AvailablePositions_toCapture(Board boardObj)
        {
            //RETURNS NULL FOR illegal moves (out of board) - or indexers //upL, upR, DownL, DownR
            int[][] available_JumpPositions = _getLegalCellsAround(2, true, _Row, _Col);
            int[][] available_CapturePositions = _getLegalCellsAround(1, true, _Row, _Col);
            int[][] availableMoves = new int[2][];
            if (available_JumpPositions != null && available_CapturePositions != null)
            {

                for (int i = 0; i < available_JumpPositions.Length; i++)
                {
                    //if available_ not null it indicates space onBoard
                    Figure captureFig = boardObj.board[available_JumpPositions[i][0]][available_JumpPositions[i][1]];
                    bool capturePositionBelongsToOponent = captureFig.Color != Color;//instead OF :  boardObj.pieceBelongsToPlayer(available_CapturePositions[i], !Color)
                    if (available_JumpPositions[i] != null && capturePositionBelongsToOponent && boardObj.board[available_JumpPositions[i][0]][available_JumpPositions[i][1]] == null)
                        availableMoves[i] = available_JumpPositions[i];
                }
            }
            return availableMoves.Where(val => val != null).ToArray();

        }
        public override int[][] AvailablePositions_toMove(Board boardObj)
        {
            //returns legal move cells within the board range
            int[][] available_MovePositions = _getLegalCellsAround(1, true, _Row, _Col);
            int[][] availableMoves = new int[2][];
            if (available_MovePositions != null)
            {
                Figure obj = null;
                for (int i = 0; i < available_MovePositions.Length; i++)
                {
                    if (available_MovePositions[i] != null)
                    {

                        obj = boardObj.board[available_MovePositions[i][0]][available_MovePositions[i][1]];
                        if (obj == null)
                            availableMoves[i] = available_MovePositions[i];
                    }

                }
            }

            return availableMoves.Where(val => val != null).ToArray();

        }

        public override bool Capture(int[] movPos, int[] delPos, Board boardObj)
        {
            boardObj.deleteFigureFromCell(delPos, true);
            return Move(movPos, boardObj);
        }

        public override bool Move(int[] movPos, Board boardObj)
        {
            bool transformed = false;
            if (movPos[0] == 0 || movPos[0] == boardObj.board.Length - 1)
            {
                boardObj.placePieceOnBoard(Color, movPos[0], movPos[1], "queen");
                boardObj.deleteFigureFromCell(new int[] { _Row, _Col }, false);
                transformed = true;

            }
            else _moveFigureToNewCell(movPos, boardObj);
            return transformed;

        }
    }
}


