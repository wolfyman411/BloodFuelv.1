using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyHandler : MonoBehaviour
{
    public float HP;
    public float HPCap;
    public float Reward;
    public float Speed;
    public float attackSpeed;
    public float attackRange;
    public bool IsRanged;
    public string eliteClass;
    public string bossClass;
    public bool canAttack = true;
    public GameObject attackType;
    public GameObject attackType2;
    public GameObject attackType3;
    public Vector2 attackSize;
    public float attackDamage;
    public string specialAttack;
    public bool isBoosted;
    int attackNum;
    public float attackCooldown;
    public GameObject eliteIcon;
    public Sprite[] Icons;
    bool alreadyDied;
    public Material hurtMaterial;
    public Material reviveMaterial;
    Material baseMat;
    Color defaultColor;

    soundhandler soundSystem;
    // Start is called before the first frame update
    void Start()
    {
        canAttack = false;
        soundSystem = GameObject.Find("AudioReference").GetComponent<soundhandler>();
        HPCap = HP;
        GetComponent<NavMeshAgent>().speed = Speed;
        baseMat = gameObject.GetComponent<SpriteRenderer>().material;

        //Invisible Fixer
        gameObject.transform.rotation = new Quaternion(0,0,0,0);
        Invoke("SpawnChecker", 0.05f);
        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }
    void SpawnChecker()
    {
        //Sanity Spawn Checker
        if ((gameObject.transform.position - GameObject.Find("Player").transform.position).magnitude < 30.0f)
        {
            gameObject.transform.position = GameObject.Find("GameHandler").GetComponent<HordeHandler>().Spawners[0].transform.position;
            Invoke("SpawnChecker", 0.05f);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
            canAttack = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Attack Checker
        if ((transform.position - GetComponent<EnemyFollow>().GetTargetPos()).magnitude <= attackRange && canAttack == true && GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == false)
        {
            attackNum = 1;
            canAttack = false;
            attackEvent();
        }

        //Hunter Targeter
        if (specialAttack == "Hunter")
        {
            GetComponent<EnemyFollow>().SetTargetName(GameObject.Find("DistanceKeeper"));
        }
    }
    public void AttackEnd()
    {
        attackNum = 0;
        if (GetComponent<EnemyAnim>())
        {
            GetComponent<EnemyAnim>().attackType = attackNum;
        }
        if (HP > 0.0f)
        {
            Invoke("AttackEnable", attackCooldown);
            GetComponent<NavMeshAgent>().speed = Speed;
            GetComponent<EnemyFollow>().SetTarget();
        }
    }
    public void AttackEnable()
    {
        if (HP > 0.0f)
        {
            canAttack = true;
        }
    }
    void attackEvent()
    {
        Invoke("AttackEnd", attackCooldown);
        if (specialAttack == "")
        {
            Vector2 attackPos = GetComponent<EnemyFollow>().GetTargetPos();
            if (IsRanged)
            {
                attackPos = gameObject.transform.position;
            }
            attackNum = 1;
            GetComponent<EnemyAnim>().attackType = attackNum;
            GameObject newAttack = Instantiate(attackType);
            newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, attackPos, gameObject, IsRanged);
            GetComponent<NavMeshAgent>().speed = 0;
            GetComponent<EnemyFollow>().ClearTarget();
            if (IsRanged)
            {
                soundSystem.createSound(soundSystem.bowattack, gameObject.transform.position, true);
            }
            else
            {
                soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
            }
        }
        else if (specialAttack == "dash")
        {
            Vector2 attackPos = gameObject.transform.position;
            attackPos.y -= 1.0f;
            attackNum = 1;
            GetComponent<EnemyAnim>().attackType = attackNum;
            GameObject newAttack = Instantiate(attackType);
            newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, attackPos, gameObject, IsRanged);
            GetComponent<NavMeshAgent>().speed = 0;
            GetComponent<EnemyFollow>().ClearTarget();
            soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
        }
        else if (specialAttack == "split")
        {
            attackNum = 1;
            GetComponent<EnemyAnim>().attackType = attackNum;
            for (int i = 0; i < 3; i++)
            {
                Vector3 newPos;
                newPos.z = 0.0f;
                newPos.x = gameObject.transform.position.x + Random.Range(-0.5f, 0.5f);
                newPos.y = gameObject.transform.position.y + Random.Range(-0.5f, 0.5f);

                GameObject newAttack = Instantiate(attackType);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, newPos, gameObject, IsRanged);
            }
            soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
        }
        //Boss Enemies
        else if (specialAttack == "Vengeful")
        {
            //AxeSwing
            attackNum = Random.Range(1, 3);
            GetComponent<EnemyAnim>().attackType = attackNum;
            Debug.Log(attackNum);
            if (attackNum == 1)
            {
                GameObject newAttack = Instantiate(attackType);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, GetComponent<EnemyFollow>().GetTargetPos(), gameObject, IsRanged);
            }
            //AxeSlam
            else if (attackNum == 2 && (transform.position - GetComponent<EnemyFollow>().GetTargetPos()).magnitude < 7.0f)
            {
                GameObject newAttack = Instantiate(attackType2);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType2, 0.5f, new Vector2(10.0f, 10.0f), 50.0f, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2.0f), gameObject, IsRanged);
                GetComponent<NavMeshAgent>().speed = 0;
                GetComponent<EnemyFollow>().ClearTarget();
                soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
            }
            //AxeThrow
            else if (attackNum == 2)
            {
                GameObject newAttack = Instantiate(attackType3);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType3, 15.0f, new Vector2(1.0f, 1.0f), attackDamage, gameObject.transform.position, gameObject, true);
                soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
            }
        }
        else if (specialAttack == "Hunter")
        {
            attackNum = Random.Range(1, 100);
            Debug.Log(attackNum);

            //ArrowShot
            if (attackNum > 0 && attackNum < 60)
            {
                GetComponent<EnemyAnim>().attackType = 1;
                int randomAtk = Random.Range(1, 3);
                if (randomAtk == 1)
                {
                    GameObject newAttack = Instantiate(attackType);
                    newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, gameObject.transform.position, gameObject, IsRanged);
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 newPos;
                        newPos.z = 0.0f;
                        newPos.x = gameObject.transform.position.x + Random.Range(-1.0f, 1.0f);
                        newPos.y = gameObject.transform.position.y + Random.Range(-5.0f, 5.0f);

                        GameObject newAttack = Instantiate(attackType);
                        newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, attackSpeed, attackSize, attackDamage, newPos, gameObject, IsRanged);
                    }
                }
                soundSystem.createSound(soundSystem.bowattack, gameObject.transform.position, true);
            }
            //HolyWater
            else if (attackNum > 60 && attackNum < 85)
            {
                GetComponent<EnemyAnim>().attackType = 2;
                GameObject newAttack = Instantiate(attackType2);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType2, 17.0f, attackSize, attackDamage, gameObject.transform.position, gameObject, IsRanged);
                soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
            }
            //Beartrap
            else if (attackNum > 85)
            {
                GetComponent<EnemyAnim>().attackType = 3;
                GameObject newAttack = Instantiate(attackType3);
                newAttack.GetComponent<AttackHandler>().setUpAttack(attackType3, 0.0f, attackSize, 30.0f, gameObject.transform.position, gameObject, IsRanged);
                soundSystem.createSound(soundSystem.trapplace, gameObject.transform.position, true);
            }
        }
        else if (specialAttack == "Orion")
        {
            attackNum = Random.Range(1, 100);
            Debug.Log(attackNum);
            //Projectiles
            if (attackNum > 0 && attackNum < 35) //0-35
            {
                int randomAtk = Random.Range(1, 3);
                GetComponent<EnemyAnim>().attackType = 1;
                GetComponent<EnemyFollow>().ClearTarget();
                //Vertical
                if (randomAtk == 1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        float xpos = GameObject.Find("Player").transform.position.x-21 + i*6;
                        float ypos = GameObject.Find("Player").transform.position.y - 15;
                        if (i%2 != 0)
                        {
                            ypos = GameObject.Find("Player").transform.position.y + 15;
                        }
                        GameObject newAttack = Instantiate(attackType);
                        newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, 10.0f, new Vector2(1.0f, 1.0f), 10, new Vector2(xpos,ypos), gameObject, true);
                    }
                }
                //Horizontal
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        float xpos = GameObject.Find("Player").transform.position.x - 21;
                        float ypos = GameObject.Find("Player").transform.position.y - 20 + i * 2.8f;
                        if (i % 2 != 0)
                        {
                            xpos = GameObject.Find("Player").transform.position.x + 21;
                        }
                        GameObject newAttack = Instantiate(attackType);
                        newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, 10.0f, new Vector2(1.0f, 1.0f), 10, new Vector2(xpos, ypos), gameObject, true);
                    }
                }
                soundSystem.createSound(soundSystem.enemyswing, gameObject.transform.position, true);
            }
            //AOE
            else if (attackNum > 35 && attackNum < 70) //35-70
            {
                int randomAtk = Random.Range(1, 3);
                GetComponent<EnemyAnim>().attackType = 2;
                //Multiple
                if (randomAtk == 1)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        float xpos = GameObject.Find("Player").transform.position.x + Random.Range(-15,15);
                        float ypos = GameObject.Find("Player").transform.position.y + Random.Range(-15, 15);
                        GameObject newAttack = Instantiate(attackType2);
                        newAttack.GetComponent<AttackHandler>().setUpAttack(attackType2, 0.2f, new Vector2(5.0f, 5.0f), 25, new Vector2(xpos, ypos), gameObject, false);
                    }
                }
                //Large
                else
                {
                    GameObject newAttack = Instantiate(attackType2);
                    newAttack.GetComponent<AttackHandler>().setUpAttack(attackType2, 0.5f, new Vector2(30.0f, 30.0f), 25, GetComponent<EnemyFollow>().GetTargetPos(), gameObject, false);
                }
                soundSystem.createSound(soundSystem.cardpick, gameObject.transform.position, true);
            }
            //Spawn
            else if (attackNum > 70 && attackNum < 100) //70-100
            {
                int randomAtk = Random.Range(1, 4);
                GetComponent<EnemyAnim>().attackType = 3;
                //Boss
                if (randomAtk == 1)
                {
                    int random = Random.Range(1, GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableBosses.Length);
                    GameObject boss = GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableBosses[random];
                    Instantiate(boss);
                    List<GameObject> Spawners = GameObject.Find("GameHandler").GetComponent<HordeHandler>().Spawners;
                    boss.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
                }
                //Elites
                else if (randomAtk == 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int random = Random.Range(0, GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableEnemies.Length);
                        GameObject elite = GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableEnemies[random];
                        string[] eliteTypes = GameObject.Find("GameHandler").GetComponent<HordeHandler>().eliteTypes;
                        Instantiate(elite);
                        elite.GetComponent<EnemyHandler>().makeElite(eliteTypes[Random.Range(0, eliteTypes.Length)]);
                        List<GameObject> Spawners = GameObject.Find("GameHandler").GetComponent<HordeHandler>().Spawners;
                        elite.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
                    }
                }
                //Horde
                else
                {
                    for (int i = 0; i < 15; i++)
                    {
                        int random = Random.Range(0, GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableEnemies.Length);
                        GameObject enemy = GameObject.Find("GameHandler").GetComponent<HordeHandler>().SpawnableEnemies[random];
                        Instantiate(enemy);
                        List<GameObject> Spawners = GameObject.Find("GameHandler").GetComponent<HordeHandler>().Spawners;
                        enemy.transform.position = Spawners[Random.Range(0, Spawners.Count)].transform.position;
                    }
                }
                soundSystem.createSound(soundSystem.enemyhit, gameObject.transform.position, true);
            }
        }
    }
    public void subHP(float val)
    {
        soundSystem.createSound(soundSystem.enemyhit, gameObject.transform.position, true);
        HP -= val;
        StartCoroutine(HurtEvent());
        if (HP <= 0)
        {
            soundSystem.createSound(soundSystem.enemydie, gameObject.transform.position, true);
            GameObject.Find("GameHandler").GetComponent<HordeHandler>().removeEnemy(eliteClass);
            dieEvent();
        }
    }

    public void makeElite(string eliteType)
    {
        //Base Stats
        HP *= 2 + (GameObject.Find("GameHandler").GetComponent<HordeHandler>().curWave) / 2;
        Reward *= 2f;
        Vector2 sizeVec;
        sizeVec.x = 1.3f;
        sizeVec.y = 1.3f;
        transform.localScale *= sizeVec;
        eliteClass = eliteType;
        GameObject eliteIconRef = Instantiate(eliteIcon);
        eliteIconRef.GetComponent<Icon>().setParent(gameObject);

        //Specialized
        SpriteRenderer enemyColor = gameObject.GetComponent<SpriteRenderer>();
        enemyColor.color = gameObject.GetComponent<SpriteRenderer>().material.color;

        if (eliteType == "Blessed") //Blessed: Regenerates HP [White]
        {
            eliteIconRef.GetComponent<SpriteRenderer>().sprite = Icons[0];
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(enemyColor.color.r * 0.5f, enemyColor.color.g * 0.5f, enemyColor.color.b * 0.5f);
            HP *= 0.5f;
            StartCoroutine(Regen());
        }
        else if (eliteType == "Righteous") //Righteous: Aggressive and deals a lot of damage [Red]
        {
            eliteIconRef.GetComponent<SpriteRenderer>().sprite = Icons[1];
            attackDamage *= 1.5f;
            Speed *= 0.8f;
            attackCooldown *= 0.8f;
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(enemyColor.color.r * 0.9f, enemyColor.color.g * 0.3f, enemyColor.color.b * 0.3f);
        }
        else if (eliteType == "Protected") //Protected: Lots of HP [Blue]
        {
            eliteIconRef.GetComponent<SpriteRenderer>().sprite = Icons[2];
            HP *= 2.0f;
            attackCooldown *= 0.5f;
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(enemyColor.color.r * 0.3f, enemyColor.color.g * 0.3f, enemyColor.color.b * 0.9f);
        }
        else if (eliteType == "Paladin") //Paladin: Boosts enemies [Yellow]
        {
            eliteIconRef.GetComponent<SpriteRenderer>().sprite = Icons[3];
            HP *= 0.8f;
            Speed *= 0.8f;
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(enemyColor.color.r * 0.9f, enemyColor.color.g * 0.9f, enemyColor.color.b * 0.3f);
            InvokeRepeating("BoostEnemies", 1.0f, 1.0f);
        }
        else if (eliteType == "Resurrector") //Resurrector: Revives dead enemies [Green]
        {
            eliteIconRef.GetComponent<SpriteRenderer>().sprite = Icons[4];
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(enemyColor.color.r * 0.3f, enemyColor.color.g * 0.9f, enemyColor.color.b * 0.3f);
        }
    }
    //GetComponent<SpriteRenderer>().material = reviveMaterial;
    //Regenerate
    public IEnumerator Regen()
    {
        if (HP < HPCap && HP > 0.0f && baseMat)
        {
            if (bossClass == "")
            {
                HP += HPCap / 3;
            }
            else
            {
                HP += 10;
            }
            GetComponent<SpriteRenderer>().material = reviveMaterial;
            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().material = baseMat;
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Regen());
    }

    //Boost Nearby Enemies
    public void BoostEnemies()
    {
        if (HP > 0.0f)
        {
            GameObject[] allEnemies = FindObjectsOfType<GameObject>();
            foreach (GameObject enemy in allEnemies)
            {
                if ((transform.position - enemy.transform.position).magnitude < 10.0f)
                {
                    EnemyHandler enemyHandlerRef = enemy.GetComponent<EnemyHandler>();
                    if (enemyHandlerRef != null && enemyHandlerRef.eliteClass == "" && enemyHandlerRef.bossClass == "")
                    {
                        enemyHandlerRef.GiveBoost();
                    }
                }
            }
        }
    }

    public void GiveBoost()
    {
        if (isBoosted == false)
        {
            isBoosted = true;
            HP *= 1.3f;
            attackDamage *= 1.3f;
            gameObject.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 0.7f);
        }
    }

    void dieEvent()
    {
        GameObject.Find("GameHandler").GetComponent<GameLogic>().deadEnemies.Add(gameObject);
        canAttack = false;
        if (alreadyDied == false)
        {
            GameObject.Find("Player").GetComponent<PlayerController>().playerCombo += 1;
            GameObject.Find("Player").GetComponent<PlayerController>().subBlood(-1 * Reward);
        }
        GameObject.Find("GameHandler").GetComponent<GameLogic>().addScore(Convert.ToInt32(Reward) * GameObject.Find("Player").GetComponent<PlayerController>().playerCombo);
        GetComponent<NavMeshAgent>().speed = 0;
        GetComponent<NavMeshAgent>().enabled = false;
        if (!(bossClass == "") && alreadyDied == false)
        {
            GameObject.Find("GameHandler").GetComponent<CardShuffler>().MoveCards = true;
            GameObject.Find("GameHandler").GetComponent<CardShuffler>().DisplayCards = 3;
            GameObject.Find("GameHandler").GetComponent<HordeHandler>().maximumEnemyCount = 0;
            GameObject.Find("GameHandler").GetComponent<MusicHandler>().ChangeMusic(0);
        }
        if (eliteClass == "Resurrector" && alreadyDied == false)
        {
            GameObject[] allEnemies = FindObjectsOfType<GameObject>();
            foreach (GameObject enemy in allEnemies)
            {
                if ((transform.position - enemy.transform.position).magnitude < 5.0f)
                {
                    EnemyHandler enemyHandlerRef = enemy.GetComponent<EnemyHandler>();
                    if (enemyHandlerRef != null && enemyHandlerRef.HP <= 0.0f)
                    {
                        StartCoroutine(enemyHandlerRef.Resurrect());
                    }
                }
            }
        }
        alreadyDied = true;
    }

    public IEnumerator Resurrect()
    {
        Debug.Log("I have resurrected.");
        if (bossClass == "" && eliteClass != "Resurrector" && tag == "Character")
        {
            GetComponent<SpriteRenderer>().material = reviveMaterial;
            GetComponent<NavMeshAgent>().enabled = true;
            GameObject.Find("GameHandler").GetComponent<GameLogic>().deadEnemies.Remove(gameObject);
            canAttack = true;
            Color baseColor = GetComponent<SpriteRenderer>().color;
            GetComponent<NavMeshAgent>().speed = Speed;
            GetComponent<EnemyFollow>().SetTarget();
            HP = HPCap / 2;

            yield return new WaitForSeconds(0.5f);

            if (GetComponent<EnemyHandler>())
            {
                GetComponent<SpriteRenderer>().material = baseMat;
                GetComponent<SpriteRenderer>().color = baseColor;
            }
        }
    }

    public IEnumerator DecayHide()
    {
        if (GetComponent<SpriteRenderer>().color.a <= 0.0f)
        {
            Destroy(gameObject);
        }
        tag = "Untagged";
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.a - 0.01f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(DecayHide());
    }

    public IEnumerator HurtEvent()
    {
        GetComponent<SpriteRenderer>().material = hurtMaterial;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().material = baseMat;
    }
}
