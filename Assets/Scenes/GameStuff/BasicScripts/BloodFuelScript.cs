using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodFuelScript : MonoBehaviour
{
    public float maxBloodFuel = 100;
    public float curBloodFuel;
    [SerializeField] GameObject player;
    PlayerController playerController;

    public Text bloodFuelDis;

    [ContextMenu("Change BloodFuel")]
    public void UpdateText(string newText)
    {
        bloodFuelDis.text = newText+"%";
    }

}
