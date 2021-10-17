public class Board
{
    Square[,] board;

    // this will be tweaked (and probably won't end up being linear) 
    int POSITION_FACTOR = 1;

    public Board(char whiteGap, char blackGap)
    {
        board = new Square[8, 8];
        for (int i = 0; i < board.Length; i++)
        {
            board[1, i] = Square.WHITE;
            board[6, i] = Square.BLACK;
        }
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
                    total += (j - 1) * POSITION_FACTOR;
                }
                else if (board[i, j] == Square.BLACK)
                {
                    total += (j - 6) * POSITION_FACTOR;
                }

            }
        }
        return total;
    }


    public void UpdateBoard(Move move, bool whiteMove)
    {
        /*
         - transform move.from and move.to
         - make move.from empty
         - make move.to whiteMove ? Square.WHITE : Square.BLACK
         - if enPassant then make (move.from.y, move.to.x) empty
        */
    }
    
    
}

