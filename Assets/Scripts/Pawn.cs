using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pawn : MonoBehaviour
{

    public bool enemy;
    bool turn;
    public bool isTurn {get {return turn;}}
    int direction;
    public Tilemap tilemap;

    public GameObject highlightPrefab;
    public GameObject capturePrefab;

    List<GameObject> highlights;

    List<Move> moves;

    bool clicked;

    public Sprite white;
    public Sprite black; 

    SpriteRenderer sr;

    bool flipped = false;

    public bool isFlipped {get {return flipped;}}

    new Collider2D collider;

    PawnController controller;
    // Start is called before the first frame update
    void Awake()
    {
        highlights = new List<GameObject>();
        clicked = false;
        sr = GetComponent<SpriteRenderer>();
        ColourInit();

        turn = !enemy;

        moves = new List<Move>();

        collider = GetComponent<Collider2D>();
        controller = GetComponentInParent<PawnController>();
    }

    void ColourInit()
    {
        if (enemy)
        {
            sr.sprite = black;
        }
        else
        {
            sr.sprite = white;
        }
        direction = enemy ? -1 : 1;
    }

    public Sprite GetSprite()
    {
        return sr.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        /*if (enemy) {
            Debug.Log("over a black pawn");
            return;
        }*/

        if (!turn) return;
        if (clicked) return;
        LogPosition();
        HighlightMoves(true);
        
    }

    void OnMouseExit()
    {
        if (!turn) return;
        if (!clicked) {
            //Debug.Log("mouse left pawn at " + ToChess(GetRank(), GetFile(), flipped));
            RevertHighlights();
        }
        
    }

    void HighlightMoves(bool display)
    {
        if (display)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        //if (enemy) return;

        CheckSquare(1, display);

        if (direction == 1 && GetFile() == -3
            || direction == -1 && GetFile() == 2)
        {
            CheckSquare(2, display);
        }

        CheckCaptures(display);

        //Debug.Log(direction);
        //Debug.Log(GetFile());

        if (direction == 1 && GetFile() == 0
            || direction == -1 && GetFile() == -1)
        {
            CheckEnPassant(display);
        }

    }

    void CheckSquare(int distance, bool display)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.5f, 0), Vector2.up * direction, distance, LayerMask.GetMask("Piece"));
        if (hit.collider != null)
        {
            Debug.Log("check " + distance + " square(s) away from " + ToChess(GetRank(), GetFile(), flipped) + ": collision with " + hit.collider.gameObject);
        }
        else
        {
            Vector3Int movePosition = ToPosition(GetRank(), GetFile() + distance * direction);
            Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                                    (Vector2Int) movePosition, !enemy, false, flipped);
            if (display)
            {   
                GameObject highlight = Instantiate(highlightPrefab, transform.position + Vector3.up * distance * direction, Quaternion.identity, transform);
                MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
                mh.SetMove(move);
                highlights.Add(highlight);
                LogPosition("can move to", ToChess(movePosition.x, movePosition.y, flipped)); 
            }
            else
            {
                moves.Add(move);
            }
            
        }
    }

    void CheckCaptures(bool display)
    {

        CheckCaptureHelp(-1, display);
        CheckCaptureHelp(1, display);
        
        
    }

    void CheckCaptureHelp(int dir, bool display)
    {
        Vector2 lr = dir == -1 ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.5f, 0), Vector2.up * direction + lr, 1.5f, LayerMask.GetMask("Piece"));
        if (hit.collider != null)
        {
            Debug.Log("check capture to left from " + ToChess(GetRank(), GetFile(), flipped) + ": collision with " + hit.collider.gameObject);
            Pawn other = hit.collider.gameObject.GetComponent<Pawn>();
            if (other.enemy != this.enemy)
            {
                /*Vector3Int movePosition = ToPosition(GetRank() + dir, GetFile() + 1 * direction);
                Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                            (Vector2Int) movePosition, !enemy, true, flipped);
                if (display)
                {
                    GameObject highlight = Instantiate(capturePrefab, transform.position + new Vector3Int(-1,direction, 0), Quaternion.identity, transform);
                    MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
                    mh.SetMove(move);
                    highlights.Add(highlight); 
                }
                else
                {
                    Debug.Log(move);
                    moves.Add(move);
                }*/
                ShowMove(display, false, dir);
            }
        } 
    }    

    void ShowMove(bool display, bool enPassant, int dir)
    {
        Vector3Int movePosition = ToPosition(GetRank() + dir, GetFile() + 1 * direction);
        Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                    (Vector2Int) movePosition, !enemy, true, flipped, enPassant);
        if (display)
        {
            GameObject highlight = Instantiate(capturePrefab, transform.position + new Vector3Int(dir,direction, 0), Quaternion.identity, transform);
            MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
            if (enPassant) mh.SetEnPassant();
            mh.SetMove(move);
            highlights.Add(highlight); 
        }
        else
        {
            Debug.Log(move);
            moves.Add(move);
        }
    }


    void CheckEnPassant(bool display)
    {
        //Debug.Log("checking for en passant");
        PawnController controller = GetComponentInParent<PawnController>();
        Move lastMove = controller.GetLastMove();
        if (lastMove == null) return;

        int lr;

        if (flipped != lastMove.flipped)
        {
            lr = (-1 - lastMove.from.x) - GetRank();
        }
        else
        {
            lr = lastMove.from.x - GetRank();
        }

        
        //Debug.Log("lr is " + lr);

        bool adjacent = lr == 1 || lr == -1;
        //Debug.Log("last move was: " + lastMove);
        //Debug.Log(lastMove.from - lastMove.to);
        Vector2Int displacement = lastMove.from - lastMove.to;
        bool movedTwo = (displacement == Vector2Int.down * 2) || (displacement == Vector2Int.up * 2);
        //bool movedTwo = (lastMove.from == lastMove.to + Vector2Int.up * 2 * direction);
        //Debug.Log("last move was 2 spaces? " + movedTwo);

        int lastY = (flipped != lastMove.flipped) ? -1 - lastMove.to.y : lastMove.to.y;

        if (lastY == GetFile() && adjacent && movedTwo)
        {
            /*Vector3Int movePosition = ToPosition(GetRank() + lr, GetFile() + 1 * direction);
            Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                                (Vector2Int) movePosition, !enemy, true, flipped, true);
            if (display)
                {
                    GameObject highlight = Instantiate(capturePrefab, transform.position + new Vector3Int(lr,direction, 0), Quaternion.identity, transform);
                    MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
                    mh.SetEnPassant();
                    mh.SetMove(move);
                    highlights.Add(highlight); 
                }
                else
                {
                    Debug.Log(move);
                    moves.Add(move);
                }*/
            ShowMove(display, true, lr);
        }
    }

    void RevertHighlights()
    {
        //Debug.Log("reverting highlights of pawn at " + ToChess(GetRank(), GetFile(), flipped));
        int nullHighlights = 0;
        foreach(GameObject highlight in highlights)
        {
            if (highlight == null) 
            {
                //highlights.Remove(highlight);
                nullHighlights++;
            }
            else
            {
                Vector3 position = highlight.transform.position;
                Destroy(highlight);
            }
        }
        transform.GetChild(0).gameObject.SetActive(false);
        highlights = new List<GameObject>();
    }

    public int GetRank()
    {
        return (int) (transform.position.x);
    }

    public int GetFile()
    {
        return (int) (transform.position.y);
    }

    void LogPosition()
    {
        //Debug.Log("pawn at " + ToChess(GetRank(), GetFile(), flipped));
    }

    void LogPosition(string msg, string pos)
    {
        //Debug.Log(msg + " " + pos);
    }

    public static string ToChess(int rank, int file, bool flipped)
    {
        if (flipped)
        {
            rank = -1 - rank;
            file = -1 - file;
        }
        string output = "" + (char) (97 + rank + 4) + (file + 5);
        return output;
    }

    Vector3Int ToPosition(int rank, int file)
    {
        return new Vector3Int(rank, file,0);
    }

    void OnMouseDown()
    {
        if (!turn) return;
        if (GetMoves().Count == 0) return;
        //Debug.Log("clicked a pawn");
        clicked = !clicked;
        if (!clicked) 
        {
            //Debug.Log("unclicked");
            controller.EnableOthers();
            return;
        }
        //Debug.Log("number of highlights: " + highlights.Count);
        foreach (GameObject highlight in highlights)
        {
            //TODO: not sure if we still need this
            if (highlight != null)
            {
                controller.DisableOthers();
            }
        }
        SendMessageUpwards("PawnClicked", new Vector2Int(GetRank(), GetFile()));
    }

    void OtherClicked(Vector2Int position)
    {
        if (!(GetRank() == position.x && GetFile() == position.y))
        {
            clicked = false;
            RevertHighlights();
        }
        //SendMessageUpwards("EnableOthers");
    }

    //TODO make sure that which colour a move is is registered correctly
    public void MakeMove(Move move)
    {
        //bool isCapture = (position.x != transform.position.x);
        controller.EnableOthers();
        if (move.isCapture) {
            //Debug.Log("Capture!");
            controller.Capture(move);
        }

        Vector3 prev = transform.position;

        //Vector2Int from = new Vector2Int(GetRank(), GetFile());
        transform.position = new Vector3(move.to.x, move.to.y, 0);
        //Vector2Int to = new Vector2Int(GetRank(), GetFile());
        
        

        if ((direction == 1 && GetFile() == 3)
            || (direction == -1 && GetFile() == -4))
        {
            controller.Victory();
        }

        clicked = false;
        RevertHighlights();
        controller.EndTurn(move);
        
    }

    void FlipTurn()
    {
        turn = !turn;
    }

    void DestroyIfCaptured(Vector3 position)
    {
        if (position.x == transform.position.x && position.y == transform.position.y)
        {
            Destroy();
        }
    }

    void GameOver()
    {
        //Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
    }

    void Flip()
    {
        direction = -direction;
        flipped = !flipped;
        transform.position = new Vector3(-1 - transform.position.x, -1 - transform.position.y, transform.position.z);

    }

    /*void AddToList()
    {
        SendMessageUpwards("AddPawn", enemy);
    }*/

    void DisableCollider()
    {
        if (!clicked) collider.enabled = false;
    }

    void EnableCollider()
    {
        collider.enabled = true;
    }

    public List<Move> GetMoves()
    {
        moves = new List<Move>();
        HighlightMoves(false);
        return moves;
    }

    public void UndoCapture(Pawn capturedBy)
    {
        this.flipped = capturedBy.flipped;
        Debug.Log("flipped? " + this.flipped);
        this.enemy = !capturedBy.enemy;
        ColourInit();
        if (this.flipped) direction = -direction;
        turn = true;
    }

    public void Destroy()
    {
        SendMessageUpwards("RemovePawn", enemy);
        Destroy(this.gameObject);
    }
}


