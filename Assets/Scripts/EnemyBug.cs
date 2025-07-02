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
    [SerializeField] private bool isDashing = false;
    private float originalSpeed;
    private Coroutine dashCoroutine;

    void Start()
    {
        originalSpeed = speed;
        StartCoroutine(DashRoutine());
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
        isDashing = true;
        speed *= dashSpeedMultiplier;

        yield return new WaitForSeconds(dashDuration);

        // End dash
        speed = originalSpeed;
        isDashing = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.TakeExp(10);
                PlayerController.instance.TakeCountEnemy();
                PlayerController.instance.TakeBar(5);
            }
            Destroy(gameObject);
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
}