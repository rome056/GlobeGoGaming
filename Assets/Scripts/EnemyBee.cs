using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBee : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban

    public float chargeSpeed = 10f;        // Bilis kapag umatake
    public float detectionRange = 5f;      // Range para ma-detect ang player
    public float chargeDelay = 1f;         // Gaano katagal titigil bago umatake

    private GameObject player;
    private bool isCharging = false;
    private bool isWaitingToCharge = false;
    private float chargeTimer = 0f;
    public bool isHooked = false;  // ← Idedetect kung nahook na

    private bool isSlowed = false;
    private bool isStunned = false;
    private float originalSpeed;

    private Renderer modelRenderer;
    private Color originalColor;

    public GameObject stunEffectPrefab;      // Prefab ng visual effect
    private GameObject stunEffectInstance;   // Para masira after stun
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        originalSpeed = speed;
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }
    void Update()
    {
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isCharging)
        {
            // Dash papunta sa player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * chargeSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Player detected
            if (!isWaitingToCharge)
            {
                isWaitingToCharge = true;
                chargeTimer = chargeDelay;
            }
            else
            {
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f)
                {
                    isCharging = true;
                    isWaitingToCharge = false;
                }
            }
        }
        else
        {
            // Normal movement to the left (patrol)
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(gameObject);

        }
        if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }

        {
            if (other.CompareTag("Hook"))
            {
                HookMechanism hook = other.GetComponent<HookMechanism>();
                if (hook != null)
                {
                    isHooked = true;                 // ← I-flag na nahook na ito
                    hook.HookBee(gameObject);        // ← I-hook ang sarili
                    PlayerController.instance.TakeBar(10);
                    PlayerController.instance.TakeExp(10);
                    PlayerController.instance.TakeCountEnemy();
                }
            }
        }

    }
    public void SlowEffect(float newSpeed, float duration)
    {
        if (!isStunned)
        {
            isSlowed = true;
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
        isSlowed = false;

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
