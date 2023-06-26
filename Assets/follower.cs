using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follower : MonoBehaviour
{
    public Vector3 playerPos;
    Vector3 startPos;

    Vector3 offset = new Vector3(0f,0f, 0f);
    float smoothTime = 0.25f;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GameObject.Find("Player").transform.position;
        Vector3 targetPosition = playerPos + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void ShakeCamera(float shakeAmount, float duration)
    {
        startPos = transform.position;
        StartCoroutine(ShakeRoutine(shakeAmount, duration));
    }

    private IEnumerator ShakeRoutine(float shakeAmount, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            transform.position = playerPos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after shaking
        transform.position = playerPos;
    }
}
