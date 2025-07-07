using UnityEngine;
using System.Collections;

public class EnemyBug : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f; // Normal speed
    public float dashSpeedMultiplier = 3f; // How much faster during dash
    public float dashDuration = 1f; // How long dash lasts
    public float dashCooldown = 3f; // Time between dashes

    [Header("Debug")]
    private float originalSpeed;
    private Coroutine dashCoroutine;

    private bool isStunned = false;


    private Renderer modelRenderer;
    private Color originalColor;
    public GameObject stunEffectPrefab;      // Prefab ng visual effect
    private GameObject stunEffectInstance;
    void Start()
    {
        originalSpeed = speed;
        StartCoroutine(DashRoutine());
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }

    void Update()
    {
        // Basic movement (faster when dashing)
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    IEnumerator DashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashCooldown);
            yield return StartCoroutine(BugDashSkill());
        }
    }

    IEnumerator BugDashSkill()
    {
        // Start dash
        speed *= dashSpeedMultiplier;

        yield return new WaitForSeconds(dashDuration);

        // End dash
        speed = originalSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            if (PlayerController.instance != null)
            {
                PlayerController.instance.TakeExp(10);
                PlayerController.instance.TakeCountEnemy();
                PlayerController.instance.TakeBar(5);
            }
        }
        else if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
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
}