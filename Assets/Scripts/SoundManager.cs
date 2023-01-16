using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    public AudioSource[] ost;
    public AudioSource[] sound;
    public AudioSource[] dialogue;
    public int sceneID;

    private void Start()
    {
        switch (sceneID)
        {
            case -1:
                break;
            case 0:
                PlayOST(0);
                break;
            case 2:
                PlaySound(9);
                break;
            case 4:
                PlayOST(3);
                break;
            case 6:
                PlayOST(4);
                break;
            case 8:
                PlayOST(5);
                break;
            case 10:
                PlayOST(6);
                break;
            case 11:
                PlaySound(11);
                break;
            case 12:
                PlayOST(7);
                break;

            default:
                PlayOST(2);
                break;
        }
    }

    void Awake()
    {
        soundManager = this;
    }

    public void PlayOST(int i)
    {
        ost[i].Play();
    }

    public void PlaySound(int i)
    {
        sound[i].Play();
    }

    public void PlayDialogue(int i)
    {
        dialogue[i].Play();
    }

    public void StopAllSFX()
    {
        foreach (AudioSource clip in ost)
        {
            clip.Stop();
        }
        foreach (AudioSource clip in dialogue)
        {
            clip.Stop();
        }
        foreach (AudioSource clip in sound)
        {
            clip.Stop();
        }
    }
}
