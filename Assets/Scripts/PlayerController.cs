using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Hook Settings")]
    public GameObject hookPrefab;
    private HookMechanism hook;
    private bool isFishing = false;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;

    [Header("Boundary Visualization")]
    public bool showBoundaries = true;
    public Color boundaryColor = Color.red;

    [Header("Player HP")]
    public float MaxHP = 100f;
    public float currentHP;

    [Header("Player EXP")]
    public float MaxExp = 100f;
    public float currentEXP = 0f;
    public TextMeshProUGUI limittextExp;
    public Slider expSlider;
    public Image expFill;

    [Header("Count Enemy Kill")]
    public int counterEnemy = 0;
    public TextMeshProUGUI counttextEnemy;

    [Header("Bar System")]
    public float MaxBar = 100f;
    public float currentBar = 0f;
    public TextMeshProUGUI BarText;
    public Slider barSlider;
    public Image barFill;
    public Gradient barGradient;

    [Header("HP Bar")]
    public Slider hpSlider;
    public Image hpFill;
    public Gradient hpGradient;

    private Coroutine barDecreaseCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHP = MaxHP;
        currentEXP = 0f;
        counterEnemy = 0;
        currentBar = 0f;
        barDecreaseCoroutine = StartCoroutine(DecreaseBarOverTime());
        UpdateUIHealth();
        UpdateUIExp();
        UpdateUIBar();
        UpdateCountEnemy();
    }

    private void Update()
    {
        HandleMovement();
        HandleFishing();
    }

    #region Movement & Fishing
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            Vector3 newPosition = transform.position;
            newPosition.x += horizontalInput * moveSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);
            transform.position = newPosition;
        }
    }

    private void HandleFishing()
    {
        if (Input.GetMouseButtonDown(0) && !isFishing)
        {
            LaunchHook();
        }

        if (isFishing && hook != null && !hook.IsReturning && !hook.IsMovingForward)
        {
            isFishing = false;
        }
    }

    private void LaunchHook()
    {
        var hookObject = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        hook = hookObject.GetComponent<HookMechanism>();
        hook.SetUpHook(transform);
        isFishing = true;
        hook.onHookReturn = HookReturned;
    }

    public void HookReturned()
    {
        isFishing = false;
    }
    #endregion

    #region Take Damage & Heal
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);
        UpdateUIHealth();
        Debug.Log($"Player HP: {currentHP}");

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);
        UpdateUIHealth();
        Debug.Log($"Healed! Current HP: {currentHP}");
    }
    #endregion

    #region EXP & Bar Logic
    public void TakeExp(int exp)
    {
        currentEXP += exp;
        currentEXP = Mathf.Clamp(currentEXP, 0, MaxExp);
        Debug.Log($"EXP: {currentEXP}");
        UpdateUIExp();
    }

    public void TakeBar(int amount)
    {
        currentBar += amount;
        currentBar = Mathf.Clamp(currentBar, 0, MaxBar);
        Debug.Log($"Bar: {currentBar}");
        UpdateUIBar();
    }
    #endregion

    #region UI Updates
    public void UpdateUIHealth()
    {
        if (hpSlider)
        {
            hpSlider.maxValue = MaxHP;
            hpSlider.value = currentHP;
        }
        if (hpFill) hpFill.color = hpGradient.Evaluate(hpSlider.normalizedValue);
    }

    public void UpdateUIExp()
    {
        if (expSlider)
        {
            expSlider.maxValue = MaxExp;
            expSlider.value = currentEXP;
        }
        if (limittextExp) limittextExp.text = $"EXP: {currentEXP}/{MaxExp}";
    }

    public void UpdateUIBar()
    {
        if (barSlider)
        {
            barSlider.maxValue = MaxBar;
            barSlider.value = currentBar;
        }
        if (barFill) barFill.color = barGradient.Evaluate(barSlider.normalizedValue);
        if (BarText) BarText.text = $"Bar: {currentBar}/{MaxBar}";
    }

    public void UpdateCountEnemy()
    {
        if (counttextEnemy)
        {
            counttextEnemy.text = $"Enemies Hooked: {counterEnemy}";
        }
    }
    #endregion

    #region Misc Logic
    public void TakeCountEnemy()
    {
        counterEnemy++;
        UpdateCountEnemy();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit player");
        }
        if (other.CompareTag("EnemyAttack"))
        {
            EnemyBee enemy = other.GetComponent<EnemyBee>();
            if (enemy != null && !enemy.isHooked)
            {
                TakeDamage(10);
            }
        }
    }

    IEnumerator DecreaseBarOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            TakeBar(-1);
        }
    }

    private void OnDestroy()
    {
        if (barDecreaseCoroutine != null)
        {
            StopCoroutine(barDecreaseCoroutine);
        }
    }

    private void OnDrawGizmos()
    {
        if (showBoundaries)
        {
            Gizmos.color = boundaryColor;
            Gizmos.DrawLine(new Vector3(leftBoundary, transform.position.y - 2, transform.position.z), new Vector3(leftBoundary, transform.position.y + 2, transform.position.z));
            Gizmos.DrawLine(new Vector3(rightBoundary, transform.position.y - 2, transform.position.z), new Vector3(rightBoundary, transform.position.y + 2, transform.position.z));
            Gizmos.DrawLine(new Vector3(leftBoundary, transform.position.y - 1, transform.position.z), new Vector3(rightBoundary, transform.position.y - 1, transform.position.z));
        }
    }
    #endregion
}
