using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIMenu : MonoBehaviour
{

    string plural = "parallel universes ahead of you.";
    string singular = "parallel universe ahead of you.";

    public GameObject whiteGaps;
    public GameObject blackGaps;

    public TextMeshProUGUI difficulty;

    public TextMeshProUGUI pus;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDifficulty(int maxDepth)
    {
        difficulty.text = maxDepth.ToString();
        pus.text = maxDepth == 1 ? singular : plural;
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
