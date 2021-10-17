using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMenu : MonoBehaviour
{

    public GameObject whiteGaps;
    public GameObject blackGaps;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsPlayerWhite(bool isWhite)
    {
        if (isWhite)
        {
            //disable both, set text if doing that
            //make white gap group appear
        }
        else
        {
            //the same, but black group
        }
    }

    public void GapSelected(string gap)
    {
        //record gap, then enable go button
    }
}
