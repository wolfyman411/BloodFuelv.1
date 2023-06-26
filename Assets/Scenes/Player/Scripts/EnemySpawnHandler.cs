using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class EnemySpawnHandler : MonoBehaviour
{
    void Update()
    {
        float distance = (GameObject.Find("Player").transform.position - transform.position).magnitude;
        if (distance < 25.0f)
        {
            tag = "Untagged";
        }
        else
        {
            tag = "Respawn";
        }
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,0);
    }
}
