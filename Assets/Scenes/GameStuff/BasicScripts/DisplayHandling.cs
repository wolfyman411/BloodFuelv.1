using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DisplayHandling : MonoBehaviour
{
    public SpriteRenderer selfSprite;
    // Start is called before the first frame update
    void Start()
    {
        selfSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        selfSprite.sortingOrder = -1*(int)selfSprite.transform.position.y;
    }
}
