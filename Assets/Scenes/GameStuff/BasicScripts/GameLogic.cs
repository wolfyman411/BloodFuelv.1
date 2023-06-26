using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject currentTarget;
    public int scoreNum = 0;
    public List<GameObject> deadEnemies;
    public GameObject bossEnemy;
    public bool disableAI;
    public int DeathTokens;
    int curTokens;
    int subtractedPoints;
    bool alreadyAdded;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Score").GetComponent<Text>().color = new Color (1,1,1,0);
        GameObject.Find("BossHPBar").transform.localScale = Vector3.zero;
        Invoke("BloodDecay", playerController.bleedOutRate);
        InvokeRepeating("UpdateWaveHud", 1f, 1f);
        InvokeRepeating("UpdateBossHud", 0.1f, 0.1f);
        InvokeRepeating("deleteCorpses", 0.2f, 0.2f);
        GameObject.Find("PlayerMimic").transform.localScale = Vector3.zero;
        Invoke("BlackScreenEnter", 0.1f);
    }

    void BlackScreenEnter()
    {
        GameObject.Find("GameoverBackground").transform.localScale = new Vector3(100, 100, 100);
        GameObject.Find("GameoverBackground").GetComponent<Image>().color = new Color(0, 0, 0, GameObject.Find("GameoverBackground").GetComponent<Image>().color.a - 0.1f);
        GameObject.Find("Score").GetComponent<Text>().color = new Color(1, 1, 1, GameObject.Find("Score").GetComponent<Text>().color.a + 0.1f);
        if (GameObject.Find("GameoverBackground").GetComponent<Image>().color.a <= 0)
        {
            GameObject.Find("GameoverBackground").transform.localScale = Vector3.zero;
        }
        else
        {
            Invoke("BlackScreenEnter", 0.1f);
        }
    }

    private void Update()
    {
        if (disableAI)
        {
            playerController.speed = 0;
            GameObject.Find("GameMusic").GetComponent<AudioSource>().Stop();
            GameObject.Find("GameMusicDrums").GetComponent<AudioSource>().Stop();
            GameObject.Find("OrionMarch").GetComponent<AudioSource>().Stop();
            GameObject.Find("OrionChaos").GetComponent<AudioSource>().Stop();
            GameObject.Find("OrionMarchDrums").GetComponent<AudioSource>().Stop();
            GameObject.Find("OrionChaosDrums").GetComponent<AudioSource>().Stop();
            GameObject.Find("BossAnnounce").GetComponent<AudioSource>().Stop();
        }
        GameObject.Find("PlayerMimic").GetComponent<Image>().sprite = GameObject.Find("Player").GetComponent<SpriteRenderer>().sprite;
    }

    void deleteCorpses()
    {
        if (deadEnemies.Count > 20)
        {
            if (deadEnemies[0])
            {
                deadEnemies[0].GetComponent<EnemyHandler>().StartCoroutine(deadEnemies[0].GetComponent<EnemyHandler>().DecayHide());
            }
            deadEnemies.RemoveAt(0);
        }
    }

    void UpdateWaveHud()
    {
        GameObject.Find("Wave").GetComponent<Text>().text = GetComponent<HordeHandler>().curWave.ToString();
        GameObject.Find("WavePos").GetComponent<Text>().text = GetComponent<HordeHandler>().wavePos.ToString();
        GameObject.Find("WaveTimer").GetComponent<Text>().text = Convert.ToInt32(GetComponent<HordeHandler>().waveTimer).ToString();
        GameObject.Find("Combo").GetComponent<Text>().text = GameObject.Find("Player").GetComponent<PlayerController>().playerCombo.ToString();
    }

    void UpdateBossHud()
    {
        if (bossEnemy && bossEnemy.GetComponent<EnemyHandler>().HP > 0.0f)
        {
            GameObject.Find("BossHPBar").transform.localScale = Vector3.one;
            GameObject.Find("BossHPBar").GetComponent<Slider>().maxValue = bossEnemy.GetComponent<EnemyHandler>().HPCap;
            GameObject.Find("BossHPBar").GetComponent<Slider>().value = bossEnemy.GetComponent<EnemyHandler>().HP;
        }
        else
        {
            GameObject.Find("BossHPBar").transform.localScale = Vector3.zero;
        }
    }
    // Bleedout
    void BloodDecay()
    {
        if (playerController.bleedOutRate <= 0.1f)
        {
            playerController.bleedOutRate = 0.1f;
        }
        playerController.removeBloodFuel(1);

        Invoke("BloodDecay", playerController.bleedOutRate);
    }

    public void UpdateTargetting()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyFollow>())
            {
                enemy.GetComponent<EnemyFollow>().SetTarget();
            }
        }
    }

    public void addScore(int addVal)
    {
        if (scoreNum <= 0)
        {
            scoreNum = 0;
        }
        Text score = GameObject.Find("Score").GetComponent<Text>();
        scoreNum += addVal;
        score.text = scoreNum.ToString().PadLeft(9, '0');
    }

    public IEnumerator gameOverEvent()
    {
        DeathTokens = scoreNum / 3000;
        if (alreadyAdded == false)
        {
            ShopHandler.SavedTokenAmount += DeathTokens;
            PlayerPrefs.SetInt("BloodTokens", ShopHandler.SavedTokenAmount);
            alreadyAdded = true;
        }

        playerController.cardM1 = null;
        playerController.cardM2 = null;
        playerController.cardShift = null;
        playerController.cardSpace = null;
        playerController.cardCtrl = null;

        GameObject.Find("GameOverMusic").GetComponent<AudioSource>().Play();
        disableAI = true;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyFollow>())
            {
                enemy.GetComponent<EnemyFollow>().ClearTarget();
            }
        }
        playerController.speed = 0;
        yield return new WaitForSeconds(2.0f);
        GameObject.Find("Player").GetComponent<SpriteRenderer>().sortingOrder = 0;

        GameObject.Find("GameoverBackground").transform.localScale = Vector3.one;
        GameObject.Find("PlayerMimic").transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        GetComponent<CardDisplayHandler>().UpdateCards();
        InvokeRepeating("showGameOver", 0.01f, 0.01f);
        calculateTokens();
    }

    IEnumerator GameOverMainMenu()
    {
        yield return new WaitForSeconds(5.0f);
        InvokeRepeating("hidePlayer", 0.05f, 0.05f);
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("MainMenu");
    }

    void calculateTokens()
    {
        if (scoreNum <= 0)
        {
            Debug.Log(DeathTokens);
            scoreNum = 0;
            addScore(0);
            StartCoroutine(GameOverMainMenu());
            GameObject.Find("TokenAmount").GetComponent<Text>().text = DeathTokens.ToString() + "x";
        }
        else
        {
            addScore(-777);
            subtractedPoints += 777;
            if (subtractedPoints >= 3000)
            {
                subtractedPoints -= 3000;
                addToken();
            }
            Invoke("calculateTokens", 0.05f);
        }
    }
    void addToken()
    {
        curTokens++;
        GameObject.Find("TokenAmount").GetComponent<Text>().text = curTokens.ToString()+"x";
    }
    void showGameOver()
    {
        if (GameObject.Find("Player").GetComponent<SpriteRenderer>().flipX)
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(-1.4f, 1.4f, 1.4f);
        }
        else
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
        GameObject.Find("GameoverBackground").GetComponent<Image>().color = new Color(0, 0, 0, GameObject.Find("GameoverBackground").GetComponent<Image>().color.a + 0.01f);
        if (GameObject.Find("GameoverBackground").GetComponent<Image>().color.a < 1.0f)
        {
            GameObject.Find("DeathToken").GetComponent<Image>().color = new Color(1, 1, 1, GameObject.Find("DeathToken").GetComponent<Image>().color.a + 0.01f);
            GameObject.Find("TokenAmount").GetComponent<Text>().color = new Color(1, 1, 1, GameObject.Find("TokenAmount").GetComponent<Text>().color.a + 0.01f);
        }
    }

    void hidePlayer()
    {
        if (GameObject.Find("Player").GetComponent<SpriteRenderer>().flipX)
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(1, 1, 1);
        }
        GameObject.Find("Score").GetComponent<Text>().color = new Color(1, 1, 1, GameObject.Find("Score").GetComponent<Text>().color.a - 0.01f);
        GameObject.Find("PlayerMimic").GetComponent<Image>().color = new Color(1, 1, 1, GameObject.Find("PlayerMimic").GetComponent<Image>().color.a - 0.01f);
        GameObject.Find("DeathToken").GetComponent<Image>().color = new Color(1, 1, 1, GameObject.Find("DeathToken").GetComponent<Image>().color.a - 0.01f);
        GameObject.Find("TokenAmount").GetComponent<Text>().color = new Color(1, 1, 1, GameObject.Find("TokenAmount").GetComponent<Text>().color.a - 0.01f);
    }
}
