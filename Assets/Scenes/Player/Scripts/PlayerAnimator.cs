using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimator : MonoBehaviour
{
    //Anim Vars
    private Animator anim;
    private string WALK_ANIMATION = "IsWalking?";
    private string CARD_ACTIVATE = "ActivatedCard?";
    private string LIGHT_ATTACK = "LightAttack";
    private string DEATH_ANIMATION = "IsDead";
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger(LIGHT_ATTACK, 0);
    }

    // Update is called once per frame
    void Update()
    {
        AnimatePlayer();
        if (GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == true)
        {
            anim.SetBool(DEATH_ANIMATION, true);
        }
    }

    void AnimatePlayer()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //Direction Checker
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Flipper
        if (GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == false)
        {
            if (h > 0.5)
            {
                spriteRenderer.flipX = true;
            }
            else if (h < -0.5)
            {
                spriteRenderer.flipX = false;
            }
        }

        //Walk Anim
        if (v > 0.5 || v < -0.5)
        {
            anim.SetBool(WALK_ANIMATION, true);
        }
        else if (h > 0.5 || h < -0.5)
        {
            anim.SetBool(WALK_ANIMATION, true);
        }
        else
        {
            anim.SetBool(WALK_ANIMATION, false);
        }
    }

    public void ActivatedCard(string type, bool variant)
    {
        anim.SetBool(CARD_ACTIVATE, true);
        if (type == "Attack" && variant == false)
        {
            anim.SetInteger(LIGHT_ATTACK, 0);
            anim.SetInteger(LIGHT_ATTACK, 1);
            Invoke("StopAnim", 0.5f);
        }
        else if (type == "Attack" && variant)
        {
            anim.SetInteger(LIGHT_ATTACK, 0);
            anim.SetInteger(LIGHT_ATTACK, 2);
            Invoke("StopAnim", 1.0f);
        }

        if (type == "Ability" && variant)
        {
            anim.SetInteger(LIGHT_ATTACK, 0);
            anim.SetInteger(LIGHT_ATTACK, 3);
            Invoke("StopAnim", 1.0f);
        }
        else if (type == "Ability" && variant == false)
        {
            anim.SetInteger(LIGHT_ATTACK, 0);
            anim.SetInteger(LIGHT_ATTACK, 4);
            Invoke("StopAnim", 1.0f);
        }
    }

    public void StopAnim()
    {
        anim.SetInteger(LIGHT_ATTACK, 0);
        anim.SetBool(CARD_ACTIVATE, false);
    }
}
