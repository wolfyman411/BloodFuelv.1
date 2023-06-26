using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using Color = UnityEngine.Color;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AttackHandler : MonoBehaviour
{
    public SpriteRenderer shape;
    public float speed;
    public Vector2 size;
    public float damage;
    public GameObject copySelf;
    public Vector2 Attackparent;
    public Vector2 baseSize;
    public GameObject AttackParentRef;
    public bool rangedAttack;
    bool playerTouching;
    public Vector2 direction;

    soundhandler soundSystem;
    // Start is called before the first frame update
    void Start()
    {
        soundSystem = GameObject.Find("AudioReference").GetComponent<soundhandler>();
        direction = (GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget.transform.position - transform.position).normalized;
        if (rangedAttack == true)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void Update()
    {
        if (rangedAttack == false)
        {
            if (copySelf != null)
            {
                if (copySelf.transform.localScale.x > gameObject.transform.localScale.x)
                {   
                    if (playerTouching == true)
                    {
                        //Do Half, because both deal damage.
                        if (Random.Range(1.0f, 100.0f) > GameObject.Find("Player").GetComponent<PlayerController>().curDodgeChance)
                        {
                            soundSystem.createSound(soundSystem.playerhurt, gameObject.transform.position, false);
                            GameObject.Find("Main Camera").GetComponent<follower>().ShakeCamera(damage/100.0f, damage / 100.0f);
                            GameObject.Find("Player").GetComponent<PlayerController>().subBlood(damage / 2);
                            GameObject.Find("Player").GetComponent<PlayerController>().playerHurt = true;
                            GameObject.Find("Player").GetComponent<PlayerController>().bleedOutRate -= 0.1f;
                        }
                    }
                    Destroy(gameObject);
                    Destroy(copySelf);
                }
            }
        }
        else
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            if ((transform.position - GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget.transform.position).magnitude > 25.0f)
            {
                Destroy(gameObject);
            }
            if ((transform.position - GameObject.Find("Player").transform.position).magnitude < 1.0f)
            {
                //Deal Damage
                if (Random.Range(1.0f, 100.0f) > GameObject.Find("Player").GetComponent<PlayerController>().curDodgeChance)
                {
                    soundSystem.createSound(soundSystem.playerhurt, gameObject.transform.position, false);
                    GameObject.Find("Main Camera").GetComponent<follower>().ShakeCamera(damage / 100.0f, damage / 100.0f);
                    GameObject.Find("Player").GetComponent<PlayerController>().subBlood(damage);
                    GameObject.Find("Player").GetComponent<PlayerController>().playerHurt = true;
                    GameObject.Find("Player").GetComponent<PlayerController>().bleedOutRate -= 0.1f;
                }
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    public void updateAttack()
    {
        if (AttackParentRef != null && rangedAttack == false && AttackParentRef.GetComponent<EnemyHandler>())
        {
            if (AttackParentRef != null && AttackParentRef.GetComponent<EnemyHandler>().HP > 0.0f)
            {
                copySelf.transform.localScale = new Vector2(copySelf.transform.localScale.x + speed, copySelf.transform.localScale.y + speed);
            }
            else
            {
                Destroy(copySelf);
                Destroy(gameObject);
            }
        }
        else
        {
            copySelf.transform.localScale = new Vector2(copySelf.transform.localScale.x + speed, copySelf.transform.localScale.y + speed);
        }
    }

    public void setUpAttack(GameObject newShape, float newSpeed, Vector2 newSize, float newDamage, Vector2 parentPos, GameObject parent, bool isRanged)
    {
        speed=newSpeed;
        size=newSize;
        damage=newDamage;
        Attackparent = parentPos;
        baseSize = newSize;
        AttackParentRef = parent;
        rangedAttack = isRanged;

        if (shape != null && rangedAttack == false)
        {
            shape.sprite = newShape.GetComponent<SpriteRenderer>().sprite;
            gameObject.transform.position = Attackparent;
            shape.transform.localScale = size;
            shape.color = new Color(shape.color.r, shape.color.g, shape.color.b, 0.3f);
            copySelf = gameObject;
            copySelf = Instantiate(copySelf);
            copySelf.GetComponent<SpriteRenderer>().size = new Vector2(0.1f, 0.1f);
            copySelf.GetComponent<SpriteRenderer>().sortingOrder = -900;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = -901;
            copySelf.transform.localScale = new Vector2(0.0f, 0.0f);
            copySelf.GetComponent<SpriteRenderer>().color = new Color(shape.color.r, shape.color.g, shape.color.b, 0.9f);
            InvokeRepeating("updateAttack", 0.05f, 0.05f);
        }
        else if (shape != null && rangedAttack == true)
        {
            //direction = (GameObject.Find("Player").transform.position - transform.position).normalized;
            gameObject.transform.position = Attackparent;
            shape.transform.localScale = size;
            shape.color = new Color(shape.color.r, shape.color.g, shape.color.b, 1.0f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 999;
        }
    }

    //Player Checkers
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTouching = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTouching = false;
        }
    }
}
