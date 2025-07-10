using System.Collections;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;            // Forward speed
    private float zigzagTimer;
    private Vector3 basePosition;
    public float zigzagFrequency = 3f;  // Bagong parameter para kontrolin ang bilis ng zigzag
    public float zigzagWidth = 1f;  // Mas descriptive na pangalan
    private float fixedYPosition;  // Fixed Y position

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
        basePosition = transform.position;

        fixedYPosition = transform.position.y;  // I-save ang starting Y position
        zigzagTimer = 0f;
    }

    void Update()
    {
        if (isHooked || isStunned) return;

        // Move left towards the base
        zigzagTimer += Time.deltaTime * zigzagFrequency;

        float zigzagOffset = Mathf.Sin(zigzagTimer * Mathf.PI * 2) * zigzagWidth;

        // Apply movement
        transform.position = new Vector3(
            transform.position.x - speed * Time.deltaTime,  // Left movement
            fixedYPosition,                                 // Fixed Y position
            transform.position.z + zigzagOffset * Time.deltaTime * zigzagFrequency  // Zigzag
        );

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            PlayerController.instance.TakeExp(10);
            PlayerController.instance.TakeBar(10);
           
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
                PlayerController.instance.TakeBar(10);
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
