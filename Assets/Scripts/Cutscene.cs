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

    [SerializeField] private GameObject[] dialogues;
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private GameObject[] cowboyEnd;
    [SerializeField] private int cutsceneID = 0;
    private int index = 0;
    private bool cooldown;
    private bool blockCooldown;
    private readonly float cooldownTime = 1f;
    private bool blackScreen;

    void Start()
    {
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

        if (cutsceneID == 0 && index == 2 && !blackScreen)
        {
            StartCoroutine(GetInBar());
            blackScreen = true;
        }
        else if (cutsceneID == 2)
        {

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
        //add animation
        yield return new WaitForSeconds(3f);
        StartCoroutine(SmoothTransition());
        yield return new WaitForSeconds(2f);
        backgrounds[0].SetActive(false);
        UnlockCooldown();
        index++;
        NextPhrase();
        dialogueBox.SetActive(true);
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
}
