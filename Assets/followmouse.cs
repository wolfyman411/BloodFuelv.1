using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followmouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mousePos;
        //mousePos.x = Input.mousePosition.x/43-10;
        //mousePos.y = Input.mousePosition.y/43-5;
        //mousePos.z = 0.0f;
        //transform.position = mousePos;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;
        transform.position = worldPosition;
    }
}
