using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void SetSprite(Sprite sprite)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
