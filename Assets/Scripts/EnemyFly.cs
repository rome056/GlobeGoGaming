using System.Collections;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;            // Forward speed
    public float jumpHeight = 1f;       // Height of jump (Y-axis)
    public float jumpDuration = 0.5f;   // Duration of one zigzag jump
    public float zigzagOffsetZ = 1f;    // Sideways zigzag amount (Z-axis)

    private bool isJumping = false;
    private bool jumpRight = true;

    private float jumpStartTime;
    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;

    [Header("Status")]
    public bool isHooked = false;
    private bool isStunned = false;
    private float originalSpeed;

    [Header("Visuals")]
    private Renderer modelRenderer;
    private Color originalColor;

    public GameObject stunEffectPrefab;
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
        if (isHooked || isStunned) return;

        // Move left towards the base
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!isJumping)
        {
            isJumping = true;
            jumpStartTime = Time.time;
            jumpStartPos = transform.position;

            float targetZ = jumpStartPos.z + (jumpRight ? zigzagOffsetZ : -zigzagOffsetZ);
            jumpTargetPos = new Vector3(jumpStartPos.x, jumpStartPos.y, targetZ);

            jumpRight = !jumpRight;
        }
        else
        {
            float elapsed = Time.time - jumpStartTime;
            float percent = elapsed / jumpDuration;

            if (percent >= 1f)
            {
                isJumping = false;
                transform.position = new Vector3(transform.position.x, jumpStartPos.y, jumpTargetPos.z);
            }
            else
            {
                float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                float newZ = Mathf.Lerp(jumpStartPos.z, jumpTargetPos.z, percent);
                transform.position = new Vector3(transform.position.x, jumpStartPos.y + yOffset, newZ);
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
        else if (other.CompareTag("Hook"))
        {
            HookMechanism hook = other.GetComponent<HookMechanism>();
            if (hook != null)
            {
                isHooked = true;
                hook.HookBee(gameObject);
            }
        }
    }

    public void SlowEffect(float newSpeed, float duration)
    {
        if (!isStunned)
        {
            speed = newSpeed;
            StartCoroutine(BlinkEffect(duration));
            StartCoroutine(ResetSpeedAfter(duration));
        }
    }

    private IEnumerator ResetSpeedAfter(float duration)
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
            StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        float prevSpeed = speed;
        speed = 0f;

        if (stunEffectPrefab != null && stunEffectInstance == null)
        {
            stunEffectInstance = Instantiate(stunEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            stunEffectInstance.transform.SetParent(transform);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;
        speed = prevSpeed;

        if (stunEffectInstance != null)
            Destroy(stunEffectInstance);
    }
}
