using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class orionend : MonoBehaviour
{
    public GameObject whiteSphere;
    bool gameEnded;
    PlayerController playerController;
    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    void Update()
    {
        if (GetComponent<EnemyHandler>().HP <= 0 && gameEnded == false)
        {
            GameObject.Find("OrionDefeat").GetComponent<AudioSource>().Play();
            hideHUD();
            StartCoroutine(winGame());
            gameEnded = true;
            Invoke("winGame2", 5.0f);
            Invoke("winScreen", 15.0f);
        }
    }

    IEnumerator winGame()
    {
        playerController.cardM1 = null;
        playerController.cardM2 = null;
        playerController.cardShift = null;
        playerController.cardSpace = null;
        playerController.cardCtrl = null;
        GameObject.Find("GameHandler").GetComponent<CardDisplayHandler>().UpdateCards();
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().MoveCards = false;
        GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI = true;
        //Create Sphere
        GameObject sRef = Instantiate(whiteSphere);
        sRef.transform.position = gameObject.transform.position;

        //Expand Sphere medium fast
        ScaleObject(0.5f, 2.0f, sRef);
        yield return new WaitForSeconds(1.0f);

        //Shrink Sphere small slow
        ScaleObject(3.5f, 0.1f, sRef);
        yield return new WaitForSeconds(4.0f);

        //Expand Sphere large fast
        ScaleObject(1.0f, 75.0f, sRef);
        GameObject.Find("PlayerMimic").transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        yield return new WaitForSeconds(1.0f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");

        //RemoveEnemies
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyHandler>())
            {
                //Destroy(enemy);
            }
        }
    }
    void winGame2()
    {
        InvokeRepeating("hidePlayer", 0.1f, 0.1f);
    }

    void winScreen()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void ScaleObject(float duration, float targetScale, GameObject objectRef)
    {
        StartCoroutine(ScaleCoroutine(duration, targetScale, objectRef));
    }

    private IEnumerator ScaleCoroutine(float duration, float targetScale, GameObject objectRef)
    {
        Vector3 initialScale = objectRef.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            objectRef.transform.localScale = Vector3.Lerp(initialScale, new Vector2(targetScale, targetScale), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectRef.transform.localScale = new Vector2(targetScale, targetScale);
    }

    void hideHUD()
    {
        GameObject.Find("HealthBar").GetComponent<RectTransform>().localScale = Vector2.zero;
        GameObject.Find("Wavebar").GetComponent<RectTransform>().localScale = Vector2.zero;
        GameObject.Find("ScoreBackground").GetComponent<RectTransform>().localScale = Vector2.zero;
        GameObject.Find("BossHP").GetComponent<RectTransform>().localScale = Vector2.zero;
        GameObject.Find("Score").GetComponent<RectTransform>().localScale = Vector2.zero;
        GameObject.Find("ReviveImage").GetComponent<RectTransform>().localScale = Vector2.zero;
    }

    void hidePlayer()
    {
        Debug.Log("test");
        if (GameObject.Find("Player").GetComponent<SpriteRenderer>().flipX)
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(-1.4f, 1.4f, 1.4f);
        }
        else
        {
            GameObject.Find("PlayerMimic").transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
        GameObject.Find("PlayerMimic").GetComponent<Image>().color = new Color(1, 1, 1, GameObject.Find("PlayerMimic").GetComponent<Image>().color.a - 0.01f);
    }
}
