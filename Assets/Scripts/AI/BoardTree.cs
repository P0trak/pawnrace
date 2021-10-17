using System.Collections.Generic;

public class BoardTree
{

    BoardNode root;

    public BoardTree(Board start)
    {
        root = new BoardNode(start);
    }

    class BoardNode
    {
        Board board;

        List<BoardNode> children;

        public BoardNode(Board board)
        {
            this.board = board;
            children = new List<BoardNode>();
        }
    }

}