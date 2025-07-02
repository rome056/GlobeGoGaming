using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban
    public float jumpHeight = 1f;          // Taas ng jump sa Y
    public float jumpDuration = 0.5f;      // Gaano katagal ang isang jump
    public float zigzagOffsetZ = 1f;       // Layo ng left/right zigzag
    private float jumpStartTime;
    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;
    private bool isJumping = false;
    public bool isHooked = false; // ← Flag kung nahook na

    private bool jumpRight = true;


    private bool isSlowed = false;
    private bool isStunned = false;
    private float originalSpeed;

    private Renderer modelRenderer;
    private Color originalColor;


    public GameObject stunEffectPrefab;      // Prefab ng visual effect
    private GameObject stunEffectInstance;   // Para masira after stun

    void Start()
    {
        originalSpeed = speed;
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }

    void Update()
    {
        if (isHooked) return;
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!isJumping)
        {
            // Start new zigzag jump
            isJumping = true;
            jumpStartTime = Time.time;
            jumpStartPos = transform.position;

            float targetZ = jumpStartPos.z + (jumpRight ? zigzagOffsetZ : -zigzagOffsetZ);
            jumpTargetPos = new Vector3(jumpStartPos.x, jumpStartPos.y, targetZ);

            // Next time, other direction naman
            jumpRight = !jumpRight;
        }
        else
        {
            // Calculate progress
            float elapsed = Time.time - jumpStartTime;
            float percent = elapsed / jumpDuration;

            if (percent >= 1f)
            {
                // Done jumping
                isJumping = false;
                transform.position = new Vector3(transform.position.x, jumpStartPos.y, jumpTargetPos.z);
            }
            else
            {
                // Jump motion: pa-curve (Y axis) + zigzag (Z axis)
                float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                float newZ = Mathf.Lerp(jumpStartPos.z, jumpTargetPos.z, percent);
                transform.position = new Vector3(transform.position.x, jumpStartPos.y + yOffset, newZ);
            }
        }
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
                isHooked = true; // ✅ Hinto ang movement
                hook.HookBee(gameObject); // Ipasa ang sarili sa hook
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