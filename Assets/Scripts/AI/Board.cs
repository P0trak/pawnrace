using System.Collections.Generic;
using UnityEngine;

public class Board
{
    Square[,] board;

    // this will be tweaked (and probably won't end up being linear) 
    int POSITION_FACTOR = 2;

    Move lastMove = null;

    bool gameOver = false;

    bool whiteMove = true;

    public Board(char whiteGap, char blackGap)
    {
        board = new Square[8, 8];
        int w = (int) whiteGap - (int) 'a';
        int b = (int) blackGap - (int) 'a';

        Debug.Log("Creating new board: " + w.ToString() + " " + b.ToString());

        for (int i = 0; i < board.GetLength(1); i++)
        {
            if (i != b) 
            {
                board[1, i] = Square.BLACK;
            }
            if (i != w) 
            {
                board[6, i] = Square.WHITE;
            }
        }

        Debug.Log("Board just after creation:\n" + this.ToString());
    }

    public Board(char whiteGap, char blackGap, Move lastMove)  : this(whiteGap, blackGap)
    {
        this.lastMove = lastMove;
    }

    public Board(Board other, Move lastMove)
    {
        this.board = CopyOf(other.board);
        this.gameOver = other.gameOver;
        this.whiteMove = other.whiteMove;
        this.lastMove = lastMove;
    }

    Square[,] CopyOf(Square[,] other)
    {
        Square[,] copy = new Square[other.GetLength(0), other.GetLength(1)];
        for (int i = 0; i < other.GetLength(0); i++)
        {
            for (int j = 0; j < other.GetLength(1); j++)
            {
                copy[i,j] = other[i,j];
            }
        }
        return copy;
    }


