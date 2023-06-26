using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explode : MonoBehaviour
{
    public Material reviveMaterial;
    public GameObject attackType;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Explode", 1.0f);
        Invoke("Flash", 0.5f);
        Invoke("Remove", 10.0f);
    }

    void Flash()
    {
        GetComponent<SpriteRenderer>().material = reviveMaterial;
    }
    void Explode()
    {
        if (gameObject)
        {
            GameObject newAttack = Instantiate(attackType);
            newAttack.GetComponent<AttackHandler>().setUpAttack(attackType, 1.0f, new Vector2(10, 10), 35, gameObject.transform.position, gameObject, false);
            gameObject.transform.localScale = Vector3.zero;
        }
    }

    void Remove()
    {
        Destroy(gameObject);
    }
}
