using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUDCardHandler : MonoBehaviour
{
    int UILayer;
    public float hoverHeight = 1f; // Desired height above the original position
    public float moveSpeed = 5f; // Speed of the object's movement

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        hoverHeight = GameObject.Find("CardDes").transform.position.y;
        UILayer = LayerMask.NameToLayer("CardLayer");
        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    private void Update()
    {
        if (IsPointerOverUIElement() == true)
        {
            targetPosition = new Vector3(targetPosition.x, hoverHeight);
            StartCoroutine(MoveObject());
        }
        else
        {
            targetPosition = originalPosition;
            StartCoroutine(MoveObject());
        }
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    private IEnumerator MoveObject()
    {
        while (transform.position != targetPosition)
        {
            // Move the object towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
