using System.Collections.Generic;
using UnityEngine;

public class BoardTree
{

    BoardNode root;
    int maxDepth;

    public BoardTree(Board start, int maxDepth)
    {
        root = new BoardNode(start, true);
        Debug.Log("starting board has score " + root.GetScore());
        this.maxDepth = maxDepth;
        Debug.Log("initialising tree");
        InitialiseTree();
    }

    public Move SelectMove() 
    {
        int index = minimax();
        Debug.Log("child at index " + index + " selected");
        BoardNode optimalChild = root.GetChild(index);
        if (optimalChild.parentMove.enPassant)
        {
            Debug.Log("this move is en passant!");
        }
        return optimalChild.parentMove;
    }

    public void UpdateTree(BoardNode newRoot)
    {
        this.root = newRoot;
        Debug.Log("Current board is now:\n");
        root.DisplayBoard();
        Debug.Log("board has score " + root.GetScore());
        PopulateTree(newRoot, 0, this.maxDepth);

    }

    public void ApplyMove(Move move)
    {
        foreach (BoardNode child in root.GetChildren())
        {
            //Debug.Log("move that led to this child:" + child.parentMove.ToString());
            if (child.parentMove.Equals(move))
            {
                Debug.Log("found the right child for move " + move.ToString());
                UpdateTree(child);
            }
        }
    }

    public void UndoMove(Move grandparent)
    {
        if (root.parentMove == null)
        {
            Debug.Log("should be back at the start");
            return; 
        } 
        BoardNode parent = root.UnapplyMove(root.parentMove, grandparent);
        UpdateTree(parent);

    }

    public void InitialiseTree()
    {
        PopulateTree(root, 0, this.maxDepth);
    }

    void PopulateTree(BoardNode start, int depth, int maxDepth)
    {
        if (depth < maxDepth)
        {
            if (start.GetChildren().Count == 0) //TODO: make sure not game over
            {
                //Debug.Log("Adding children");
                start.AddChildren();
            }

            foreach (BoardNode child in start.GetChildren())
            {
                PopulateTree(child, depth + 1, maxDepth);
            }
        }
    }

    //TODO: check this (might need to be !root.isWhiteMove)
    public int minimax() 
    {
        return minimax(root, root.isWhiteMove, 0);
    }

    public int minimax(BoardNode start, bool max, int depth) 
    {

        if (start.GetChildren().Count == 0) {
            return start.GetScore();
        }

        List<int> scores = new List<int>();

        foreach(BoardNode node in start.GetChildren()) 
        {
            scores.Add(minimax(node, !max, depth + 1));
        }

        //Debug.Log("Number of children: " + scores.Count);

        int result = max ? GetMaxScore(scores) : GetMinScore(scores);

        if (depth == 0) 
        {
            //Debug.Log("chosen move leads to score of " + result);
            return scores.IndexOf(result);
        }
        else
        {
            return result;
        }
    }

    int GetMaxScore(List<int> scores) 
    {
        int max = int.MinValue;
        foreach (int i in scores)
        {
            if (i > max) max = i;
        }
        return max;
    }

    int GetMinScore(List<int> scores) 
    {
        int min = int.MaxValue;
        foreach (int i in scores)
        {
            if (i < min) min = i;
        }
        return min;
    }

    public void DisplayRoot()
    {
        root.DisplayBoard();
    }


    

}