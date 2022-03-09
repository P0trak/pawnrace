using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    bool vsAI;

    string whiteGap = "";

    string blackGap = "";

    bool isPlayerWhite;

    int maxTreeDepth = 3;

    string plural = "parallel universes ahead of you.";
    string singular = "parallel universe ahead of you.";

    public TextMeshProUGUI difficulty;

    public TextMeshProUGUI pus;

    public GameObject goButton;

    public Button easier;
    public Button harder;

    void Awake()
    {
        Debug.Log(maxTreeDepth);
        easier.onClick.AddListener(DecreaseDifficulty);
        harder.onClick.AddListener(IncreaseDifficulty);
        difficulty.text = maxTreeDepth.ToString();
    }

    public void setAgainstAI(bool vsAI)
    {
        this.vsAI = vsAI;
    }

    public void whiteClicked(string newFile)
    {
        whiteGap = newFile;
        enableWhenReady();
    }

    public void blackClicked(string newFile)
    {
        blackGap = newFile;
        enableWhenReady();
    }

    public void IsPlayerWhite(bool isWhite)
    {
        isPlayerWhite = isWhite;
    }

    public void ReturnFromFriend()
    {
        whiteGap = "";
        blackGap = "";
        goButton.SetActive(false);
    }

    public void ReturnFromAI()
    {
        whiteGap = "";
        blackGap = "";
        goButton.SetActive(false);
    }

    public void IncreaseDifficulty()
    {
        maxTreeDepth++;
        Debug.Log(maxTreeDepth);
        if (maxTreeDepth >= 5)
        {
            harder.interactable = false;
        }
        if (maxTreeDepth > 1)
        {
           easier.interactable = true; 
        }

        SetDiffText();
        
    }

    public void DecreaseDifficulty()
    {
        maxTreeDepth--;
        Debug.Log(maxTreeDepth);
        if (maxTreeDepth <= 1)
        {
            easier.interactable = false;
        }
        if (maxTreeDepth < 5)
        {
            harder.interactable = true;
        }

        SetDiffText();
    }

    void SetDiffText()
    {
        difficulty.text = maxTreeDepth.ToString();
        pus.text = maxTreeDepth == 1 ? singular : plural;
    }


    void enableWhenReady()
    {
        if (vsAI)
        {
            if ((isPlayerWhite && !whiteGap.Equals("")) || (!isPlayerWhite && !blackGap.Equals("")))
            {
                goButton.SetActive(true);
            }
        }
        else
        {
            if (!(whiteGap.Equals("") || blackGap.Equals("")))
            {
                goButton.SetActive(true);
            }
        }
        
    }

    public void Begin()
    {
        /*string code = (vsAI ? "1" : "0") + whiteGap + blackGap;
        string path = "Assets/Resources/code.txt";
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(code);
        writer.Close();

        AssetDatabase.Refresh();        */

        PlayerPrefs.SetInt("vsAI", vsAI ? 1 : 0);
        PlayerPrefs.SetString("whiteGap", whiteGap);
        PlayerPrefs.SetString("blackGap", blackGap);

        if (vsAI)
        {
            PlayerPrefs.SetInt("isPlayerWhite", isPlayerWhite ? 1 : 0);
            PlayerPrefs.SetInt("maxTreeDepth", maxTreeDepth);
        }
        
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }

    /*void TestRead(string path)
    {
        AssetDatabase.ImportAsset(path);
        TextAsset text = (TextAsset) Resources.Load("code");
        Debug.Log(text.text);
    }*/

    void OnDisable()
    {
        PlayerPrefs.SetInt("vsAI", vsAI ? 1 : 0);
        PlayerPrefs.SetString("whiteGap", whiteGap);
        PlayerPrefs.SetString("blackGap", blackGap);
        PlayerPrefs.SetInt("maxTreeDepth", maxTreeDepth);
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        Debug.Log("quitting");
        Application.Quit();
    }
}
