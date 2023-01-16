using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSAnimationsHelper : MonoBehaviour
{
    [SerializeField] private GameObject[] tvchannels;
    private int currentChannel = 0;

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ChannelSwitch()
    {
        if (tvchannels.Length > 0)
        {
            foreach (GameObject channel in tvchannels)
            {
                channel.SetActive(false);
            }
            if (currentChannel < tvchannels.Length - 1)
            {
                currentChannel++;
            }
            else
            {
                currentChannel = 0;
            }
            tvchannels[currentChannel].SetActive(true);
        }
    }
    public void PlayOST(int i)
    {
        SoundManager.soundManager.PlayOST(i);
    }

    public void PlaySound(int i)
    {
        SoundManager.soundManager.PlaySound(i);
    }
    public void PlayDialogue(int i)
    {
        SoundManager.soundManager.PlayDialogue(i);
    }

    public void StopAllSFX()
    {
        SoundManager.soundManager.StopAllSFX();
    }
}
