using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAxis : MonoBehaviour
{
    public bool xAxis;

    string x = "A B C D E F G H";
    string xFlip = "H G F E D C B A";

    string y = "8\n7\n6\n5\n4\n3\n2\n1";

    string yFlip = "1\n2\n3\n4\n5\n6\n7\n8";

    bool flipped = false;

    public void Flip()
    {
        flipped = !flipped;

        TextMeshPro text = GetComponent<TextMeshPro>();

        if (text == null)
        Debug.Log("nani");

        if (xAxis)
        {
            text.text = flipped ? xFlip : x;
        }
        else 
        {
            text.text = flipped ? yFlip : y;
        }

    }
}
