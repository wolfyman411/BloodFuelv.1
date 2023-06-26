using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MusicHandler : MonoBehaviour
{
    public AudioSource gameTrack;
    public AudioSource gameTrackDrums;
    public PlayerController playerController;
    public AudioSource OrionMarch;
    public AudioSource OrionMarchDrums;
    public AudioSource OrionChaos;
    public AudioSource OrionChaosDrums;
    public AudioClip[] bossTrack;
    public AudioClip[] bossDrums;
    public Camera camera;
    // Start is called before the first frame update
    // Update is called once per frame
    public bool isWave10;
    public int Chaos = 1;
    void Update()
    {
        gameTrack.volume = playerController.getBloodFuel()/100;
        gameTrackDrums.volume = (playerController.getMaxBloodFuel()-playerController.getBloodFuel())/100;
        if (isWave10)
        {
            gameTrack.Stop();
            gameTrackDrums.Stop();
            if (OrionMarchDrums.isPlaying == false && playerController.BloodFuel > 0)
            {
                InvokeRepeating("SwitchaRoo", 0.5f, 0.5f);
                OrionMarch.Play();
                OrionMarchDrums.Play();
                OrionChaos.Play();
                OrionChaosDrums.Play();
            }
            if (Chaos == 1)
            {
                OrionMarch.volume = playerController.getBloodFuel() / 100;
                OrionMarchDrums.volume = (playerController.getMaxBloodFuel() - playerController.getBloodFuel()) / 100;
                OrionChaos.volume = 0.0f;
                OrionChaosDrums.volume = 0.0f;
            }
            else if (Chaos == -1)
            {
                OrionChaos.volume = playerController.getBloodFuel() / 100;
                OrionChaosDrums.volume = (playerController.getMaxBloodFuel() - playerController.getBloodFuel()) / 100;
                OrionMarch.volume = 0.0f;
                OrionMarchDrums.volume = 0.0f;
            }
        }
    }
    public void SwitchaRoo()
    {
        int randomint = Random.Range(1,100);
        if (randomint > 80 && playerController.BloodFuel > 0 && GetComponent<HordeHandler>().aliveBossEnemies > 0)
        {
            Chaos *= -1;
            // Get the camera's current projection matrix
            Matrix4x4 projectionMatrix = Camera.main.projectionMatrix;

            // Apply a horizontal flip to the projection matrix
            projectionMatrix *= Matrix4x4.Scale(new Vector3(-1, 1, 1));

            // Update the camera's projection matrix
            Camera.main.projectionMatrix = projectionMatrix;
        }
    }

    public void ChangeMusic(int trackNum)
    {
        gameTrack.clip = bossTrack[trackNum];
        gameTrackDrums.clip = bossTrack[trackNum];
        gameTrack.Play();
        gameTrackDrums.Play();
    }
}
