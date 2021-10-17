using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    bool vsAI;

    string whiteGap = "";

    string blackGap = "";

    bool isPlayerWhite;

    public GameObject goButton;

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
        PlayerPrefs.Save();
    }

    public void Quit()
    {
        Debug.Log("quitting");
        Application.Quit();
    }
}
