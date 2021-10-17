using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading;
using System.IO;

public class PawnController : MonoBehaviour
{

    Stack<Move> moves;

    public GameObject pawnPrefab;

    AudioSource source;
    public AudioClip victory;
    public AudioClip stalemate;
    public AudioClip capture;
    public AudioClip rewind;

    int whitePawns = 0;
    int blackPawns = 0;

    new public Camera camera;

    bool gameOver = false;

    bool checkSM = false;
    public int maxWaitTime;
    int waitTime;
    bool flipped = false;

    bool vsAI;

    bool isPlayerWhite;

    char whiteGap;
    char blackGap;

    bool isWhiteTurn = true;

    Board board;


    void Start()
    {   
        moves = new Stack<Move>();
        source = GetComponent<AudioSource>();
        AddToList();
        //BroadcastMessage("AddToList");
        Debug.Log("white pawns: " + whitePawns);
        Debug.Log("black pawns: " + blackPawns);

        /*TextAsset file = (TextAsset) Resources.Load("code");
        string code = file.text;
        Debug.Log(code);
        vsAI = code[0] == '1';
        char whiteGap = code[1];
        char blackGap = code[2];
        MakeGaps(whiteGap, blackGap);*/

        MakeGaps(whiteGap, blackGap);

    }

    void OnEnable()
    {
        vsAI = PlayerPrefs.GetInt("vsAI") == 1;
        if (vsAI)
        {
            isPlayerWhite = PlayerPrefs.GetInt("isPlayerWhite") == 1;
            if (isPlayerWhite)
            {
                whiteGap = PlayerPrefs.GetString("whiteGap")[0];
                blackGap = ChooseGap();
            }
            else
            {
                blackGap = PlayerPrefs.GetString("blackGap")[0];
                whiteGap = ChooseGap();
            }
        }
        else
        {
            whiteGap = PlayerPrefs.GetString("whiteGap")[0];
            blackGap = PlayerPrefs.GetString("blackGap")[0];
        }
        
    }

    char ChooseGap()
    {
        //TODO: pick a better strategy
        //Random r = new Random();
        int random = (int) Mathf.Floor(Random.Range(0, 7));
        char gap = (char) (random + 97);
        Debug.Log("random gap at " + gap);
        return gap;
    }

    void MakeGaps(char whiteGap, char blackGap)
    {
        Pawn[] pawns = GetComponentsInChildren<Pawn>();
        Debug.Log("total number of pawns: " + pawns.Length);
        foreach(Pawn pawn in pawns)
        {
            char rank = Pawn.ToChess(pawn.GetRank(), pawn.GetFile(), pawn.isFlipped)[0];
            if (pawn.enemy)
            {
                if (rank == blackGap)
                {
                    pawn.Destroy();
                }
            }
            else
            {
                if (rank == whiteGap)
                {
                    pawn.Destroy();
                }
            
            }
        }
    }

    void AddToList()
    {
        Pawn[] pawns = GetComponentsInChildren<Pawn>();
        Debug.Log("total number of pawns now: " + pawns.Length);
        foreach(Pawn pawn in pawns)
        {
            if (pawn.enemy)
            {
                blackPawns++;
            }
            else
            {
                whitePawns++;
            }
        }
    }

    void FixedUpdate()
    {
        if (checkSM)
        {
            waitTime--;
            if (waitTime == 0)
            {
                checkSM = false;
                CheckStalemate();
                if (vsAI)
                {
                    if (isWhiteTurn != isPlayerWhite)
                    {
                        ChooseMove();
                    }
                }
            }
        }
    }

    /*void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            MoveHighlight mh = null;

            if (Physics.Raycast(ray, out hit))
            {
                Transform highlight = hit.transform;
                mh = highlight.GetComponent<MoveHighlight>();
            }

            if (mh != null)
            {
                mh.click();
            } 
            else
            {
                BroadcastMessage("");
            }
            //raycast to screen on highlight layer
            //
            //Debug.Log("Mouse 0 ");
        }
    }*/

    void CheckStalemate()
    {
        List<Move> possMoves = GetAllMoves();
        StringBuilder allMoves = new StringBuilder();
        foreach(Move possMove in possMoves)
        {
            allMoves.Append("\n");
            allMoves.Append(possMove);
        }
        Debug.Log("all possible moves:" + allMoves.ToString());
        if (possMoves.Count == 0 && !gameOver)
        {
            gameOver = true;
            Stalemate();
        }
    }

    List<Move> GetAllMoves()
    {
        List<Move> moves = new List<Move>();
        Pawn[] pawns = GetComponentsInChildren<Pawn>();
        GetPawnPositions();
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(-2.5f,2.5f), Vector2.down, 2f);
        Debug.Log("raycast went " + hit.distance + " units");
        if (hit.collider != null)
        {
            Pawn pawn = hit.collider.GetComponent<Pawn>();
            Debug.Log("raycast hit pawn at " + Pawn.ToChess(pawn.GetRank(), pawn.GetFile(), pawn.isFlipped));
        } 
        else
        {
            Debug.Log("what the actual heck");
        }
        //TODO try a raycast here from b7, check position of pawn it hits, then cry
        foreach(Pawn pawn in pawns)
        {
            if (pawn.isTurn)
            {
                moves.AddRange(pawn.GetMoves());
            }
        }
        
