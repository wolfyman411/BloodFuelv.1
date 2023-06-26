using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class endingcutscene : MonoBehaviour
{
    public string[] lines;
    public string displayLine = "";
    public int curStr = 0;
    public int curLine = 0;
    float dialogueSpeed = 0.1f;
    float dialogueChange = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        ShopHandler.SavedTokenAmount += 200;
        PlayerPrefs.SetInt("BloodTokens", ShopHandler.SavedTokenAmount);

        updatedstats.OrionBeaten = true;
        PlayerPrefs.SetInt("OrionBeaten", 1);
        Invoke("updateDialogue", dialogueSpeed);
    }

    void updateDialogue()
    {
        if (curStr < lines[curLine].Length)
        {
            displayLine += lines[curLine][curStr];
            curStr++;
            Invoke("updateDialogue", dialogueSpeed);
        }
        else
        {
            Invoke("clearDialogue", dialogueChange);
        }
        GameObject.Find("Orion").GetComponent<Text>().text = displayLine;

    }

    void clearDialogue()
    {
        if (curLine < lines.Length-1)
        {
            curLine++;
            displayLine = "";
            curStr = 0;
            Invoke("updateDialogue", dialogueChange);
            GameObject.Find("Orion").GetComponent<Text>().text = displayLine;
        }
        else if (curLine >= lines.Length-1 && curLine > 0)
        {
            Invoke("BlackEnter", 1.0f);
        }
    }

    void BlackEnter()
    {
        GameObject.Find("BlackScreen").transform.localScale = new Vector3(100, 100, 100);
        GameObject.Find("BlackScreen").GetComponent<Image>().color = new Color(0, 0, 0, GameObject.Find("BlackScreen").GetComponent<Image>().color.a + 0.1f);
        if (GameObject.Find("BlackScreen").GetComponent<Image>().color.a >= 1)
        {
            SceneManager.LoadScene("MainMenu");
        }
        Invoke("BlackEnter", 0.1f);
    }
}
