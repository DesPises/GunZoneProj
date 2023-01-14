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
            if (index < dialogues.Length)
                NextPhrase();
        }
        else if (index >= dialogues.Length && !blackScreen)
        {
            blackScreen = true;
            StartCoroutine(NextScene());
        }

        // Cutscenes special sequences

        if (cutsceneID == 0 && index == 1)
        {
            backgrounds[0].SetActive(false);
        }
        else if (cutsceneID == 2)
        {
            if (index == 1)
            {
                cowboyEnd[0].SetActive(true);
                cowboyEnd[1].SetActive(false);
            }
            else if (index == 2)
            {
                cowboyEnd[0].SetActive(false);
                cowboyEnd[1].SetActive(true);
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
        TextMeshPro[] names = dialogues[index].GetComponentsInParent<TextMeshPro>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = true;
        }
        foreach (TextMeshPro name in names)
        {
            name.enabled = true;
        }
    }

    void DisableAll()
    {
        for (int i = 0; i < dialogues.Length; i++)
        {
            dialogues[i].SetActive(false);
            dialogues[i].GetComponentInParent<SpriteRenderer>().enabled = false;
            dialogues[i].GetComponentInParent<TextMeshPro>().enabled = false;
        }
    }

    void ResetCooldown()
    {
        cooldown = false;
    }

    IEnumerator NextScene()
    {
        Instantiate(blackScreenExit, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(2f);
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
                canvas.SetActive(false);
                break;
            case 1:
                backgrounds[0].SetActive(false);
                backgrounds[1].SetActive(false);
                backgrounds[2].SetActive(true);
                backgrounds[3].SetActive(true);
                break;
            case 2:
                backgrounds[0].SetActive(false);
                backgrounds[1].SetActive(false);
                backgrounds[2].SetActive(false);
                backgrounds[3].SetActive(true);
                break;
        }
    }

    IEnumerator SmoothTransition()
    {
        Instantiate(blackScreenEntry, Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Instantiate(blackScreenExit, Vector3.zero, Quaternion.identity);
    }
}
