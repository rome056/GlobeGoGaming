using UnityEngine;

public class HookMechanism : MonoBehaviour
{
    public static HookMechanism instance;
    public float speed;
    public float maxDistance;

    private Vector3 targetDirection;
    private Vector3 startPoint;
    private Vector3 targetPoint;
    private float hookProgress = 0f;

    private Transform playerTransform;
    private GameObject hookedEnemy;
    private LineRenderer lineRenderer;

    public bool IsReturning { get; private set; } = false;
    public bool IsMovingForward { get; private set; } = true;
    public bool HasCaughtEnemy { get; private set; } = false;
    public GameObject HookedEnemy => hookedEnemy;
    public System.Action onHookReturn; // Tawagin ito kapag tapos na ang hook

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null && playerTransform != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, playerTransform.position); // Simula (player)
            lineRenderer.SetPosition(1, transform.position);       // Hook position
        }
    }

    public void SetUpHook(Transform player)
    {
        playerTransform = player;
        startPoint = player.position;
        speed = PlayerController.instance.hookSpeed;
        maxDistance = PlayerController.instance.hookRange;

        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane ground = new Plane(Vector3.up, Vector3.zero); // Assume Y-up

        if (ground.Raycast(ray, out float distance))
        {
            targetPoint = ray.GetPoint(distance);
        }
        else
        {
            targetPoint = player.position + player.forward * maxDistance;
        }

        // Limit max distance
        if (Vector3.Distance(startPoint, targetPoint) > maxDistance)
        {
            targetPoint = startPoint + (targetPoint - startPoint).normalized * maxDistance;
        }

        targetDirection = (targetPoint - startPoint).normalized;
        hookProgress = 0f;
    }

    void Update()
{
    if (playerTransform == null)
            return; // Hintayin ang SetUpHook para hindi mag-error

        if (IsMovingForward)
        {
            hookProgress += Time.deltaTime * (speed / Vector3.Distance(startPoint, targetPoint));
            float easedProgress = Mathf.SmoothStep(0f, 1f, hookProgress);
            transform.position = Vector3.Lerp(startPoint, targetPoint, easedProgress);

            if (hookProgress >= 1f)
            {
                IsMovingForward = false;
                IsReturning = true;
            }
        }
        else if (IsReturning)
        {
            Vector3 returnDir = (playerTransform.position - transform.position).normalized;
            transform.position += returnDir * speed * Time.deltaTime;

            if (hookedEnemy != null)
            {
                hookedEnemy.transform.position = transform.position; // Hinila ang enemy
            }

            if (Vector3.Distance(transform.position, playerTransform.position) < 1f)
            {
                onHookReturn?.Invoke(); // Tawagin si Player
                Destroy(gameObject);    // Tapos sirain ang hook
            }
        }
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, playerTransform.position); 
            lineRenderer.SetPosition(1, transform.position);      
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (!IsReturning && other.CompareTag("Enemy"))
        {
            HasCaughtEnemy = true;
            hookedEnemy = other.gameObject;
            IsMovingForward = false;
            IsReturning = true;
        }
        if (!IsReturning && other.CompareTag("EnemyAttack"))
        {
            HasCaughtEnemy = true;
            hookedEnemy = other.gameObject;
            IsMovingForward = false;
            IsReturning = true;
        }
    }
    public void HookBee(GameObject enemyObject)
    {
        HasCaughtEnemy = true;
        hookedEnemy = enemyObject;
        IsMovingForward = false;
        IsReturning = true;

        // 🔒 Mark the enemy as hooked
        EnemyBee enemyScript = enemyObject.GetComponent<EnemyBee>();
        if (enemyScript != null)
        {
            enemyScript.isHooked = true; // ← i-flag na nahook siya
        }
    }
    public void HookSlime(GameObject enemyObject)
    {
        HasCaughtEnemy = true;
        hookedEnemy = enemyObject;
        IsMovingForward = false;
        IsReturning = true;

        EnemySlime enemyScript = enemyObject.GetComponent<EnemySlime>();
        if (enemyScript != null)
        {
            enemyScript.isHooked = true;
        }
    }
}
