using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundhandler : MonoBehaviour
{

    public AudioSource newSound;
    public List<GameObject> createdSounds;
    //Player
    public AudioClip ability;
    public AudioClip boon;
    public AudioClip heavyattack;
    public AudioClip playerattack;
    public AudioClip playerdead;
    public AudioClip playerhurt;
    public AudioClip playerhurtheavy;
    public AudioClip explosion;

    //Enemy
    public AudioClip bowattack;
    public AudioClip enemydie;
    public AudioClip enemyhit;
    public AudioClip enemyhurt;
    public AudioClip enemyswing;
    public AudioClip trapplace;

    //UI
    public AudioClip carddraw;
    public AudioClip cardpick;

    public void createSound(AudioClip clipRef, Vector3 pos, bool randomPitch)
    {
        AudioSource soundRef = Instantiate(newSound);
        soundRef.transform.position = pos;
        soundRef.clip = clipRef;
        if (randomPitch)
        {
            soundRef.pitch *= Random.Range(0.7f, 1.3f);
        }
        soundRef.Play();
        createdSounds.Add(soundRef.gameObject);
        Invoke("removeSound", 5.0f);
    }

    void removeSound()
    {
        Destroy(createdSounds[0]);
        createdSounds.RemoveAt(0);
    }
}
