using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHopper : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban
    public float jumpHeight = 1f;
    public float jumpDuration = 0.5f;
    public float jumpInterval = 3f;

    private float jumpTimer;
    private bool isJumping = false;
    private float jumpStartTime;
    private Vector3 originalPosition;

    private bool isSlowed = false;
    private bool isStunned = false;
    private float originalSpeed;

    private Renderer modelRenderer;
    private Color originalColor;

    public GameObject stunEffectPrefab;      // Prefab ng visual effect
    private GameObject stunEffectInstance;
    void Start()
    {
        jumpTimer = jumpInterval;
        originalPosition = transform.position;
        originalSpeed = speed;
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }

    void Update()
    {
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        if (!isJumping && jumpTimer <= 0f)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            originalPosition = transform.position;
            jumpTimer = jumpInterval;
        }

        // Handle fake jump (tataas-bababa lang sa Y)
        if (isJumping)
        {
            float elapsed = Time.time - jumpStartTime;
            float percent = elapsed / jumpDuration;

            if (percent >= 1f)
            {
                // Jump done
                isJumping = false;
                transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);
            }
            else
            {
                float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(transform.position.x, originalPosition.y + yOffset, transform.position.z);
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