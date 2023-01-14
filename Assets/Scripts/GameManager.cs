using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float cooldown = 5f;
    private float multiplier = 0.9f; // each wave subtract 10% from cooldown
    [SerializeField] private Transform[] points;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemiesInWave = 2;
    [SerializeField] private int wavesRemaining = 10;
    private int enemiesCounter;
    private bool cooldownBool;
    [SerializeField] GameObject blackScreenEntry;
    [SerializeField] GameObject blackScreenExit;

    void Start()
    {
        blackScreenEntry.SetActive(true);
        StartCoroutine(SpawnWave(1f));
    }

    void Update()
    {
        enemiesCounter = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemiesCounter == 0 && wavesRemaining <= 0)
        {
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator SpawnWave(float cooldownF)
    {
        yield return new WaitForSeconds(cooldownF);
        cooldown *= multiplier;
        wavesRemaining--;
        for (int i = 0; i < enemiesInWave; i++)
        {
            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            int randomPointIndex = Random.Range(0, points.Length);
            Instantiate(enemyPrefabs[randomEnemyIndex], points[randomPointIndex].position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }
        if (wavesRemaining > 0)
            StartCoroutine(SpawnWave(cooldown));
    }

    public IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(1f);
        if (blackScreenExit)
            blackScreenExit.SetActive(true);
        yield return new WaitForSeconds(2f);
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }
}
