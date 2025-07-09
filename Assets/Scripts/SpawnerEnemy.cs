using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnerEnemy : MonoBehaviour
{
    [Header("----- Enemy Prefabs -----")]
    public List<GameObject> enemies = new List<GameObject>();

    [Header("----- Spawner Points -----")]
    public List<Transform> spawner = new List<Transform>();

    [Header("----- Spawn Settings -----")]
    public float spawnInterval = 3f;

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

        // Optional: Add delay between waves
        yield return new WaitForSeconds(10f);

        StartWave();
    }

    void SpawnEnemy()
    {
        if (enemies.Count == 0 || spawner.Count == 0)
        {
            Debug.LogWarning("No enemies or spawners assigned!");
            return;
        }

        int enemyIndex = Random.Range(0, enemies.Count);
        GameObject enemyPrefab = enemies[enemyIndex];

        int spawnerIndex = Random.Range(0, spawner.Count);
        Transform spawnPoint = spawner[spawnerIndex];

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
