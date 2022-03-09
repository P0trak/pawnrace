using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Move
{
    public readonly Vector2Int from;
    public readonly Vector2Int to;
    public readonly bool whiteMove;
    public readonly bool isCapture;
    public readonly bool flipped;
    public readonly bool enPassant;

    public Move(Vector2Int from, Vector2Int to, bool whiteMove, bool isCapture, bool flipped){
        this.from = from;
        this.to = to;
        this.whiteMove = whiteMove;
        this.isCapture = isCapture;
        this.flipped = flipped;
        this.enPassant = false;
    }

    public Move(Vector2Int from, Vector2Int to, bool whiteMove, bool isCapture, bool flipped, bool enPassant){
        this.from = from;
        this.to = to;
        this.whiteMove = whiteMove;
        this.isCapture = isCapture;
        this.flipped = flipped;
        this.enPassant = enPassant;
    }


    public override string ToString() {
        StringBuilder output = new StringBuilder();
        output.Append(whiteMove ? "white" : "black");
        output.Append(" moves ");
        output.Append(Pawn.ToChess(from.x, from.y, flipped));
        output.Append(isCapture ? "x" : "-");
        output.Append(Pawn.ToChess(to.x, to.y, flipped));
        return output.ToString();
    }

    public override bool Equals(object obj)
    {
        if (!Object.ReferenceEquals(obj.GetType(), this.GetType()))
        {
            return false;
        }
        Move other = (Move) obj;
        if (this.flipped != other.flipped)
        {
            return FlipPosition().Equals(other);
        }
        return other.from.Equals(this.from) && other.to.Equals(this.to)
        && other.whiteMove == this.whiteMove && other.isCapture == this.isCapture && other.enPassant == this.enPassant;
    }

    public override int GetHashCode()
    {
        if (flipped) return FlipPosition().GetHashCode();
        return from.GetHashCode();
    }

    public Move FlipPosition()
    {
        Move move = new Move(new Vector2Int(-1 - from.x, -1 - from.y),
                             new Vector2Int(-1 - to.x, -1 - to.y),
                             whiteMove,
                             isCapture,
                             !flipped,
                             enPassant);
        return move;
    }

}
