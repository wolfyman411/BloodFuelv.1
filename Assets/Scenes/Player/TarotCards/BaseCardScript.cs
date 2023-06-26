using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BaseCardScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string cardName;
    public string cardDesc;
    public string cardType;
    public string bulletEffect;
    public float cardCost;
    public float rechargeTime;
    public float speed;
    public float damage;
    public bool isPiercing;
    public PlayerController playerController;
    public GameObject selfRef;
    string whatType;
    float TimeAlive;
    public Texture2D cardImage;
    public bool isParent;
    public bool variationSound;

    public GameObject splatPart;

    float dashPower = 10f;
    public float playerBaseSpeed;

    soundhandler soundSystem;
    void Start()
    {
        soundSystem = GameObject.Find("AudioReference").GetComponent<soundhandler>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 999;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerBaseSpeed = playerController.speed;
        if (cardType == "Attack")
        {
            InvokeRepeating("timeAliveUpdate", 0.1f, 0.1f);
            //GetComponent<Collider2D>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - GameObject.Find("Player").transform.position).magnitude > 30.0f && cardType == "Attack" && isParent == false)
        {
            Destroy(gameObject);
        }

        //Homing Checker
        if (bulletEffect == "Homing")
        {
            GameObject nearestEnemy = null;
            float shortestDistance = Mathf.Infinity;
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Character");

            foreach (GameObject enemy in allEnemies)
            {
                if (enemy.GetComponent<EnemyHandler>().HP > 0.0f)
                {
                    float distance = Vector2.Distance(gameObject.transform.position, enemy.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                Rigidbody2D bulletRigidbody = GetComponent<Rigidbody2D>();
                if (bulletRigidbody != null)
                {
                    // Calculate the direction from the bullet to the nearest enemy
                    Vector2 targetDirection = (nearestEnemy.transform.position - gameObject.transform.position).normalized;
                    Vector2 newVelocity = bulletRigidbody.velocity.normalized + targetDirection * 5.0f;
                    bulletRigidbody.velocity = newVelocity.normalized * speed;
                }
            }
        }
    }

    public void useCard(float curRecharge)
    {
        if (curRecharge >= rechargeTime && playerController.cardShift)
        {
            soundhandler soundSystem = GameObject.Find("AudioReference").GetComponent<soundhandler>();
            //Sound Handler
            if (cardType == "Attack")
            {
                if (variationSound)
                {
                    soundSystem.createSound(soundSystem.heavyattack,gameObject.transform.position,true);
                }
                else
                {
                    soundSystem.createSound(soundSystem.playerattack, gameObject.transform.position, true);
                }
            }
            else
            {
                playerController.GetComponent<SpriteRenderer>().material = playerController.reviveMaterial;
                Invoke("FixMat", 0.3f);
                if (variationSound)
                {
                    soundSystem.createSound(soundSystem.boon, gameObject.transform.position, false);
                }
                else
                {
                    soundSystem.createSound(soundSystem.ability, gameObject.transform.position,false);
                }
            }
            if (cardType == "Attack")
            {
                if (bulletEffect == "Simple")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CreateBullet(mousePosition);
                }
                else if (bulletEffect == "Homing")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CreateBullet(mousePosition);
                }
                else if (bulletEffect == "2Shot")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CreateBullet(mousePosition);
                    GameObject SecondBullet = CreateBullet(mousePosition);
                    SecondBullet.GetComponent<Rigidbody2D>().velocity *= 0.5f;
                    SecondBullet.GetComponent<BaseCardScript>().damage *= 2f;
                    SecondBullet.transform.localScale *= 1.3f;
                }
                else if (bulletEffect == "Shotgun")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    for (int i = 0; i < 5; i++)
                    {
                        float spread = 3.0f;
                        CreateBullet(new Vector3 (mousePosition.x+Random.Range(-spread, spread), mousePosition.y + Random.Range(-spread, spread), 0.0f));
                    }
                }
                else if (bulletEffect == "Explosive")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    CreateBullet(mousePosition);
                }
            }
            if (cardType == "Ability")
            {
                if (bulletEffect == "Teleportation")
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.z = 0f;
                    GameObject.Find("Player").transform.position = mousePosition;
                }
                if (bulletEffect == "Dash")
                {
                    float dashDuration = 0.3f;
                    playerController.speed += dashPower;
                    playerController.isDashing = true;
                    Invoke("StopDash", dashDuration);
                }
                if (bulletEffect == "Temperance")
                {
                    playerController.subBlood(-1 * playerController.MaxBloodFuel/2);
                }
                if (bulletEffect == "Random")
                {
                    Invoke("RandomEffect", 0.1f);
                }
                if (bulletEffect == "Invisible")
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.GetComponent<NavMeshAgent>())
                        {
                            enemy.GetComponent<EnemyFollow>().ClearTarget();
                        }
                    }
                    GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    playerController.tempSpeed = 5.0f;
                    Invoke("StopInvisibilty", 1.0f);
                }
                if (bulletEffect == "Shockwave")
                {
                    GameObject.Find("Main Camera").GetComponent<follower>().ShakeCamera(0.5f, 0.2f);
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.GetComponent<EnemyHandler>() && (GameObject.Find("Player").transform.position - enemy.transform.position).magnitude < 10.0f)
                        {
                            enemy.GetComponent<EnemyHandler>().subHP(damage);
                        }
                    }
                }
                if (bulletEffect == "Dummy")
                {
                    GameObject dummyRef = Instantiate(GetComponent<dummyRef>().dummyObject);
                    dummyRef.transform.position = GameObject.Find("Player").transform.position;
                    GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget = dummyRef;
                    GameObject.Find("GameHandler").GetComponent<GameLogic>().UpdateTargetting();
                    Invoke("FixDummy", 5.0f);
                }
            }
            GameObject.Find("Player").GetComponent<PlayerController>().resetCoolDown(whatType);
            GameObject.Find("Player").GetComponent<PlayerController>().subBlood(cardCost);
        }
    }
    GameObject CreateBullet(Vector3 bulletDirection)
    {
        GameObject newBullet = Instantiate(selfRef);
        newBullet.GetComponent<BaseCardScript>().isParent = false;
        newBullet.transform.position = GameObject.Find("Player").transform.position;
        Rigidbody2D newBulletRb = newBullet.GetComponent<Rigidbody2D>();
        if (newBulletRb != null)
        {
            bulletDirection.z = 0f; // Set the z-coordinate to ensure it's in the same plane as the bullet

            // Calculate the launch direction from player to mouse position
            Vector2 launchDirection = (bulletDirection - newBullet.transform.position).normalized;

            newBulletRb.AddForce(launchDirection * speed, ForceMode2D.Impulse);
            float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
            newBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        return newBullet;
    }
    public void FixDummy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
        GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget = GameObject.Find("Player");
        GameObject.Find("GameHandler").GetComponent<GameLogic>().UpdateTargetting();
    }
    public void StopInvisibilty()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<NavMeshAgent>())
            {
                enemy.GetComponent<EnemyFollow>().SetTarget();
            }
        }
        GameObject.Find("Player").GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        playerController.tempSpeed = 0.0f;
    }
    public void RandomEffect()
    {
        int randomInt = Random.Range(1,5);
        if (randomInt == 1)
        {
            GameObject.Find("WoF").GetComponent<Text>().text = "Speed Increased!";
            playerController.speed += 0.1f;
        }
        else if (randomInt == 2)
        {
            GameObject.Find("WoF").GetComponent<Text>().text = "Damage Increased!";
            playerController.baseDamageAdditive += 0.1f;
        }
        else if(randomInt == 3)
        {
            GameObject.Find("WoF").GetComponent<Text>().text = "Max BloodFuel Increased!";
            playerController.MaxBloodFuel += 5f;
        }
        else if(randomInt == 4)
        {
            GameObject.Find("WoF").GetComponent<Text>().text = "Dodge Chance Increased!";
            playerController.baseDodgeChance += 2f;
        }
        else
        {
            GameObject.Find("WoF").GetComponent<Text>().text = "Unlucky, Nothing Increased!";
        }
        GameObject.Find("GameHandler").GetComponent<VisualHud>().moveWof();
    }
    public void StopDash()
    {
        playerController.isDashing = false;
        playerController.speed = playerBaseSpeed;
    }
    public void setType(string type)
    {
        whatType = type;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") && isPiercing == false && cardType == "Attack" && isParent == false)
        {
            if (bulletEffect == "Explosive")
            {
                soundSystem.createSound(soundSystem.explosion, gameObject.transform.position, true);
                GameObject dummyRef = Instantiate(GetComponent<dummyRef>().dummyObject);
                dummyRef.transform.position = gameObject.transform.position;
                dummyRef.GetComponent<SpriteRenderer>().sortingOrder = 999;
            }
            GameObject splatRef = Instantiate(splatPart);
            splatRef.transform.position = gameObject.transform.position;
            Destroy(gameObject);
        }
        if (other.CompareTag("Character") && isPiercing == false && cardType == "Attack" && isParent == false)
        {
            if (other.GetComponent<EnemyHandler>().HP > 0.0f)
            {
                if (bulletEffect == "Explosive")
                {
                    soundSystem.createSound(soundSystem.explosion, gameObject.transform.position, true);
                    GameObject dummyRef = Instantiate(GetComponent<dummyRef>().dummyObject);
                    dummyRef.transform.position = gameObject.transform.position;
                    dummyRef.GetComponent<SpriteRenderer>().sortingOrder = 999;
                }
                other.GetComponentInParent<EnemyHandler>().subHP(damage * playerController.damageAdditive);
                GameObject splatRef = Instantiate(splatPart);
                splatRef.transform.position = gameObject.transform.position;
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Character") && isPiercing == true && cardType == "Attack" && isParent == false)
        {
            if (other.GetComponent<EnemyHandler>().HP > 0.0f)
            {
                other.GetComponentInParent<EnemyHandler>().subHP(damage * playerController.damageAdditive);
            }
        }
    }

    public void timeAliveUpdate()
    {
        TimeAlive += 0.1f;
    }

    void FixMat()
    {
        playerController.GetComponent<SpriteRenderer>().material = playerController.baseMat;
    }
}
