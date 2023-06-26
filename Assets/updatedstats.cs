using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updatedstats : MonoBehaviour
{
    public static float upgradeSpeed;
    public static float upgradeBloodFuel;
    public static float upgradeDamage;
    public static int upgradeRevive;
    public static float upgradeDodge;
    public static bool OrionBeaten;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController pc = GetComponent<PlayerController>();
        pc.speed += upgradeSpeed;
        pc.MaxBloodFuel += upgradeBloodFuel;
        pc.baseDamageAdditive += upgradeDamage;
        pc.numRevives += upgradeRevive;
        pc.baseDodgeChance += upgradeDodge;
        pc.BloodFuel = pc.MaxBloodFuel;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
