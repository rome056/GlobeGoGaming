using System.Collections;
using UnityEngine;

public class EnemyHopper : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float jumpHeight = 1f;
    public float jumpDuration = 0.5f;
    public float jumpInterval = 3f;

    private float jumpTimer;
    private bool isJumping = false;
    private float jumpStartTime;
    private Vector3 jumpStartPos;

    private bool isStunned = false;
    private float originalSpeed;

    [Header("Visuals")]
    private Renderer modelRenderer;
    private Color originalColor;

    public GameObject stunEffectPrefab;
    private GameObject stunEffectInstance;

    void Start()
    {
        jumpTimer = jumpInterval;
        originalSpeed = speed;

        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
            originalColor = modelRenderer.material.color;
    }

    void Update()
    {
        if (isStunned) return;

        // Move left towards the base
        transform.position += Vector3.left * speed * Time.deltaTime;

        jumpTimer -= Time.deltaTime;

        if (!isJumping && jumpTimer <= 0f)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            jumpStartPos = transform.position;
            jumpTimer = jumpInterval;
        }

        if (isJumping)
        {
            float elapsed = Time.time - jumpStartTime;
            float percent = elapsed / jumpDuration;

            if (percent >= 1f)
            {
                isJumping = false;
                transform.position = new Vector3(transform.position.x, jumpStartPos.y, transform.position.z);
            }
            else
            {
                float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(transform.position.x, jumpStartPos.y + yOffset, transform.position.z);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.TakeExp(10);
            PlayerController.instance.TakeCountEnemy();
            PlayerController.instance.TakeBar(10);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
    }

    public void SlowEffect(float newSpeed, float duration)
    {
        if (isStunned) return; // Ignore slow when stunned

        StartCoroutine(SlowCoroutine(newSpeed, duration));
    }

    private IEnumerator SlowCoroutine(float newSpeed, float duration)
    {
        float prevSpeed = speed;
        speed = newSpeed;

        if (modelRenderer != null)
            StartCoroutine(BlinkEffect(duration));

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
            StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        float prevSpeed = speed;
        speed = 0f;

        if (stunEffectPrefab != null && stunEffectInstance == null)
        {
            stunEffectInstance = Instantiate(stunEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
            stunEffectInstance.transform.SetParent(transform);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;
        speed = originalSpeed;

        if (stunEffectInstance != null)
            Destroy(stunEffectInstance);
    }
}
