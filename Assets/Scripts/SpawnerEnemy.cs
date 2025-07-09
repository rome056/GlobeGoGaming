using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnerEnemy : MonoBehaviour
{
    [Header("----- Enemy Prefabs -----")]
    //public List<GameObject> enemies = new List<GameObject>();
    public GameObject[] enemies;

    [Header("----- Spawner Points -----")]
    //public List<Transform> spawner = new List<Transform>();
    public Transform[] spawner;

    [Header("----- Spawn Settings -----")]
    public float spawnInterval = 3f;
    public float spawnDecrement = 0.2f;
    public float limitDecrement = 1f;

    [Header("----- Wave Settings -----")]
    public int startingEnemiesPerWave = 10;
    public int enemyIncrementPerWave = 5;

    private int currentWave = 1;
    private int enemiesToSpawnInWave;
    private int enemiesSpawnedThisWave;

    [Header("----- Wave UI -----")]
    public GameObject wavePanel;
    public TextMeshProUGUI waveTXT;

    public float waveTextDisplayTime = 3f;

    void Start()
    {
        spawnInterval = Mathf.Max(spawnInterval, limitDecrement);
        StartWave();
    }

    void StartWave()
    {
        enemiesToSpawnInWave = startingEnemiesPerWave + enemyIncrementPerWave * (currentWave - 1);
        enemiesSpawnedThisWave = 0;

        StartCoroutine(SpawnWave());
        StartCoroutine(ShowWaveText());
    }

    IEnumerator SpawnWave()
    {

        while (enemiesSpawnedThisWave < enemiesToSpawnInWave)
        {
            SpawnEnemy();
            enemiesSpawnedThisWave++;
            yield return new WaitForSeconds(spawnInterval);
        }

        currentWave++;

        spawnInterval -= spawnDecrement;
        spawnInterval = Mathf.Clamp(spawnInterval, limitDecrement, 999f);

        // Optional: Add delay between waves
        yield return new WaitForSeconds(10f);

        StartWave();
    }

    void SpawnEnemy()
    {
        if (enemies.Length == 0 || spawner.Length == 0)
        {
            Debug.LogWarning("No enemies or spawners assigned!");
            return;
        }

        GameObject enemyPrefab = null;
        Transform spawnPoint = null;

        if (enemiesSpawnedThisWave == 0)
        {
            // First enemy = enemies[0] -> spawner[0]
            enemyPrefab = enemies[0];
            spawnPoint = spawner[0];
        }
        else if (enemiesSpawnedThisWave == 1)
        {
            // Second enemy = enemies[1] -> spawner[1]
            enemyPrefab = enemies[1];
            spawnPoint = spawner[1];
        }
        else
        {
            // Other enemies = enemies[2] or enemies[3] -> spawner[2] only
            int enemyIndex = Random.Range(2, enemies.Length);

            // If you want them to spawn ONLY in spawner[2]:
            int spawnerIndex = 2;

            enemyPrefab = enemies[enemyIndex];
            spawnPoint = spawner[spawnerIndex];
        }

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }



    IEnumerator ShowWaveText()
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(true);
            waveTXT.text = $"WAVE {currentWave}";

            yield return new WaitForSeconds(waveTextDisplayTime);

            wavePanel.SetActive(false);
        }
    }
}
