using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public float speed = 2f;

    private float fixedYPosition;

    [Header("Summoning Skill")]
    public GameObject[] enemyPrefabs; // Mga pwedeng i-summon na enemies
    public int minSummonCount = 3;
    public int maxSummonCount = 5;
    public float summonInterval = 3f;
    public float summonRadius = 3f;
    private float summonTimer;

    [Header("Status")]
    public bool isHooked = false;
    private bool isStunned = false;
    private float originalSpeed;

    [Header("Visuals")]
    public GameObject summonEffectPrefab;
    public GameObject stunEffectPrefab;
    private GameObject stunEffectInstance;
    private Renderer modelRenderer;
    private Color originalColor;
    public GameObject damageEffectPrefab;
    [Header("Health")]
    public int maxHealth = 500;
    public int currentHealth;
    public Slider healthSlider; // Reference sa UI health bar
    public int hookDamage = 50; // Damage kada hook
    void Start()
    {
        fixedYPosition = transform.position.y;
        originalSpeed = speed;
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null) originalColor = modelRenderer.material.color;

        currentHealth = maxHealth;
        UpdateHealthUI();
        summonTimer = summonInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHooked || isStunned) return;

        // Simple left movement only
        transform.position = new Vector3(
            transform.position.x - speed * Time.deltaTime,
            fixedYPosition,
            transform.position.z
        );

        // Summoning Skill
        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0f)
        {
            SummonEnemies();
            summonTimer = summonInterval;
        }
    }

    void SummonEnemies()
    {
        if (enemyPrefabs.Length == 0) return;

        int summonCount = Random.Range(minSummonCount, maxSummonCount + 1);

        // Summon effect
        if (summonEffectPrefab != null)
        {
            Instantiate(summonEffectPrefab, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < summonCount; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * summonRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            if (PlayerController.instance != null)
            {
                PlayerController.instance.TakeExp(100);
                PlayerController.instance.TakeCountEnemy();
                PlayerController.instance.TakeBar(10);
            }
        }
        else if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
    }
    public void SlowEffect(float newSpeed, float duration)
    {
        if (!isStunned)
        {
            //isSlowed = true;
            float currentSpeed = speed;
            speed = newSpeed;
            StartCoroutine(BlinkEffect(duration));
            StartCoroutine(ResetSpeedAfter(duration, currentSpeed));
        }
    }
    private IEnumerator ResetSpeedAfter(float duration, float originalSpeed)
    {
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
        //isSlowed = false;

        if (modelRenderer != null)
            modelRenderer.material.color = originalColor;
    }

    private IEnumerator BlinkEffect(float duration)
    {
        if (modelRenderer == null) yield break;

        float elapsed = 0f;
        bool toggle = false;
        Color slowColor = Color.blue;

        while (elapsed < duration)
        {
            modelRenderer.material.color = toggle ? slowColor : originalColor;
            toggle = !toggle;
            elapsed += 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        modelRenderer.material.color = originalColor;
    }
    public void Stun(float duration)
    {
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine(duration));
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        float originalSpeed = speed;
        speed = 0f; // 🛑 Stop movement

        // 🔆 Mag-spawn ng visual effect (optional)
        if (stunEffectPrefab != null && stunEffectInstance == null)
        {
            stunEffectInstance = Instantiate(stunEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            stunEffectInstance.transform.SetParent(transform); // Para sumunod sa enemy
        }

        yield return new WaitForSeconds(duration);

        // ✅ Restore
        isStunned = false;
        speed = originalSpeed;

        if (stunEffectInstance != null)
        {
            Destroy(stunEffectInstance);
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        // Damage effect
        if (damageEffectPrefab != null)
        {
            Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
    void Die()
    {
        Destroy(gameObject);
        if (PlayerController.instance != null)
        {
            PlayerController.instance.TakeExp(100);
            PlayerController.instance.TakeCountEnemy();
            PlayerController.instance.TakeBar(10);
        }
    }
    IEnumerator ResetHookState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHooked = false;
    }
}