//------------------- old code for CheckCaptures ----------------------------------------------------

/*RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.5f, 0), Vector2.up * direction + Vector2.left, 1.5f, LayerMask.GetMask("Piece"));
        if (hit.collider != null)
        {
            Debug.Log("check capture to left from " + ToChess(GetRank(), GetFile(), flipped) + ": collision with " + hit.collider.gameObject);
            Pawn other = hit.collider.gameObject.GetComponent<Pawn>();
            if (other.enemy != this.enemy)
            {
                Vector3Int movePosition = ToPosition(GetRank() - 1, GetFile() + 1 * direction);
                Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                            (Vector2Int) movePosition, !enemy, true, flipped);
                if (display)
                {
                    GameObject highlight = Instantiate(capturePrefab, transform.position + new Vector3Int(-1,direction, 0), Quaternion.identity, transform);
                    MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
                    mh.SetMove(move);
                    highlights.Add(highlight); 
                }
                else
                {
                    Debug.Log(move);
                    moves.Add(move);
                }
            }
        } 
        else
        {
            //Debug.Log("ok didn't hit anything");
        }

        hit = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0.5f, 0), Vector2.up * direction + Vector2.right, 1.5f, LayerMask.GetMask("Piece"));
        if (hit.collider != null)
        {
            Debug.Log("check capture to right from " + ToChess(GetRank(), GetFile(), flipped) + ": collision with " + hit.collider.gameObject);
            Pawn other = hit.collider.gameObject.GetComponent<Pawn>();
            if (other.enemy != this.enemy)
            {
                Vector3Int movePosition = ToPosition(GetRank() + 1, GetFile() + 1 * direction);
                Move move = new Move(new Vector2Int(GetRank(), GetFile()),
                            (Vector2Int) movePosition, !enemy, true, flipped);
                if (display)
                {
                    GameObject highlight = Instantiate(capturePrefab, transform.position + new Vector3Int(1,direction, 0), Quaternion.identity, transform);
                    MoveHighlight mh = highlight.GetComponent<MoveHighlight>();
                    mh.SetMove(move);
                    highlights.Add(highlight); 
                }
                else
                {
                    Debug.Log(move);
                    moves.Add(move);
                }
            }
        }*/


//--------------------------------------------------------------------------------------------------------------