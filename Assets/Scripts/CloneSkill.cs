using System.Collections;
using UnityEngine;

public class CloneSkill : MonoBehaviour
{
    public LineRenderer hookLine;
    public Transform hookStart;
    public Transform hookEnd;
    public float pullSpeed = 7f;
    public float delayBeforeStart = 0.3f;

    private GameObject hookedEnemy;

    void Start()
    {
        if (hookStart == null) hookStart = transform.Find("HookStart");
        if (hookEnd == null) hookEnd = transform.Find("HookEnd");
        if (hookLine == null) hookLine = GetComponent<LineRenderer>();

        if (hookLine != null && hookStart != null && hookEnd != null)
        {
            hookLine.positionCount = 2;
            hookLine.sortingOrder = 5;
            hookLine.material = new Material(Shader.Find("Sprites/Default"));
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        Invoke(nameof(StartHook), delayBeforeStart);
    }

    void Update()
    {
        if (hookLine != null && hookStart != null && hookEnd != null)
        {
            hookLine.SetPosition(0, hookStart.position);
            hookLine.SetPosition(1, hookEnd.position);
        }
    }

    void StartHook()
    {
        GameObject enemy = FindNearestEnemy();
        if (enemy != null)
        {
            hookedEnemy = enemy;
            StartCoroutine(PullEnemyToClone(enemy));
        }
        else
        {
            Destroy(gameObject); // Walang kalaban = destroy agad
        }
    }

    IEnumerator PullEnemyToClone(GameObject enemy)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        Collider enemyCol = enemy.GetComponent<Collider>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        while (enemy != null && Vector3.Distance(enemy.transform.position, hookStart.position) > 0.1f)
        {
            hookEnd.position = enemy.transform.position;
            hookLine.SetPosition(1, hookEnd.position);

            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, hookStart.position, pullSpeed * Time.deltaTime);
            yield return null;
        }

        // Drop enemy (optional effect)
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = new Vector3(0, -5f, 0); // para mahulog pababa
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject); // ✅ after hook, destroy clone
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }
}
