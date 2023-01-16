using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private GameObject blackScreenEntry;
    [SerializeField] private GameObject blackScreenExit;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject blackScreenObj;
    [SerializeField] private GameObject sanchez;

    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private GameObject[] cowboyEnd;
    [SerializeField] private GameObject[] tvchannels;
    private int currentChannel = 0;

    [SerializeField] private int cutsceneID = 0;
    private int index;
    private bool cooldown;
    private bool blockCooldown;
    private readonly float cooldownTime = 1f;
    private bool blackScreen;
    [SerializeField] private Animator anim;

    void Start()
    {
        index = 0;
        StartBackground();
        Instantiate(blackScreenEntry, Vector3.zero, Quaternion.identity);
        NextPhrase();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !cooldown)
        {
            index++;
            Debug.Log("index = " + index);
            if (index < dialogues.Length)
                NextPhrase();
        }
        else if (index >= dialogues.Length && !blackScreen)
        {
            blackScreen = true;
            StartCoroutine(NextScene());
        }

        // Cutscenes special sequences

        if (cutsceneID == 0) // CS_1
        {
            if (index == 0)
            {
                blackScreenObj.SetActive(true);
            }

            if (index == 2 && !blackScreen)
            {
                anim.Play("1_1entry");
                StartCoroutine(GetInBar());
                blackScreen = true;
            }
        }
        else if (cutsceneID == 1) // CS_2
        {

        }
        else if (cutsceneID == 2) // CS_3
        {
            if (index == 8)
            {
                sanchez.SetActive(true);
            }
            else if (index == 13)
            {
                anim.Play("3_2imagination");
            }


        }
        else if (cutsceneID == 3) // CS_4
        {
            if (index == 13)
            {
                anim.Play("4_1imagination");
            }
            if (index == 14)
            {
                anim.Play("4_2imagination");
            }

        }
        else if (cutsceneID == 4) // CS_5
        {
            if (index == 1 && !blackScreen)
            {
                anim.Play("5_1whatisthis");
                StartCoroutine(GetInBobik());
                blackScreen = true;
            }

        }
        else if (cutsceneID == 5) // CS_6
        {
            if (index == 12 && !blackScreen)
            {
                blackScreen = true;
                StartCoroutine(GetInZakat());
                //if ()
                //{
                //    blackScreen = false;
                //}
            }
            else if (index == 14)
            {
                anim.Play("6_2Zakat");
            }
            else if (index == 16)
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentScene + 1);
            }
        }
    }

    void NextPhrase()
    {
        cooldown = true;
        Invoke(nameof(ResetCooldown), cooldownTime);
        DisableAll();
        dialogues[index].SetActive(true);
        SpriteRenderer[] sprites = dialogues[index].GetComponentsInParent<SpriteRenderer>();
        TextMeshProUGUI[] names = dialogues[index].GetComponentsInParent<TextMeshProUGUI>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = true;
        }
        foreach (TextMeshProUGUI name in names)
        {
            name.enabled = true;
        }
    }

    void DisableAll()
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            dialogues[i].SetActive(false);

            SpriteRenderer[] sprites = dialogues[i].GetComponentsInParent<SpriteRenderer>();
            TextMeshProUGUI[] names = dialogues[i].GetComponentsInParent<TextMeshProUGUI>();
            if (sprites.Length > 0)
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    sprite.enabled = false;
                }
            }
            if (names.Length > 0)
            {
                foreach (TextMeshProUGUI name in names)
                {
                    name.enabled = false;
                }
            }
        }
    }

    void ResetCooldown()
    {
        if (!blockCooldown)
            cooldown = false;
    }

    IEnumerator NextScene()
    {
        Instantiate(blackScreenExit, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(1.9f);
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    void StartBackground()
    {
        switch (cutsceneID)
        {
            case 0:
                backgrounds[0].SetActive(true);
                backgrounds[1].SetActive(true);
                backgrounds[2].SetActive(true);
                backgrounds[3].SetActive(true);
                dialogueBox.SetActive(false);
                break;
            case 5:
                backgrounds[0].SetActive(false);
                backgrounds[1].SetActive(false);
                backgrounds[2].SetActive(true);
                backgrounds[3].SetActive(true);
                break;
            default:
                backgrounds[0].SetActive(false);
                backgrounds[1].SetActive(true);
                backgrounds[2].SetActive(true);
                backgrounds[3].SetActive(true);
                break;
        }
    }

    IEnumerator SmoothTransition()
    {
        Instantiate(blackScreenExit, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Instantiate(blackScreenEntry, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        blackScreen = false;
    }

    IEnumerator GetInBar()
    {
        LockCooldown();
        yield return new WaitForSeconds(10.5f);
        StartCoroutine(SmoothTransition());
        yield return new WaitForSeconds(2f);
        anim.Play("idleBar");
        backgrounds[0].SetActive(false);
        UnlockCooldown();
        index++;
        NextPhrase();
        dialogueBox.SetActive(true);
        SoundManager.soundManager.PlayOST(2);
    }

    void LockCooldown()
    {
        cooldown = true;
        blockCooldown = true;
    }

    void UnlockCooldown()
    {
        cooldown = false;
        blockCooldown = false;
    }

    IEnumerator GetInBobik()
    {
        LockCooldown();
        yield return new WaitForSeconds(5.5f);
        backgrounds[1].SetActive(false);
        backgrounds[1].SetActive(true);
        UnlockCooldown();
        index++;
        NextPhrase();
        blackScreen = false;
        yield return new WaitForSeconds(0.5f);
        anim.Play("5_2inbobik");
    }

    IEnumerator GetInZakat()
    {
        LockCooldown();
        StartCoroutine(SmoothTransition());
        yield return new WaitForSeconds(2f);
        anim.Play("6_1zakat");
        yield return new WaitForSeconds(3f);
        blackScreen = true;
        UnlockCooldown();
        yield return new WaitForSeconds(5f);
    }

    public void ChannelSwitch()
    {
        foreach (GameObject channel in tvchannels)
        {
            channel.SetActive(false);
        }
        if (currentChannel < tvchannels.Length)
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
