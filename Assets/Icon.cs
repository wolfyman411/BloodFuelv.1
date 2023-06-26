using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    GameObject followParent;
    // Start is called before the first frame update
    public void setParent(GameObject parent)
    {
        this.followParent = parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (followParent)
        {
            transform.position = new Vector2 (followParent.transform.position.x - 1.0f, followParent.transform.position.y+1.0f);
            GetComponent<SpriteRenderer>().sortingOrder = followParent.GetComponent<SpriteRenderer>().sortingOrder+1;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
