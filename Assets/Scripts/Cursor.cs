using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cursor : MonoBehaviour
{
    Vector2 startPosition;
    Rigidbody2D rb2d;
    public double amplitude;
    public double frequency;

    int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startPosition = new Vector2(transform.parent.position.x, transform.parent.position.y) + Vector2.up;
    }

    void FixedUpdate()
    {
        startPosition = new Vector2(transform.parent.position.x, transform.parent.position.y) + Vector2.up;
        Vector2 newPosition = startPosition + new Vector2(0, (float) (amplitude * Math.Sin(frequency * Time.frameCount)));
        rb2d.MovePosition(newPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
