using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP player;

    public bool playerIsDead;
    private float HP = 100;
    [SerializeField] bool firstLevel;
    [SerializeField] GameObject[] restartMenu;
    [SerializeField] Image hpbar;

    private void Start()
    {
        player = this;
    }

    void Update()
    {
        if (HP <= 0)
        {
            playerIsDead = true;
            if (firstLevel)
            {
                StartCoroutine(GameManager.gameManager.EndLevel());
            }
            else
            {
                foreach (GameObject obj in restartMenu)
                {
                    obj.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0;
                }
            }
        }
        else
        {
            playerIsDead = false;
            if (!Menu.isPaused)
            {
                foreach (GameObject obj in restartMenu)
                {
                    obj.SetActive(false);
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        hpbar.fillAmount = HP * 0.01f;
    }

    public void GetDamage(float damage)
    {
        HP -= damage;
        Debug.Log(HP);
        SoundManager.soundManager.PlaySound(17);
    }
}