    /*
    
    the idea here:

    - a neutral board should score 0
    - positive scores favour white, negative favour black
    - for example, how far each pawn has advanced is added to the score as (y displacement * factor)
    - passed pawns have their score multiplied massively
    - this strategy seems to promote advancing pawns and getting captures
    - maybe don't just increase score linearly with displacement
    - pawns that can't move aren't counted?

    */
    public int GetScore()
    {
        int total = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == Square.WHITE)
                {
                    if (i == 7) return int.MaxValue;
                    total -= (i - 6) * (i - 6);
                }
                else if (board[i, j] == Square.BLACK)
                {
                    if (i == 0) return int.MinValue;
                    total += (i - 1) * (i - 1);
                }

            }
        }
        return (0 - total);
    }


    public void UpdateBoard(Move move)
    {
        
        // - transform move.from and move.to

        /*
        if (move.isCapture)
        {
            Debug.Log("current board state:\n");
            Debug.Log(this.ToString());

            Debug.Log("Updating board with this move: " + move.ToString());
        }
        */

        

        
        Vector2Int from = ToBoardPos(move.from);

        /*
        if (move.isCapture)
        {
            Debug.Log("move from is " + move.from.ToString());
            Debug.Log("choosing pawn at position " + from.ToString());
        }
        */

        
        Vector2Int to = ToBoardPos(move.to);

        /*
        if (move.isCapture)
        {
            Debug.Log("move to is " + move.to.ToString());
            Debug.Log("moving to " + to.ToString());
        }
        */

        

        // - make move.from empty

        board[from.x, from.y] = Square.EMPTY;

        // - make move.to whiteMove ? Square.WHITE : Square.BLACK

        board[to.x, to.y] = move.whiteMove ? Square.WHITE : Square.BLACK;

        // - if enPassant then make (move.from.y, move.to.x) empty

        if (move.enPassant)
        {
            board[to.x, from.y] = Square.EMPTY;
        }

        this.whiteMove = !this.whiteMove;

        /*
        if (move.isCapture)
        {
            Debug.Log("board is now:\n");
            Debug.Log(this.ToString());
        }
        */

        CheckGameOver();
    }


        public Board UndoMove(Move move, Move grandparent)
    {
        
        // - transform move.from and move.to

        /*
        if (move.isCapture)
        {
            Debug.Log("current board state:\n");
            Debug.Log(this.ToString());

            Debug.Log("Updating board with this move: " + move.ToString());
        }
        */

        

        
        Vector2Int from = ToBoardPos(move.from);

        /*
        if (move.isCapture)
        {
            Debug.Log("move from is " + move.from.ToString());
            Debug.Log("choosing pawn at position " + from.ToString());
        }
        */

        
        Vector2Int to = ToBoardPos(move.to);

        /*
        if (move.isCapture)
        {
            Debug.Log("move to is " + move.to.ToString());
            Debug.Log("moving to " + to.ToString());
        }
        */

        

        // - make move.to empty if move was not non-ep capture

        if (!move.isCapture || move.enPassant)
        {
            board[to.x, to.y] = Square.EMPTY;
        }
        else
        {
            board[to.x, to.y] = move.whiteMove ? Square.BLACK : Square.WHITE;
        }
        

        // - make move.from whiteMove ? Square.WHITE : Square.BLACK

        board[from.x, from.y] = move.whiteMove ? Square.WHITE : Square.BLACK;

        // - if enPassant then make (move.from.y, move.to.x) empty

        if (move.enPassant)
        {
            board[from.x, to.y] = move.whiteMove ? Square.BLACK : Square.WHITE;
        }

        this.whiteMove = !this.whiteMove;

        /*
        if (move.isCapture)
        {
            Debug.Log("board is now:\n");
            Debug.Log(this.ToString());
        }
        */

        return new Board(this, grandparent);

    }

    void CheckGameOver() 
    {

        if (GetAllMoves().Count == 0) 
        {
            gameOver = true;
            return;
        }

        for (int j = 0; j < board.GetLength(1); j++)
        {
            if (board[0, j] == Square.BLACK || board[7, j] == Square.WHITE)
            { 
                gameOver = true;
                return;
            }

        }

        bool noWhites = true;
        bool noBlacks = true;

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i,j] == Square.WHITE)
                {
                    noWhites = false;
                }
                else if (board[i,j] == Square.BLACK)
                {
                    noBlacks = false;
                }
            }
        }
        gameOver = noWhites || noBlacks;
    }

    public List<Move> GetAllMoves() 
    {
        if (gameOver) return new List<Move>();

        Square target = whiteMove ? Square.WHITE : Square.BLACK;
        List<Move> moves = new List<Move>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i,j] == target)
                {
                    moves.AddRange(GetMoves(i,j));
                }
            }
        }

        return moves;
    }

    Vector2Int ToBoardPos(Vector2Int position)
    {
        return new Vector2Int(7 - (position.y + 4), position.x + 4);
    }

    Vector2Int ToGamePos(int rank, int file)
    {
        return new Vector2Int(file - 4, 7 - (rank + 4));
    }


    //TODO: how on earth am I gonna do en passant (maybe use a stack?)
    List<Move> GetMoves(int rank, int file)
    {
        Square from = board[rank, file];
        if (from == Square.EMPTY) return new List<Move>();
        
        List<Move> moves = new List<Move>();
        

        if (from == Square.BLACK)
        {
            if (rank < 7) 
            {
                if (board[rank + 1, file] == Square.EMPTY)
                {
                    Vector2Int to = ToGamePos(rank + 1, file);
                    moves.Add(new Move(ToGamePos(rank, file), to, false, false, false));
                    if (rank == 1 && board[rank + 2, file] == Square.EMPTY)
                        {
                            to = ToGamePos(rank + 2, file);
                            moves.Add(new Move(ToGamePos(rank, file), to, false, false, false));
                        }
                }
                if (file > 0)
                {
                    if (board[rank + 1, file - 1] == Square.WHITE)
                    {
                        Vector2Int to = ToGamePos(rank + 1, file - 1);
                        moves.Add(new Move(ToGamePos(rank, file), to, false, true, false));
                    }
                }
                if (file < 7)
                {
                    if (board[rank + 1, file + 1] == Square.WHITE)
                    {
                        Vector2Int to = ToGamePos(rank + 1, file + 1);
                        moves.Add(new Move(ToGamePos(rank, file), to, false, true, false));
                    }
                }

                if (rank == 4)
                {

                    Vector2Int lastFrom = ToBoardPos(lastMove.from);
                    Vector2Int lastTo = ToBoardPos(lastMove.to);
                    int lr = lastTo.y - file;

                    if (Mathf.Abs(lr) == 1)
                    {
                        if (lastFrom.x == 6 && lastTo.x == 4)
                        {
                            Vector2Int to = ToGamePos(rank + 1, file + lr);
                            Debug.Log("en passant possible!");
                            moves.Add(new Move(ToGamePos(rank, file), to, false, true, false, true));
                        }
                    }
                }
            }
        }
        else
        {
            if (rank > 0) 
            {
                if (board[rank - 1, file] == Square.EMPTY)
                {
                    
                    Vector2Int to = ToGamePos(rank - 1, file);
                    Move move = new Move(ToGamePos(rank, file), to, true, false, false);
                    //Debug.Log("Possible move: " + move.ToString());
                    moves.Add(move);
                    if (rank == 6 && board[rank - 2, file] == Square.EMPTY)
                    {
                        to = ToGamePos(rank - 2, file);
                        move = new Move(ToGamePos(rank, file), to, true, false, false);
                        //Debug.Log("Possible move: " + move.ToString());
                        moves.Add(move);
                    }
                }
                if (file > 0)
                {
                    if (board[rank - 1, file - 1] == Square.BLACK)
                    {
                        Vector2Int to = ToGamePos(rank - 1, file - 1);
                        moves.Add(new Move(ToGamePos(rank, file), to, true, true, false));
                    }
                }
                if (file < 7)
                {
                    if (board[rank - 1, file + 1] == Square.BLACK)
                    {
                        Vector2Int to = ToGamePos(rank - 1, file + 1);
                        moves.Add(new Move(ToGamePos(rank, file), to, true, true, false));
                    }
                }

                if (rank == 3)
                {
                    Vector2Int lastFrom = ToBoardPos(lastMove.from);
                    Vector2Int lastTo = ToBoardPos(lastMove.to);
                    int lr = lastTo.y - file;

                    if (Mathf.Abs(lr) == 1)
                    {
                        if (lastFrom.x == 1 && lastTo.x == 3)
                        {
                            Vector2Int to = ToGamePos(rank - 1, file + lr);
                            Debug.Log("en passant possible!");
                            moves.Add(new Move(ToGamePos(rank, file), to, true, true, false, true));
                        }
                    }
                }
            }
        }

        return moves;
    }

    public override string ToString()
    {

        string output = "";
        for (int i = 0; i < board.GetLength(0); i++)
        {
            string line = "";
            for (int j = 0; j < board.GetLength(1); j++)
            {
                switch (board[i,j])
                {
                    case Square.WHITE:
                        line += "W";
                        break;
                    case Square.BLACK:
                        line += "B";
                        break;
                    default:
                        line += "_";
                        break;

                }
            }
            output += line;
            output += "\n";
        }

        return output;
    }
    
    
}

