using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopitem : MonoBehaviour
{
    public string type;
    public int baseCost;
    public int updatedCost;
    public float amount;
    public float percentIncrease;
    public int amountPurchased;
    public int shopid;
    public Text textAmountPurchased;
    public Text textCost;
    public string description;

    private void Start()
    {
        ShopHandler.amountPurchased[shopid] = PlayerPrefs.GetInt("ShopItem" + shopid.ToString());
    }
    private void Update()
    {
        updatedCost = (int)(baseCost + amountPurchased * percentIncrease);
        amountPurchased = ShopHandler.amountPurchased[shopid];
        textAmountPurchased.text = amountPurchased.ToString();
        textCost.text = updatedCost.ToString();

        if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            GameObject.Find("Description").GetComponent<Text>().text = description;
        }
    }

    public void makePurchase()
    {
        GameObject.Find("ShopHandler").GetComponent<ShopHandler>().upgrade(updatedCost,type,amount);
        PlayerPrefs.SetInt("ShopItem" + shopid.ToString(), ShopHandler.amountPurchased[shopid]);
    }
}
