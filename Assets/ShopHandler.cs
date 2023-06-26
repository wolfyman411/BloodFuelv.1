using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public static int SavedTokenAmount;
    public static int[] amountPurchased = new int[5];
    public AudioClip[] introLine;
    public AudioClip[] buyLine;
    public AudioClip[] exitLine;
    public AudioClip[] cheapLine;
    public AudioSource merchantSound;
    // Start is called before the first frame update
    void Start()
    {
        LoadPrefs();
        Invoke("UpdateToken", 0.2f);
    }

    void UpdateToken()
    {
        updatedstats.upgradeBloodFuel = amountPurchased[0] * 15;
        updatedstats.upgradeDodge = amountPurchased[1] * 2.5f;
        updatedstats.upgradeDamage = amountPurchased[2] * 0.1f;
        updatedstats.upgradeRevive = amountPurchased[3];
        updatedstats.upgradeSpeed = amountPurchased[4] * 3.5f;
        GameObject.Find("TokenAmount").GetComponent<Text>().text = "x" + SavedTokenAmount.ToString();
        GameObject.Find("BloodFuelText").GetComponent<Text>().text = (updatedstats.upgradeBloodFuel+100).ToString();
        GameObject.Find("SpeedText").GetComponent<Text>().text = (updatedstats.upgradeSpeed+6).ToString();
        GameObject.Find("DodgeText").GetComponent<Text>().text = updatedstats.upgradeDodge.ToString();
        GameObject.Find("DamageText").GetComponent<Text>().text = updatedstats.upgradeDamage.ToString();
        GameObject.Find("RevivesText").GetComponent<Text>().text = updatedstats.upgradeRevive.ToString();
        SavePrefs();
    }

    void addToken(int amount)
    {
        SavedTokenAmount += amount;
        UpdateToken();
    }

    public void upgrade(int costOf, string whatType, float howMuch)
    {
        if (costOf <= SavedTokenAmount)
        {
            if (whatType == "bloodfuel")
            {
                updatedstats.upgradeBloodFuel += howMuch;
                amountPurchased[0] += 1;
            }
            else if (whatType == "dodge")
            {
                updatedstats.upgradeDodge += howMuch;
                amountPurchased[1] += 1;
            }
            else if (whatType == "damage")
            {
                updatedstats.upgradeDamage += howMuch;
                amountPurchased[2] += 1;
            }
            else if (whatType == "revive")
            {
                updatedstats.upgradeRevive += (int)howMuch;
                amountPurchased[3] += 1;
            }
            else if (whatType == "speed")
            {
                updatedstats.upgradeSpeed += howMuch;
                amountPurchased[4] += 1;
            }
            addToken(-costOf);
            Debug.Log(howMuch + " " + costOf + " " + whatType);
            purchaseLine();
            GameObject.Find("PurchaseSound").GetComponent<AudioSource>().Play();
        }
        else
        {
            brokeLine();
        }
    }

    public void purchaseLine()
    {
        merchantSound.clip = buyLine[Random.Range(0, buyLine.Length)];
        merchantSound.Play();
    }
    public void enterLine()
    {
        merchantSound.clip = introLine[Random.Range(0,introLine.Length)];
        merchantSound.Play();
    }
    public void leaveLine()
    {
        merchantSound.clip = exitLine[Random.Range(0, exitLine.Length)];
        merchantSound.Play();
    }
    public void brokeLine()
    {
        merchantSound.clip = cheapLine[Random.Range(0, cheapLine.Length)];
        merchantSound.Play();
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("BloodTokens", SavedTokenAmount);
    }

    public void LoadPrefs()
    {
        SavedTokenAmount = PlayerPrefs.GetInt("BloodTokens");
    }
}
