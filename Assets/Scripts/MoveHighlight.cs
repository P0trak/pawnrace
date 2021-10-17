using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHighlight : MonoBehaviour
{
    
    SpriteRenderer spriteRenderer;

    //public GameObject pawnImagePrefab;

    public Sprite normalMove;
    public Sprite captureMove;

    //GameObject pawnImage;

    bool ep = false;
    public bool enPassant {get {return ep;} }

    Move move;

    PawnImage image;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponentInChildren<PawnImage>();
        Pawn pawn = GetComponentInParent<Pawn>();
        image.SetSprite(pawn.GetSprite());
        image.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        image.gameObject.SetActive(true);
    }
    void OnMouseDown()
    {
        Debug.Log("moving to " + transform.position);
        SendMessageUpwards("MakeMove", move);
    }

    public void click()
    {
        Debug.Log("moving to " + transform.position);
        SendMessageUpwards("MakeMove", move);
    }

    void OnMouseExit()
    {
        image.gameObject.SetActive(false);
    }

    public void SetEnPassant()
    {
        ep = true;
    }

    public Move GetMove()
    {
        return move;
    }

    public void SetMove(Move newMove)
    {
        move = newMove;
        if (newMove.isCapture)
        {
            spriteRenderer.sprite = captureMove;
        }
        else
        {
            spriteRenderer.sprite = normalMove;
        }
    }

    
}