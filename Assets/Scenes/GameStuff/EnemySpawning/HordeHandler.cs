using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HordeHandler : MonoBehaviour
{
    public PlayerController playerController;
    //Horde Info
    public int curWave = 1;
    public float wavePos = 0;
    public float waveSpeed;
    public float alteredWaveSpeed;
    public bool continueWave = true;
    public int aliveEnemies;
    public float waveTimer;
    public float eliteChance;
    public string[] eliteTypes;
    public int maximumEnemyCount = 10;
    bool bossAnnounced = false;

    //Enemy Info
    public int aliveBasicEnemies;
    public int aliveEliteEnemies;
    public int aliveBossEnemies;
    public bool enemyKilled = false;

    //Availible Enemies
    public GameObject[] SpawnableEnemies;
    public GameObject[] SpawnableBosses;

    //Spawner Locations
    public List<GameObject> Spawners; 

    // Start is called before the first frame update
    void Start()
    {
        waveHandler();
        alteredWaveSpeed = waveSpeed;

        InvokeRepeating("WaveChecker", 1.0f, 1.0f);
        InvokeRepeating("CheckEnemyNumbers", 5.0f, 5.0f);
    }
    private void Update()
    {
        Spawners.Clear();
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            Spawners.Add(spawner);
        }
    }
    void CheckEnemyNumbers()
    {
        aliveBasicEnemies = 0;
        aliveEliteEnemies = 0;
        aliveBossEnemies = 0;
        aliveEnemies = 0;

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy.GetComponent<EnemyHandler>() && enemy.GetComponent<EnemyHandler>().HP > 0.0f)
            {
                aliveEnemies++;
                if (enemy.GetComponent<EnemyHandler>().eliteClass == "" && enemy.GetComponent<EnemyHandler>().bossClass == "")
                {
                    aliveBasicEnemies++;
                }
                else if (enemy.GetComponent<EnemyHandler>().bossClass == "")
                {
                    aliveEliteEnemies++;
                }
                else
                {
                    aliveBossEnemies++;
                }
            }
        }
        Debug.Log("Basic enemies: " + aliveBasicEnemies + "Elite enemies: " + aliveEliteEnemies + "Boss enemies: " + aliveBossEnemies + "Total enemies: " + aliveEnemies);
    }
    public void WaveChecker()
    {
        //Wave Timer
        waveTimer -= 1.0f;

        if (waveTimer < 0.0f && curWave > 0)
        {
            if (wavePos != 3)
            {
                wavePos++;
                GameObject.Find("GameHandler").GetComponent<VisualHud>().StartCoroutine(GameObject.Find("GameHandler").GetComponent<VisualHud>().MoveObjectDown());
            }
            waveHandler();
        }
        Invoke("SpawnEnemy", alteredWaveSpeed);

        //Sanity Checker
        if (aliveBasicEnemies < 0)
        {
            aliveBasicEnemies = 0;
        }
        if (aliveEliteEnemies < 0)
        {
            aliveEliteEnemies = 0;
        }
        if (aliveBossEnemies < 0)
        {
            aliveBossEnemies = 0;
        }
        if (aliveEnemies < 0)
        {
            aliveEnemies = 0;
        }
    }

    public void NextWave()
    {
        if (curWave < 10)
        {
            GameObject.Find("GameHandler").GetComponent<VisualHud>().StartCoroutine(GameObject.Find("GameHandler").GetComponent<VisualHud>().MoveWave());
            wavePos = -1;
            curWave++;
            continueWave = true;
        }
    }

    public void SpawnEnemy()
    {
        //Enemy Weights
        int randomNum = Random.Range(0,100);
        int selectedEnemy = 0;
        if (randomNum > 0 && randomNum < 25)
        {
            selectedEnemy = 0;
        }
        else if (randomNum > 25 && randomNum < 50)
        {
            selectedEnemy = 1;
        }
        else if(randomNum > 50 && randomNum < 60)
        {
            selectedEnemy = 2;
        }
        else if(randomNum > 60 && randomNum < 80)
        {
            selectedEnemy = 3;
        }
        else if (randomNum > 80 && randomNum < 100)
        {
            selectedEnemy = 4;
        }
        Debug.Log(selectedEnemy);

        if (maximumEnemyCount > aliveEnemies)
        {
            float randomChance = Random.Range(1.0f, 100.0f);
            if (randomChance < eliteChance && maximumEnemyCount > aliveEnemies)
            {
                GameObject enemy = Instantiate(SpawnableEnemies[selectedEnemy]);
                enemy.GetComponent<EnemyHandler>().makeElite(eliteTypes[Random.Range(0, eliteTypes.Length)]);
                //enemy.GetComponent<EnemyHandler>().makeElite("Blessed");
                enemy.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
                aliveEliteEnemies++;
                aliveEnemies++;
                Invoke("SpawnEnemy", alteredWaveSpeed);
            }
            else if (maximumEnemyCount > aliveEnemies)
            {
                GameObject enemy = Instantiate(SpawnableEnemies[selectedEnemy]);
                enemy.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
                aliveEnemies++;
                aliveBasicEnemies++;
                Invoke("SpawnEnemy", alteredWaveSpeed);
            }
        }
    }
    public void SpawnBoss()
    {
        continueWave = false;
        int randomBoss = Random.Range(1, SpawnableBosses.Length);
        if (curWave == 10)
        {
            randomBoss = 0;
            GameObject.Find("GameHandler").GetComponent<MusicHandler>().isWave10 = true;
        }
        GameObject enemy = Instantiate(SpawnableBosses[randomBoss]);
        if (curWave == 3 || curWave == 6 || curWave == 9)
        {
            enemy.GetComponent<EnemyHandler>().HP *= 0.5f;
            enemy.GetComponent<EnemyHandler>().makeElite(eliteTypes[Random.Range(0, eliteTypes.Length)]);
        }
        enemy.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
        enemy.GetComponent<EnemyHandler>().HP *= curWave;
        GameObject.Find("GameHandler").GetComponent<GameLogic>().bossEnemy = enemy;
        GameObject.Find("GameHandler").GetComponent<MusicHandler>().ChangeMusic(randomBoss+1);
    }
    public void removeEnemy(string isElite)
    {
        if (isElite == "")
        {
            aliveBasicEnemies--;
        }
        else
        {
            aliveEliteEnemies--;
        }
        aliveEnemies--;
        enemyKilled = true;
        Invoke("enemyKilledFlip", 0.01f);
    }
    public void enemyKilledFlip()
    {
        enemyKilled = false;
    }
    void waveHandler()
    {
        if (continueWave == true)
        {
            if (wavePos == 0)
            {
                waveTimer = 21-curWave;
                alteredWaveSpeed = 5.0f;
                eliteChance = 0.0f;
                maximumEnemyCount = 5+curWave;
                bossAnnounced = false;
                GameObject.Find("GameHandler").GetComponent<VisualHud>().updateWheel(waveTimer);
            }
            else if (wavePos == 1)
            {
                waveTimer = 15.0f + curWave*2;
                alteredWaveSpeed = 3.5f;
                eliteChance = 35.0f;
                maximumEnemyCount = 7 + curWave;
                GameObject.Find("GameHandler").GetComponent<VisualHud>().updateWheel(waveTimer);
            }
            else if (wavePos == 2)
            {
                waveTimer = 10 + curWave*1.3f;
                alteredWaveSpeed = 1.5f;
                eliteChance = 5.0f;
                maximumEnemyCount = 8 + curWave * 2;
                GameObject.Find("GameHandler").GetComponent<VisualHud>().updateWheel(waveTimer);
            }
            else if (wavePos == 3 && bossAnnounced == false)
            {
                maximumEnemyCount = 0;
                alteredWaveSpeed = 30.0f;
                eliteChance = 5.0f;
                if (aliveEnemies <= 3 && bossAnnounced == false)
                {
                    waveTimer = 10;
                    GameObject.Find("GameHandler").GetComponent<VisualHud>().updateWheel(waveTimer);
                    alteredWaveSpeed = 7.0f;
                    maximumEnemyCount = 5 + curWave;
                    StartCoroutine(BossHandlerEvent());
                }
            }
        }
    }

    IEnumerator BossHandlerEvent()
    {
        bossAnnounced = true;
        GameObject.Find("GameHandler").GetComponent<MusicHandler>().gameTrack.Stop();
        GameObject.Find("GameHandler").GetComponent<MusicHandler>().gameTrackDrums.Stop();
        GameObject.Find("BossAnnounce").GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(10.0f);
        SpawnBoss();
        GameObject.Find("GameHandler").GetComponent<MusicHandler>().gameTrack.Play();
        GameObject.Find("GameHandler").GetComponent<MusicHandler>().gameTrackDrums.Play();
    }
}