        return moves;
    }

    void GetPawnPositions()
    {
        Pawn[] pawns = GetComponentsInChildren<Pawn>();
        StringBuilder sb = new StringBuilder("enemy pawns are at:");
        foreach(Pawn pawn in pawns)
        {
            if (!pawn.isTurn)
            {
                sb.Append("\n");
                sb.Append(Pawn.ToChess(pawn.GetRank(), pawn.GetFile(), pawn.isFlipped));
            }
        }
        Debug.Log(sb.ToString());
    }

    void OnMouseDown()
    {
        //Debug.Log("clicked the board");
        BroadcastMessage("OtherClicked", new Vector2Int(-10,-10));
    }

    void PawnClicked(Vector2Int position)
    {
        BroadcastMessage("OtherClicked", position);
    }

    public void EndTurn(Move move)
    {
        AddMove(move);

        if (gameOver) return;
        
        BroadcastMessage("FlipTurn");
        
        checkSM = true;
        waitTime = maxWaitTime; 
    }

    void ChooseMove()
    {
        List<Move> moves = GetAllMoves();
        int randomMove = (int) Mathf.Floor(Random.Range(0, moves.Count));
        Move move = moves[randomMove];

        Pawn pawn = GetPawnAt(move.from);
        pawn.MakeMove(move);
    }

    void FlipTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        Debug.Log("It is now " + (isWhiteTurn ? "white" : "black") + "'s turn");
    }

    //TODO: something is going wrong and basically when all moves are got it ignores the move that just happened
    public void AddMove(Move move)
    {
        //Debug.Log(move);
        moves.Push(move);
        Debug.Log("move that just happened: " + move);

    }

    public void Capture(Move move) 
    {
        Vector3 position = new Vector3(move.to.x, move.to.y, 0);
        if (move.enPassant)
        {
            position = new Vector3(move.to.x, move.from.y, 0);
        }
        //Debug.Log("destroying pawn at " + position);
        source.PlayOneShot(capture);
        BroadcastMessage("DestroyIfCaptured", position);
    }

    public void Victory()
    {
        gameOver = true;
        source.PlayOneShot(victory);
        BroadcastMessage("GameOver");
    }

    void Stalemate()
    {
        source.PlayOneShot(stalemate);
        BroadcastMessage("GameOver");
    }

    void AddPawn(bool enemy)
    {
        if (enemy)
        {
            blackPawns++;
        }
        else
        {
            whitePawns++;
        }
    }

    void RemovePawn(bool enemy)
    {
        //Debug.Log("pawn removed");
        if (enemy)
        {
            blackPawns--;
        }
        else
        {
            whitePawns--;
        }

        if ((whitePawns == 0 || blackPawns == 0) && !gameOver)
        {
            gameOver = true;
            Victory();
        }
    }

    public void DisableOthers()
    {
        BroadcastMessage("DisableCollider");
    }

    public void EnableOthers()
    {
        BroadcastMessage("EnableCollider");
    }

    public Move GetLastMove()
    {
        if (moves.Count == 0)
        {
            return null;
        }
        return moves.Peek();
    }

    /*
    ideas for improvements: 
    
    - store moves as a (linked?) list, not stack
    - on undo, display all of them and have a pointer at the end of the list
    - when a move highlighted:
        - set the pointer to the position of that move
        - unapply all moves that took place after it (and reapply any that took place before it)
    - remove from list all moves after the one selected

    challenges: how to actually display them (going for a divine pulse kinda style here but not 100% sure)
    */
    public void UndoMove()
    {
        if (moves.Count == 0) return;
        Debug.Log("move made");
        if (gameOver) return;
        Debug.Log("it's not over");
        Move move = moves.Pop();

        if (move.flipped != flipped)
        {
            move = move.FlipPosition();
        }
        //TODO: GetPawnAt doesn't work with a flipped board (and I assume neither does the actual moving of pawns)
        Pawn pawn = GetPawnAt(move.to);
        if (pawn != null)
        {
            //if (!source.isPlaying) //this works but also doesn't play during a capture
            source.PlayOneShot(rewind);
            /*if (flipped)
            {
                pawn.transform.position = new Vector3(-1 - move.from.x, -1 - move.from.y, 0);
            }
            else
            {*/
                pawn.transform.position = new Vector3(move.from.x, move.from.y, 0);
            //}
            if (move.isCapture)
            {
                GameObject captured = Instantiate(pawnPrefab, transform);
                Pawn capturedScript = captured.GetComponent<Pawn>();
                if (!move.enPassant)
                {
                    /*if (flipped)
                    {
                        captured.transform.position = new Vector3(-1 - move.to.x, -1 - move.to.y, 0);
                    }
                    else
                    {*/
                        captured.transform.position = new Vector3(move.to.x, move.to.y, 0);
                    //}
                    
                }
                else
                {
                    /*if (flipped)
                    {
                        captured.transform.position = new Vector3(-1 - move.to.x, -1 - move.from.y, 0);
                    }
                    else
                    {*/
                        captured.transform.position = new Vector3(move.to.x, move.from.y, 0);
                    //}
                    
                    
                }
                capturedScript.UndoCapture(pawn);
                //idk what to do here yet
                AddPawn(capturedScript.enemy);
            }
            BroadcastMessage("FlipTurn");
        }
    }

    Pawn GetPawnAt(Vector2Int position)
    {
        Pawn[] pawns = GetComponentsInChildren<Pawn>();
        /*if (flipped)
        {
            position = new Vector2Int(-1 - position.x, -1 - position.y);
        }*/
        foreach (Pawn pawn in pawns)
        {
            if (new Vector2Int(pawn.GetRank(), pawn.GetFile()) == position) return pawn;
        }
        return null;
    }

    void Flip()
    {
        flipped = !flipped;
    }

    public void Quit()
    {
        Debug.Log("quitting");
        Application.Quit();
    }
}
