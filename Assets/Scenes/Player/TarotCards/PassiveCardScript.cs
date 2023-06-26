using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveCardScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string cardName;
    public string cardDesc;
    public string statUpgrade;
    public float statUpgradeAmount;
    public PlayerController playerController;
    public Texture2D cardImage;
    public bool isUpgrade;
    public bool alreadyActivated = false;

    public float JudgementBoost = 0.0f;

    void Awake()
    {
        alreadyActivated = false;
    }

    public void ActivateCard()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (alreadyActivated == false)
        {
            if (isUpgrade == true)
            {
                alreadyActivated = true;
            }

            //Check Type
            if (statUpgrade == "Damage")
            {
                playerController.baseDamageAdditive *= statUpgradeAmount;
            }
            else if (statUpgrade == "Revive")
            {
                playerController.numRevives += (int)statUpgradeAmount;
            }
            else if (statUpgrade == "Dodge")
            {
                playerController.curDodgeChance *= statUpgradeAmount;
            }

            //SpecialTypes
            if (statUpgrade == "Justice")
            {
                playerController.damageMultiplier += (playerController.MaxBloodFuel - playerController.BloodFuel)/40;
            }
            if (statUpgrade == "Star")
            {
                playerController.damageMultiplier += ((float)GameObject.Find("GameHandler").GetComponent<HordeHandler>().aliveBasicEnemies) / 20;
            }
            else if (statUpgrade == "Moon")
            {
                playerController.damageMultiplier += ((float)GameObject.Find("GameHandler").GetComponent<HordeHandler>().aliveEliteEnemies) / 5;
            }
            else if (statUpgrade == "Sun")
            {
                if (((float)GameObject.Find("GameHandler").GetComponent<HordeHandler>().wavePos >= 3))
                {
                    playerController.damageMultiplier += 0.5f;
                }
            }
            else if (statUpgrade == "Judgement")
            {
                playerController.damageMultiplier += playerController.playerCombo / 20;
                playerController.extraSpeed += playerController.playerCombo / 20;
            }
        }
    }
    public void StatDecay()
    {
        if (JudgementBoost > 0.0f)
        {
            JudgementBoost -= 0.1f;
        }
    }
}
