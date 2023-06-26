using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayHandler : MonoBehaviour
{
    public BaseCardScript M1Card;
    public BaseCardScript M2Card;
    public BaseCardScript SpaceCard;
    public BaseCardScript ShiftCard;
    public BaseCardScript CtrlCard;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        InvokeRepeating("RechargeCards", 0.01f, 0.01f);
        UpdateCards();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RechargeCards()
    {
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardM1 && playerController.cardM1Cooldown / M1Card.rechargeTime <= 1.0f)
        {
            M1Card = GameObject.Find("Player").GetComponent<PlayerController>().cardM1.GetComponent<BaseCardScript>();
            GameObject.Find("M1Cooldown").GetComponent<Image>().transform.localScale = new Vector3((M1Card.rechargeTime - playerController.cardM1Cooldown) / M1Card.rechargeTime, 1.0f, 1.0f);
        }
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardM2 && playerController.cardM2Cooldown / M2Card.rechargeTime <= 1.0f)
        {
            M2Card = GameObject.Find("Player").GetComponent<PlayerController>().cardM2.GetComponent<BaseCardScript>();
            GameObject.Find("M2Cooldown").GetComponent<Image>().transform.localScale = new Vector3((M2Card.rechargeTime - playerController.cardM2Cooldown) / M2Card.rechargeTime, 1.0f, 1.0f);
        }
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardSpace && playerController.cardSpaceCooldown / SpaceCard.rechargeTime <= 1.0f)
        {
            SpaceCard = GameObject.Find("Player").GetComponent<PlayerController>().cardSpace.GetComponent<BaseCardScript>();
            GameObject.Find("SpaceCooldown").GetComponent<Image>().transform.localScale = new Vector3((SpaceCard.rechargeTime - playerController.cardSpaceCooldown) / SpaceCard.rechargeTime, 1.0f, 1.0f);
        }
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardShift && playerController.cardShiftCooldown / ShiftCard.rechargeTime <= 1.0f)
        {
            ShiftCard = GameObject.Find("Player").GetComponent<PlayerController>().cardShift.GetComponent<BaseCardScript>();
            GameObject.Find("ShiftCooldown").GetComponent<Image>().transform.localScale = new Vector3((ShiftCard.rechargeTime - playerController.cardShiftCooldown) / ShiftCard.rechargeTime, 1.0f, 1.0f);
        }
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardCtrl && playerController.cardCtrlCooldown / CtrlCard.rechargeTime <= 1.0f)
        {
            CtrlCard = GameObject.Find("Player").GetComponent<PlayerController>().cardCtrl.GetComponent<BaseCardScript>();
            GameObject.Find("CtrlCooldown").GetComponent<Image>().transform.localScale = new Vector3((CtrlCard.rechargeTime - playerController.cardCtrlCooldown) / CtrlCard.rechargeTime, 1.0f, 1.0f);
        }
    }

    public void UpdateCards()
    {
        if (GameObject.Find("Player").GetComponent<PlayerController>().cardM1)
        {
            GameObject.Find("Card1").transform.localScale = Vector3.one;
            M1Card = GameObject.Find("Player").GetComponent<PlayerController>().cardM1.GetComponent<BaseCardScript>();
           ChangeCardDesc("M1", M1Card.cardName, M1Card.cardDesc, M1Card.cardImage);
        }
        else
        {
            GameObject.Find("Card1").transform.localScale = Vector3.zero;
        }

        if (GameObject.Find("Player").GetComponent<PlayerController>().cardM2)
        {
            GameObject.Find("Card2").transform.localScale = Vector3.one;
            M2Card = GameObject.Find("Player").GetComponent<PlayerController>().cardM2.GetComponent<BaseCardScript>();
            ChangeCardDesc("M2", M2Card.cardName, M2Card.cardDesc, M2Card.cardImage);
        }
        else
        {
            GameObject.Find("Card2").transform.localScale = Vector3.zero;
        }

        if (GameObject.Find("Player").GetComponent<PlayerController>().cardSpace)
        {
            GameObject.Find("Card3").transform.localScale = Vector3.one;
            SpaceCard = GameObject.Find("Player").GetComponent<PlayerController>().cardSpace.GetComponent<BaseCardScript>();
            ChangeCardDesc("Space", SpaceCard.cardName, SpaceCard.cardDesc, SpaceCard.cardImage);
        }
        else
        {
            GameObject.Find("Card3").transform.localScale = Vector3.zero;
        }

        if (GameObject.Find("Player").GetComponent<PlayerController>().cardShift)
        {
            GameObject.Find("Card5").transform.localScale = Vector3.one;
            ShiftCard = GameObject.Find("Player").GetComponent<PlayerController>().cardShift.GetComponent<BaseCardScript>();
            ChangeCardDesc("Shift", ShiftCard.cardName, ShiftCard.cardDesc, ShiftCard.cardImage);
        }
        else
        {
            GameObject.Find("Card5").transform.localScale = Vector3.zero;
        }

        if (GameObject.Find("Player").GetComponent<PlayerController>().cardCtrl)
        {
            GameObject.Find("Card4").transform.localScale = Vector3.one;
            CtrlCard = GameObject.Find("Player").GetComponent<PlayerController>().cardCtrl.GetComponent<BaseCardScript>();
            ChangeCardDesc("Ctrl", CtrlCard.cardName, CtrlCard.cardDesc, CtrlCard.cardImage);
        }
        else
        {
            GameObject.Find("Card4").transform.localScale = Vector3.zero;
        }
    }

    void ChangeCardDesc(string cardType, string cardName, string cardDesc, Texture2D cardImage)
    {
        GameObject.Find(cardType + "Title").GetComponent<Text>().text = cardName;
        GameObject.Find(cardType + "Desc").GetComponent<Text>().text = cardDesc;
        GameObject.Find(cardType + "CardImage").GetComponent<Image>().sprite = Sprite.Create(cardImage, new Rect(0, 0, cardImage.width, cardImage.height), Vector2.one * 0.5f);
    }
}
