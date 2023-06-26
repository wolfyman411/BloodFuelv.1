using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainmenu : MonoBehaviour
{
    public Vector3 target;

    public float moveDuration = 1f;

    private float elapsedTime = 0f;
    private Vector3 initialPosition;
    private Vector3 startPosition;

    public void Start()
    {
        if (PlayerPrefs.GetInt("OrionBeaten") == 1)
        {
            updatedstats.OrionBeaten = true;
        }
        GameObject.Find("ClickSoundSFX").GetComponent<AudioSource>().mute = true;
        GameObject.Find("ClickSoundVO").GetComponent<AudioSource>().mute = true;
        GameObject.Find("ClickSoundMusic").GetComponent<AudioSource>().mute = true;
        initialPosition = GameObject.Find("Mover").transform.position;
        target = initialPosition;
        startPosition = initialPosition;
        Invoke("BlackFade", 0.1f);
        if (updatedstats.OrionBeaten)
        {
            GameObject.Find("Boon").transform.localScale = new Vector2(0.09f, 0.09f);
        }
        else
        {
            GameObject.Find("Boon").transform.localScale = Vector2.zero;
        }
    }
    public void MoveShop()
    {
        startPosition = GameObject.Find("Mover").transform.position;
        elapsedTime = 0f;
        Debug.Log("Test");
        target.x = GameObject.Find("SettingsPos").transform.position.x;
    }
    public void MoveMain()
    {
        startPosition = GameObject.Find("Mover").transform.position;
        elapsedTime = 0f;
        Debug.Log("Test");
        target.x = initialPosition.x;
    }
    public void MoveSettings()
    {
        GameObject.Find("ClickSoundSFX").GetComponent<AudioSource>().mute = false;
        GameObject.Find("ClickSoundVO").GetComponent<AudioSource>().mute = false;
        GameObject.Find("ClickSoundMusic").GetComponent<AudioSource>().mute = false;
        startPosition = GameObject.Find("Mover").transform.position;
        elapsedTime = 0f;
        Debug.Log("Test");
        target.x = GameObject.Find("shopPos").transform.position.x;
    }

    public void StartGame()
    {
        Invoke("BlackEnter", 0.1f);
    }

    public void EndGame()
    {
        Application.Quit();
    }
    public void OpenYoutube()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCxEO3hqbBfoF_nJFfR8AAlg");
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        updatedstats.upgradeBloodFuel = 0;
        updatedstats.upgradeDamage = 0;
        updatedstats.upgradeDodge = 0;
        updatedstats.upgradeRevive = 0;
        updatedstats.upgradeSpeed = 0;
        updatedstats.OrionBeaten = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / moveDuration);

        GameObject.Find("Mover").transform.position = Vector3.Lerp(startPosition, target, t);
    }
    void BlackFade()
    {
        GameObject.Find("BlackScreen").GetComponent<Image>().color = new Color(0, 0, 0, GameObject.Find("BlackScreen").GetComponent<Image>().color.a - 0.1f);
        if (GameObject.Find("BlackScreen").GetComponent<Image>().color.a <= 0)
        {
            GameObject.Find("BlackScreen").transform.localScale = Vector3.zero;
        }
        else
        {
            Invoke("BlackFade", 0.1f);
        }
    }

    void BlackEnter()
    {
        GameObject.Find("BlackScreen").transform.localScale = new Vector3 (100,100, 100);
        GameObject.Find("BlackScreen").GetComponent<Image>().color = new Color(0, 0, 0, GameObject.Find("BlackScreen").GetComponent<Image>().color.a + 0.1f);
        if (GameObject.Find("BlackScreen").GetComponent<Image>().color.a >= 1)
        {
            SceneManager.LoadScene("Town");
        }
        Invoke("BlackEnter", 0.1f);
    }
}
