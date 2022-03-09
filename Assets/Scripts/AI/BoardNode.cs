using System.Collections.Generic;
using UnityEngine;

public class BoardNode
    {
        Board board;

        public readonly bool isWhiteMove;
        public readonly Move parentMove;
        List<BoardNode> children;

        public BoardNode(Board board, bool whiteMove)
        {
            this.board = board;
            children = new List<BoardNode>();
            this.isWhiteMove = whiteMove;
            this.parentMove = null;
        }

        public BoardNode(Board board, Move move, bool whiteMove, bool undo)
        {
            this.board = new Board(board, move);
            children = new List<BoardNode>();
            if (!undo)
            {
                this.board.UpdateBoard(move);
            }
            this.isWhiteMove = whiteMove;
            this.parentMove = move;
        }

        public int GetScore() 
        {
            return board.GetScore();
        }

        public List<BoardNode> GetChildren()
        {
            return children;
        }

        public BoardNode GetChild(int index)
        {
            if (index < 0 || index >= children.Count) return null;

            return children[index];
        }

        public void AddChildren() 
        {
            foreach (Move move in board.GetAllMoves())
            {
                children.Add(new BoardNode(board, move, !isWhiteMove, false));
            }
        }

        public void DisplayBoard()
        {
            Debug.Log(board.ToString());
        }

        public BoardNode UnapplyMove(Move move, Move grandparent)
        {
            return new BoardNode(board.UndoMove(move, grandparent), grandparent, !this.isWhiteMove, true);
        }


    }