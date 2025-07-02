using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        [Range(0f, 1f)]
        public float spawnWeight = 1f; // Probability weight for spawning
        public int minCount = 1;
        public int maxCount = 3;
    }

    [System.Serializable]
    public class WaveData
    {
        public string waveName = "Wave";
        public EnemySpawnData[] enemies;
        public int totalEnemyCount = 5;
        public float spawnDelay = 0.5f;
        public bool isBossWave = false;
        public GameObject bossPrefab;
        public string bossName = "Boss";
    }

    [System.Serializable]
    public class LevelData
    {
        public string levelName = "Level 1";
        public WaveData[] waves;

        [Header("Difficulty Modifiers")]
        public float enemySpeedMultiplier = 1f;
        public float enemyHealthMultiplier = 1f;
        public float enemyDamageMultiplier = 1f;
        public float skillFrequencyMultiplier = 1f;

        [Header("Level Settings")]
        public float timeBetweenWaves = 5f;
        public bool showIntro = true;
        public bool showCountdown = true;
    }

    [Header("Level Configuration")]
    public List<LevelData> levels = new List<LevelData>();

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public bool randomizeSpawnPoints = true;
    public float defaultTimeBetweenWaves = 5f;

    [Header("UI References")]
    public Text waveText;
    public Text countdownText;
    public Text levelText;
    public Text enemyCountText;

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip waveStartSound;
    public AudioClip bossWarningSound;
    public AudioClip levelCompleteSound;

    [Header("Events")]
    public UnityEvent OnLevelStart;
    public UnityEvent OnWaveStart;
    public UnityEvent OnWaveComplete;
    public UnityEvent OnBossSpawn;
    public UnityEvent OnLevelComplete;

    // Private variables
    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private int totalEnemiesInWave = 0;
    private bool isWaveActive = false;
    private Coroutine currentLevelCoroutine;

    // Properties
    public int CurrentLevel => currentLevelIndex + 1;
    public int CurrentWave => currentWaveIndex + 1;
    public bool IsWaveActive => isWaveActive;
    public int EnemiesAlive => enemiesAlive;

    void Start()
    {
        if (levels.Count == 0)
        {
            Debug.LogError("No levels configured in EnemyWaveSpawner!");
            return;
        }

        StartLevel(currentLevelIndex);
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex >= levels.Count)
        {
            Debug.LogWarning($"Level {levelIndex} does not exist!");
            return;
        }

        if (currentLevelCoroutine != null)
        {
            StopCoroutine(currentLevelCoroutine);
        }

        currentLevelIndex = levelIndex;
        currentWaveIndex = 0;
        currentLevelCoroutine = StartCoroutine(ExecuteLevel(levels[levelIndex]));
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            StartLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("All levels completed!");
            ShowText("🎉 ALL LEVELS COMPLETED! 🎉", "");
        }
    }

    public void RestartCurrentLevel()
    {
        StartLevel(currentLevelIndex);
    }

    IEnumerator ExecuteLevel(LevelData level)
    {
        OnLevelStart?.Invoke();
        UpdateLevelText(level.levelName);

        if (level.showIntro)
        {
            yield return StartCoroutine(ShowIntro());
        }

        for (int i = 0; i < level.waves.Length; i++)
        {
            currentWaveIndex = i;
            WaveData wave = level.waves[i];

            yield return StartCoroutine(ExecuteWave(wave, level));

            // Wait for all enemies to be defeated before proceeding
            yield return StartCoroutine(WaitForWaveCompletion());

            OnWaveComplete?.Invoke();

            // Don't wait after the last wave
            if (i < level.waves.Length - 1)
            {
                float waitTime = level.timeBetweenWaves > 0 ? level.timeBetweenWaves : defaultTimeBetweenWaves;
                yield return new WaitForSeconds(waitTime);
            }
        }

        OnLevelComplete?.Invoke();
        PlaySound(levelCompleteSound);
        ShowText("✅ LEVEL COMPLETED! ✅", "");
    }

    IEnumerator ExecuteWave(WaveData wave, LevelData level)
    {
        OnWaveStart?.Invoke();
        isWaveActive = true;

        if (wave.isBossWave)
        {
            yield return StartCoroutine(ShowBossWarning(wave.bossName, level.showCountdown));
            yield return StartCoroutine(SpawnBoss(wave, level));
        }
        else
        {
            yield return StartCoroutine(ShowWaveText(wave.waveName, level.showCountdown));
            yield return StartCoroutine(SpawnWave(wave, level));
        }
    }

    IEnumerator SpawnWave(WaveData wave, LevelData level)
    {
        totalEnemiesInWave = wave.totalEnemyCount;
        enemiesAlive = 0;

        for (int i = 0; i < wave.totalEnemyCount; i++)
        {
            GameObject enemyToSpawn = SelectRandomEnemy(wave.enemies);
            if (enemyToSpawn != null)
            {
                SpawnSingleEnemy(enemyToSpawn, level);
                enemiesAlive++;
                UpdateEnemyCountText();
            }

            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    IEnumerator SpawnBoss(WaveData wave, LevelData level)
    {
        OnBossSpawn?.Invoke();
        totalEnemiesInWave = 1;
        enemiesAlive = 0;

        if (wave.bossPrefab != null)
        {
            SpawnSingleEnemy(wave.bossPrefab, level);
            enemiesAlive++;
            UpdateEnemyCountText();
        }

        yield return null;
    }

    GameObject SelectRandomEnemy(EnemySpawnData[] enemies)
    {
        if (enemies.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var enemy in enemies)
        {
            totalWeight += enemy.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var enemy in enemies)
        {
            currentWeight += enemy.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemy.enemyPrefab;
            }
        }

        return enemies[0].enemyPrefab; // Fallback
    }

    void SpawnSingleEnemy(GameObject enemyPrefab, LevelData level)
    {
        Transform spawnPoint = GetSpawnPoint();
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        ApplyDifficultyModifiers(enemy, level);
        RegisterEnemyEvents(enemy);
    }

    Transform GetSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points configured!");
            return transform;
        }

        if (randomizeSpawnPoints)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        else
        {
            return spawnPoints[currentWaveIndex % spawnPoints.Length];
        }
    }

    void ApplyDifficultyModifiers(GameObject enemy, LevelData level)
    {
        // Apply to different enemy types - extend as needed
        var enemyBee = enemy.GetComponent<EnemyBee>();
        if (enemyBee != null)
        {
            enemyBee.speed *= level.enemySpeedMultiplier;
        }

        var enemyFly = enemy.GetComponent<EnemyFly>();
        if (enemyFly != null)
        {
            enemyFly.speed *= level.enemySpeedMultiplier;
        }

        // Generic approach for enemies with common interfaces
        var enemyHealth = enemy.GetComponent<IEnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.MultiplyHealth(level.enemyHealthMultiplier);
        }

        var enemyDamage = enemy.GetComponent<IEnemyDamage>();
        if (enemyDamage != null)
        {
            enemyDamage.MultiplyDamage(level.enemyDamageMultiplier);
        }
    }

    void RegisterEnemyEvents(GameObject enemy)
    {
        // Register for death event - adjust based on your enemy system
        var enemyHealth = enemy.GetComponent<MonoBehaviour>();
        if (enemyHealth != null)
        {
            // You might need to adapt this based on your enemy death system
            StartCoroutine(WaitForEnemyDestruction(enemy));
        }
    }

    IEnumerator WaitForEnemyDestruction(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return null;
        }

        OnEnemyDefeated();
    }

    public void OnEnemyDefeated()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
        UpdateEnemyCountText();
    }

    IEnumerator WaitForWaveCompletion()
    {
        while (enemiesAlive > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        isWaveActive = false;
    }

    // UI Methods
    IEnumerator ShowIntro()
    {
        ShowText("READY", "");
        yield return new WaitForSeconds(0.8f);
        ShowText("SET", "");
        yield return new WaitForSeconds(0.8f);
        ShowText("GO!", "");
        PlaySound(waveStartSound);
        yield return new WaitForSeconds(0.8f);
        ShowText("", "");
    }

    IEnumerator ShowWaveText(string waveName, bool showCountdown)
    {
        ShowText(waveName.ToUpper(), "");

        if (showCountdown)
        {
            yield return StartCoroutine(ShowCountdown());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            ShowText("", "");
        }
    }

    IEnumerator ShowBossWarning(string bossName, bool showCountdown)
    {
        ShowText($"⚠️ BOSS INCOMING!\n{bossName.ToUpper()}", "");
        PlaySound(bossWarningSound);

        if (showCountdown)
        {
            yield return StartCoroutine(ShowCountdown());
        }
        else
        {
            yield return new WaitForSeconds(2f);
            ShowText("", "");
        }
    }

    IEnumerator ShowCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
            countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        ShowText("", "");
    }

    void ShowText(string wave, string countdown)
    {
        if (waveText != null) waveText.text = wave;
        if (countdownText != null) countdownText.text = countdown;
    }

    void UpdateLevelText(string levelName)
    {
        if (levelText != null)
            levelText.text = $"Level: {levelName}";
    }

    void UpdateEnemyCountText()
    {
        if (enemyCountText != null)
            enemyCountText.text = $"Enemies: {enemiesAlive}/{totalEnemiesInWave}";
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Public utility methods
    public void PauseWave()
    {
        Time.timeScale = 0f;
    }

    public void ResumeWave()
    {
        Time.timeScale = 1f;
    }

    public LevelData GetCurrentLevel()
    {
        return currentLevelIndex < levels.Count ? levels[currentLevelIndex] : null;
    }

    public WaveData GetCurrentWave()
    {
        var level = GetCurrentLevel();
        return level != null && currentWaveIndex < level.waves.Length ? level.waves[currentWaveIndex] : null;
    }
}

// Optional interfaces for generic enemy handling
public interface IEnemyHealth
{
    void MultiplyHealth(float multiplier);
}

public interface IEnemyDamage
{
    void MultiplyDamage(float multiplier);
}