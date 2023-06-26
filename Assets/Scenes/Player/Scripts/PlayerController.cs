using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player Stat Vars
    public float speed = 20;
    public float extraSpeed;
    public float tempSpeed;
    public float BloodFuel = 100;
    public float MaxBloodFuel = 100;
    float h;
    float v;

    //Special Stats
    public float damageAdditive;
    public float baseDamageAdditive;
    public float damageMultiplier;
    public int numRevives;
    public float baseDodgeChance;
    public float curDodgeChance;
    public bool isDashing;
    public bool playerHurt;
    public int playerCombo = 1;
    public float bleedOutRate = 5.0f;


    //Card Handler
    float ctrlkey;
    float shiftkey;
    float spacekey;
    float m1key;
    float m2key;
    public GameObject cardShift;
    public GameObject cardSpace;
    public GameObject cardCtrl;
    public GameObject cardM1;
    public GameObject cardM2;
    public List<GameObject> upgradeCards;
    //Card Statsc
    public float cardShiftCooldown;
    public float cardSpaceCooldown;
    public float cardCtrlCooldown;
    public float cardM1Cooldown;
    public float cardM2Cooldown;
    public int cardStage = 0;

    //HUD
    public GameObject bloodFuelDis;
    public BloodFuelScript bloodFuelScript;

    public Material hurtMaterial;
    public Material reviveMaterial;
    public Material baseMat;

    soundhandler soundSystem;
    // Start is called before the first frame update
    void Start()
    {
        baseMat = gameObject.GetComponent<SpriteRenderer>().material;
        soundSystem = GameObject.Find("AudioReference").GetComponent<soundhandler>();
        InvokeRepeating("UpdateCooldown", 0.01f, 0.01f);

        //Stats
        damageAdditive = baseDamageAdditive;
        curDodgeChance = baseDodgeChance;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement Handler
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * (speed + extraSpeed + tempSpeed) * h);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * (speed+extraSpeed + tempSpeed) * v);

        //Update BloodFuel
        bloodFuelScript.UpdateText(getBloodFuel().ToString());
        VelCap();
        if (GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == false)
        {
            cardInputChecker();
        }
        applyCardUpgrades();

        //HP Checker
        if (BloodFuel < 0.0f)
        {
            BloodFuel = 0.0f;
            if (numRevives > 0)
            {
                numRevives--;
                BloodFuel = MaxBloodFuel / 2;
            }
            else
            {
                Debug.Log("GameOver!");
            }
        }
        if (BloodFuel > MaxBloodFuel)
        {
            BloodFuel = MaxBloodFuel;
        }

        //Gameover Checker
        if (BloodFuel <= 0.0f && playerHurt && numRevives <= 0 && GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == false)
        {
            GameObject.Find("GameHandler").GetComponent<GameLogic>().StartCoroutine(GameObject.Find("GameHandler").GetComponent<GameLogic>().gameOverEvent());
            soundSystem.createSound(soundSystem.playerdead, gameObject.transform.position, false);
        }

        //Dash Checker
        if (isDashing)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Character");
            foreach (GameObject enemy in allEnemies)
            {
                if (enemy.GetComponent<EnemyHandler>() && (transform.position - enemy.transform.position).magnitude < 2.0f && enemy.GetComponent<EnemyHandler>().HP > 1.0f)
                {

                    enemy.GetComponent<EnemyHandler>().subHP(1);
                }
            }
        }

        //Hurt Checker
        if (playerHurt)
        {
            GetComponent<SpriteRenderer>().material = hurtMaterial;
            Invoke("PlayerNotHurt", 0.2f);
            playerCombo = 1;
        }
    }

    public void cardInputChecker()
    {
        ctrlkey = Input.GetAxisRaw("Ctrl");
        shiftkey = Input.GetAxisRaw("Shift");
        spacekey = Input.GetAxisRaw("Space");
        m1key = Input.GetAxisRaw("M1");
        m2key = Input.GetAxisRaw("M2");

        if (ctrlkey > 0.0)
        {
            if (cardCtrl != null && cardCtrlCooldown >= cardCtrl.GetComponent<BaseCardScript>().rechargeTime)
            {
                cardCtrl.GetComponent<BaseCardScript>().useCard(cardCtrlCooldown);
                cardCtrl.GetComponent<BaseCardScript>().setType("CTRL");
                gameObject.GetComponent<PlayerAnimator>().ActivatedCard(cardCtrl.GetComponent<BaseCardScript>().cardType, cardCtrl.GetComponent<BaseCardScript>().variationSound);
            }
        }
        if (shiftkey > 0.0)
        {
            if (cardShift != null && cardShiftCooldown >= cardShift.GetComponent<BaseCardScript>().rechargeTime)
            {
                cardShift.GetComponent<BaseCardScript>().useCard(cardShiftCooldown);
                cardShift.GetComponent<BaseCardScript>().setType("SHIFT");
                gameObject.GetComponent<PlayerAnimator>().ActivatedCard(cardShift.GetComponent<BaseCardScript>().cardType, cardShift.GetComponent<BaseCardScript>().variationSound);
            }
        }
        if (spacekey > 0.0)
        {
            if (cardSpace != null && cardSpaceCooldown >= cardSpace.GetComponent<BaseCardScript>().rechargeTime)
            {
                cardSpace.GetComponent<BaseCardScript>().useCard(cardSpaceCooldown);
                cardSpace.GetComponent<BaseCardScript>().setType("SPACE");
                gameObject.GetComponent<PlayerAnimator>().ActivatedCard(cardSpace.GetComponent<BaseCardScript>().cardType, cardSpace.GetComponent<BaseCardScript>().variationSound);
            }
        }
        if (m1key > 0.0)
        {
            if (cardM1 != null && cardM1Cooldown >= cardM1.GetComponent<BaseCardScript>().rechargeTime)
            {
                cardM1.GetComponent<BaseCardScript>().useCard(cardM1Cooldown);
                cardM1.GetComponent<BaseCardScript>().setType("M1");
                gameObject.GetComponent<PlayerAnimator>().ActivatedCard(cardM1.GetComponent<BaseCardScript>().cardType, cardM1.GetComponent<BaseCardScript>().variationSound);
            }
        }
        if (m2key > 0.0)
        {
            if (cardM2 != null && cardM2Cooldown >= cardM2.GetComponent<BaseCardScript>().rechargeTime)
            {
                cardM2.GetComponent<BaseCardScript>().useCard(cardM2Cooldown);
                cardM2.GetComponent<BaseCardScript>().setType("M2");
                gameObject.GetComponent<PlayerAnimator>().ActivatedCard(cardM2.GetComponent<BaseCardScript>().cardType, cardM2.GetComponent<BaseCardScript>().variationSound);
            }
        }
    }
    
    public void subBlood(float cost)
    {
        BloodFuel -= cost;
        if (cost > 20 && playerHurt)
        {
            soundSystem.createSound(soundSystem.playerhurtheavy, gameObject.transform.position, false);
        } 
    }

    public void PlayerNotHurt()
    {
        GetComponent<SpriteRenderer>().material = baseMat;
        playerHurt = false;
    }

    public void resetCoolDown(string whatCooldown)
    {
        if (whatCooldown == "CTRL")
        {
            cardCtrlCooldown = 0.0f;
        }
        if (whatCooldown == "SPACE")
        {
            cardSpaceCooldown = 0.0f;
        }
        if (whatCooldown == "SHIFT")
        {
            cardShiftCooldown = 0.0f;
        }
        if (whatCooldown == "M1")
        {
            cardM1Cooldown = 0.0f;
        }
        if (whatCooldown == "M2")
        {
            cardM2Cooldown = 0.0f;
        }
    }

    public void UpdateCooldown()
    {
        if (cardShift)
            cardShiftCooldown += 0.01f;
        if (cardSpace)
            cardSpaceCooldown += 0.01f;
        if (cardCtrl)
            cardCtrlCooldown += 0.01f;
        if (cardM1)
            cardM1Cooldown += 0.01f;
        if (cardM2)
            cardM2Cooldown += 0.01f;
    }

    public float getBloodFuel()
    {
        return BloodFuel;
    }
    public float getMaxBloodFuel()
    {
        return MaxBloodFuel;
    }
    public void removeBloodFuel(float BloodSub)
    {
        BloodFuel -= BloodSub;
    }
    void VelCap()
    {
        float CappedXVelocity;
        float CappedYVelocity;
        if (h > 0)
        {
            CappedXVelocity = Mathf.Min(GetComponent<Rigidbody2D>().velocity.x, (speed + extraSpeed + tempSpeed));
        }
        else
        {
            CappedXVelocity = Mathf.Max(GetComponent<Rigidbody2D>().velocity.x, -(speed + extraSpeed + tempSpeed));
        }

        if (v > 0)
        {
            CappedYVelocity = Mathf.Min(GetComponent<Rigidbody2D>().velocity.y, (speed + extraSpeed + tempSpeed));
        }
        else
        {
            CappedYVelocity = Mathf.Max(GetComponent<Rigidbody2D>().velocity.y, -(speed + extraSpeed + tempSpeed));
        }

        //Walking Handlers
        if (v == 0 && h == 0)
           GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
        else
           GetComponent<Rigidbody2D>().velocity = new Vector2(CappedXVelocity, CappedYVelocity);

        //Sideways Handlers
        if (v != 0 && h != 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(CappedXVelocity, CappedYVelocity);
        else if (h != 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(CappedXVelocity, 0.0f);
        else if (v != 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, CappedYVelocity);
    }

    void applyCardUpgrades()
    {
        damageMultiplier = 1.0f;
        extraSpeed = 0.0f;
        foreach (GameObject card in upgradeCards)
        {
            if (card)
            {
                card.GetComponent<PassiveCardScript>().ActivateCard();
            }
        }
        if (damageMultiplier > 0.0f)
        {
            damageAdditive = baseDamageAdditive * damageMultiplier;
        }
    }

    public void addCard(int cardNum)
    {
        GameObject cardRef = Instantiate(GameObject.Find("GameHandler").GetComponent<CardShuffler>().shuffledCards[cardNum]);
        cardRef.transform.position = new Vector3(0, 0, -99999);
        bool removeAttacks = false;
        bool removeAbilities = false;

        //Upgrade Checker
        if (!(cardRef.GetComponent<PassiveCardScript>()))
        {
            if (cardRef.GetComponent<BaseCardScript>().cardType == "Attack")
            {
                if (!(cardM1))
                {
                    cardM1 = cardRef;
                }
                else if (!(cardM2))
                {
                    cardM2 = cardRef;
                    removeAttacks = true;
                }
            }
            else if (cardRef.GetComponent<BaseCardScript>().cardType == "Ability")
            {
                cardRef.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                if (!(cardShift))
                {
                    cardShift = cardRef;
                }
                else if (!(cardCtrl))
                {
                    cardCtrl = cardRef;
                }
                else if (!(cardSpace))
                {
                    cardSpace = cardRef;
                    removeAbilities = true;
                }
            }
        }
        //Card Remover
        if (cardRef.GetComponent<BaseCardScript>())
        {
            soundSystem.createSound(soundSystem.cardpick, gameObject.transform.position, false);
            foreach (GameObject card in GameObject.Find("GameHandler").GetComponent<CardShuffler>().availibleCards)
            {
                if (card.GetComponent<BaseCardScript>())
                {
                    if (card.GetComponent<BaseCardScript>().cardName == cardRef.GetComponent<BaseCardScript>().cardName)
                    {
                        GameObject.Find("GameHandler").GetComponent<CardShuffler>().availibleCards.Remove(card);
                        GameObject.Find("GameHandler").GetComponent<CardShuffler>().unshuffledCards.Remove(card);
                        break;
                    }
                }
            }
            //RemoveAttacks
            if (removeAttacks)
            {
                GameObject.Find("GameHandler").GetComponent<CardShuffler>().availibleCards.RemoveAll(card =>
                {
                    //Checks to make sure BaseCardScript
                    BaseCardScript baseCardScript = card?.gameObject.GetComponent<BaseCardScript>();
                    return baseCardScript != null && baseCardScript.cardType == "Attack";
                });
                GameObject.Find("GameHandler").GetComponent<CardShuffler>().unshuffledCards.RemoveAll(card =>
                {
                    BaseCardScript baseCardScript = card?.gameObject.GetComponent<BaseCardScript>();
                    return baseCardScript != null && baseCardScript.cardType == "Attack";
                });
            }
            //RemoveAbilities
            if (removeAbilities)
            {
                GameObject.Find("GameHandler").GetComponent<CardShuffler>().availibleCards.RemoveAll(card =>
                {
                    //Checks to make sure BaseCardScript
                    BaseCardScript baseCardScript = card?.gameObject.GetComponent<BaseCardScript>();
                    return baseCardScript != null && baseCardScript.cardType == "Ability";
                });
                GameObject.Find("GameHandler").GetComponent<CardShuffler>().unshuffledCards.RemoveAll(card =>
                {
                    BaseCardScript baseCardScript = card?.gameObject.GetComponent<BaseCardScript>();
                    return baseCardScript != null && baseCardScript.cardType == "Ability";
                });
            }
        }

        else
        {
            soundSystem.createSound(soundSystem.carddraw, gameObject.transform.position, false);
            upgradeCards.Add(cardRef);
        }
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().ShuffleCards("");
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().Invoke("changeCards", 1.0f);
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().MoveCards = false;
        if (cardShift)
        {
            GameObject.Find("GameHandler").GetComponent<HordeHandler>().NextWave();
        }
        else
        {
            Invoke("AbilityCards", 1.0f);
        }
        GameObject.Find("GameHandler").GetComponent<CardDisplayHandler>().UpdateCards();
    }

    void AbilityCards()
    {
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().MoveCards = true;
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().ShuffleCards("ability");
        GameObject.Find("GameHandler").GetComponent<CardShuffler>().changeCards();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Wall");
        }
    }
}
