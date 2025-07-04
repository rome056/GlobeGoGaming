using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban
    public GameObject smallEnemyPrefab;
    public float spreadRadius = 1.5f;
    private bool isStunned = false;
    private float originalSpeed;

    private Renderer modelRenderer;
    private Color originalColor;


    public GameObject stunEffectPrefab;      // Prefab ng visual effect
    private GameObject stunEffectInstance;

    void Start()
    {
        originalSpeed = speed;
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }
    void Update()
    {
        if (isStunned) return;

        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

    }
    public void OnHooked()
    {
        SplitIntoSmallerEnemies();
        Destroy(gameObject); // Patayin ang kalaban
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.TakeExp(10);
            PlayerController.instance.TakeCountEnemy();
            PlayerController.instance.TakeBar(10);
            Destroy(gameObject);

        }
        if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Hook"))
        {
            HookMechanism hook = other.GetComponent<HookMechanism>();
            if (hook != null)
            {
                hook.HookBee(gameObject); // Mahuhook siya
                OnHooked(); // Trigger destroy & spawn
            }
        }
    }
    void SplitIntoSmallerEnemies()
    {
        for (int i = 0; i < 4; i++)
        {
            // Random offset position (para kumalat ng kaunti)
            Vector3 offset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                0f,
                Random.Range(-spreadRadius, spreadRadius)
            );

            Vector3 spawnPos = transform.position + offset;
            Instantiate(smallEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
    public void SlowEffect(float newSpeed, float duration)
    {
        if (!isStunned)
        {
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
}