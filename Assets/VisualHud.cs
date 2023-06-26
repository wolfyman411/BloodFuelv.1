using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualHud : MonoBehaviour
{
    public float rotationDuration = 10f;

    Quaternion initialRotation;
    Quaternion targetRotation;
    float timeRotated = 0f;
    float timeMoved = 0f;
    GameObject wheelRef;

    GameObject imageObject;
    public Sprite[] imageRef;
    Vector3 initialImagePos;

    GameObject waveNumberRef;
    Vector3 initialWavePos;
    public Sprite[] waveRef;

    public Sprite[] bloodSprites;
    int bloodSprite = 0;
    Vector3 initialBloodPos;

    public Sprite[] tokenSprites;
    int tokenSprite = 0;

    GameObject WoF;
    GameObject WoFD;
    Vector3 initialWoF;
    void Start()
    {
        wheelRef = GameObject.Find("wheel");
        initialRotation = wheelRef.transform.rotation;
        targetRotation = Quaternion.Euler(wheelRef.transform.eulerAngles + new Vector3(0f, 0f, 90f));

        imageObject = GameObject.Find("phaseimage");
        initialImagePos = imageObject.transform.position;

        waveNumberRef = GameObject.Find("waveimage");
        initialWavePos = waveNumberRef.transform.position;

        WoF = GameObject.Find("WoF");
        WoFD = GameObject.Find("WoFDes");
        initialWoF = WoF.transform.position;

        InvokeRepeating("BloodPlay", 0.1f, 0.1f);
        initialBloodPos = GameObject.Find("bloodindicator").transform.position;

        InvokeRepeating("TokenPlay", 0.1f, 0.1f);
    }
    void BloodPlay()
    {
        if (bloodSprite >= bloodSprites.Length)
        {
            bloodSprite = 0;
        }
        GameObject.Find("bloodindicator").GetComponent<Image>().sprite = bloodSprites[bloodSprite];
        bloodSprite++;
        GameObject.Find("bloodindicator").transform.position = new Vector3(initialBloodPos.x,initialBloodPos.y - 0.4f*(GameObject.Find("Player").GetComponent<PlayerController>().MaxBloodFuel - GameObject.Find("Player").GetComponent<PlayerController>().BloodFuel),initialBloodPos.z);
    }
    void TokenPlay()
    {
        if (tokenSprite >= tokenSprites.Length)
        {
            tokenSprite = 0;
        }
        GameObject.Find("DeathToken").GetComponent<Image>().sprite = tokenSprites[tokenSprite];
        tokenSprite++;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<HordeHandler>().waveTimer >= 0f && GetComponent<HordeHandler>().curWave > 0)
        {
            rotateWheel();
        }
        GameObject.Find("hurtindicator").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, (GameObject.Find("Player").GetComponent<PlayerController>().MaxBloodFuel-GameObject.Find("Player").GetComponent<PlayerController>().BloodFuel) / GameObject.Find("Player").GetComponent<PlayerController>().MaxBloodFuel);
        GameObject.Find("Revives").GetComponent<Text>().text = GameObject.Find("Player").GetComponent<PlayerController>().numRevives.ToString()+"x";
    }

    public IEnumerator MoveObjectDown()
    {
        timeMoved = 0.0f;
        while (timeMoved < 0.1f)
        {
            timeMoved += Time.deltaTime;
            float t = Mathf.Clamp01(timeMoved / 0.1f);
            imageObject.transform.position = Vector3.Lerp(initialImagePos, new Vector3(initialImagePos.x, initialImagePos.y-50.0f, initialImagePos.z), t);

            yield return null;
        }
        yield return new WaitForSeconds(0.15f);

        imageObject.GetComponent<Image>().sprite = imageRef[(int)GetComponent<HordeHandler>().wavePos];
        timeMoved = 0.0f;
        while (timeMoved < 1.0f)
        {
            timeMoved += Time.deltaTime;
            float t = Mathf.Clamp01(timeMoved / 1.0f);
            imageObject.transform.position = Vector3.Lerp(imageObject.transform.position, new Vector3(initialImagePos.x, initialImagePos.y, initialImagePos.z), t);

            yield return null;
        }
    }

    public IEnumerator MoveWave()
    {
        timeMoved = 0.0f;
        while (timeMoved < 0.1f)
        {
            timeMoved += Time.deltaTime;
            float t = Mathf.Clamp01(timeMoved / 0.1f);
            waveNumberRef.transform.position = Vector3.Lerp(initialWavePos, new Vector3(initialWavePos.x, initialWavePos.y - 50.0f, initialWavePos.z), t);

            yield return null;
        }
        yield return new WaitForSeconds(0.15f);

        waveNumberRef.GetComponent<Image>().sprite = waveRef[GetComponent<HordeHandler>().curWave];
        timeMoved = 0.0f;
        while (timeMoved < 1.0f)
        {
            timeMoved += Time.deltaTime;
            float t = Mathf.Clamp01(timeMoved / 1.0f);
            waveNumberRef.transform.position = Vector3.Lerp(waveNumberRef.transform.position, new Vector3(initialWavePos.x, initialWavePos.y, initialWavePos.z), t);

            yield return null;
        }
    }

    public void updateWheel(float duration)
    {
        timeRotated = 0f;
        initialRotation = wheelRef.transform.rotation;
        targetRotation = Quaternion.Euler(wheelRef.transform.eulerAngles + new Vector3(0f, 0f, 90f));
        rotationDuration = duration;
    }

    void rotateWheel()
    {
        if (timeRotated >= rotationDuration)
        {
            return;
        }
        timeRotated += Time.deltaTime;

        float t = Mathf.Clamp01(timeRotated / rotationDuration);
        Quaternion currentRotation = Quaternion.Lerp(initialRotation, targetRotation, t);

        wheelRef.transform.rotation = currentRotation;
    }

    public void moveWof()
    {
        if (WoF.transform.position.x > WoFD.transform.position.x)
        {
            WoF.transform.position = new Vector3(WoF.transform.position.x - 1, WoF.transform.position.y, WoF.transform.position.z);
            Invoke("moveWof", 0.01f);
        }
        else
        {
            Invoke("returnWof", 2.0f);
        }
    }
    void returnWof()
    {
        if (WoF.transform.position.x < initialWoF.x)
        {
            WoF.transform.position = new Vector3(WoF.transform.position.x + 1, WoF.transform.position.y, WoF.transform.position.z);
            Invoke("returnWof", 0.01f);
        }
    }
}
