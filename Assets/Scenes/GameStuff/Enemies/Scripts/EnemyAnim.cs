using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] SpriteRenderer selfSprite;
    public float playerX;
    public float selfX;
    public int attackType = 0;

    private Animator anim;
    private string DEATH_ANIMATION = "Dead";
    private string ATTACK_ANIMATION = "AttackNum";
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        selfSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<EnemyHandler>().HP > 0.0f)
        {
            if (GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget.transform.position.x < selfSprite.transform.position.x)
            {
                selfSprite.flipX = false;
            }
            else
                selfSprite.flipX = true;
        }

        playerX = player.transform.position.x;
        selfX = selfSprite.transform.position.x;

        //HP
        if (GetComponent<EnemyHandler>().HP <= 0.0f && anim)
        {
            anim.SetBool(DEATH_ANIMATION, true);
        }
        else
        {
            anim.SetBool(DEATH_ANIMATION, false);
        }

        //Attack
        if (anim)
        {
            anim.SetInteger(ATTACK_ANIMATION, attackType);
        }
    }

}
