using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Remove", 10.0f);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
