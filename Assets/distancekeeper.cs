using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class distancekeeper : MonoBehaviour
{
    public Transform player;
    public float distance = 1000f;
    public float smoothSpeed = 5f;

    private void Start()
    {
        Invoke("FlipSides",Random.Range(5.0f,10.0f));
    }

    void FlipSides()
    {
        gameObject.transform.position *= -1;
        Invoke("FlipSides", Random.Range(5.0f, 10.0f));
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("Player").transform;
        // Find the player distance
        Vector3 direction = transform.position - player.position;
        direction.Normalize();

        // Add distance to the object
        Vector3 targetPosition = player.position + direction * distance;
        targetPosition.z = transform.position.z;

        // Move slowly
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Sanity Checker
        ClampPositionToScreen();
    }

    private void ClampPositionToScreen()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).x, Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, Camera.main.ScreenToWorldPoint(Vector3.zero).y, Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y);
        transform.position = clampedPosition;
    }
}
