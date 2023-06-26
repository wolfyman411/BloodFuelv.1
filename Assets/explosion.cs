using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    public float lifeTime = 2.0f;
    float aliveFor = 0.0f;
    public float damage = 15.0f;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        InvokeRepeating("count", 0.1f, 0.1f);
    }

    void count()
    {
        aliveFor += 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f, 1.0f,(lifeTime - aliveFor)/lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character") && aliveFor < 0.2f && other.GetComponent<EnemyHandler>().HP > 0.0f)
        {
            other.GetComponentInParent<EnemyHandler>().subHP(damage * playerController.damageAdditive);
        }
    }
}
