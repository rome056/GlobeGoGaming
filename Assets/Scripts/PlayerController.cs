using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Hook Settings")]
    public GameObject hookPrefab;
    private GameObject hookObject;
    private HookMechanism hook;
    private bool isFishing = false;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float leftBoundary = -5f;  // Set sa Inspector ang left limit
    public float rightBoundary = 5f;  // Set sa Inspector ang right limit
    private Coroutine barDecreaseCoroutine;

    [Header("Boundary Visualization")]
    public bool showBoundaries = true;
    public Color boundaryColor = Color.red;

    [Header("Player HP")]
    public float MaxHP = 100;
    public float currentHP = 100;

    [Header("Player EXP")]
    public float MaxExp = 100;
    public float currentEXP = 0;
    public Text limittextExp;
    public Slider expSlider;
    public Image expFill;
   

    [Header("Count Enemy Kill")]
    public int counterEnemy = 0;
    //public Text counttextEnemy;
    public TextMeshProUGUI counttextEnemy;

    [Header("Bar System")]
    public float MaxBar = 100;
    public float currentBar = 0;
    public Text BarText;
    public Slider barSlider;
    public Image barFill;
    public Gradient barGradient;

    [Header("HP Bar")]
    public Slider hpSlider;
    public Image hpFill;
    public Gradient hpGradient;

    private void Start()
    {
        currentHP = MaxHP;
        currentEXP = 0;
        counterEnemy = 0;
        currentBar = 0;
        barDecreaseCoroutine = StartCoroutine(DecreaseBarOverTime());
    }
    public void Awake()
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

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateUIHealth();   
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Destroy(gameObject);
        }


    }

    public void TakeExp(int exp)
    {
        //currentEXP += exp;
        currentEXP = currentEXP + exp;
        Debug.Log("Exp is " + currentEXP);
        UpdateExp();
        UpdateUIExp();

        if (currentEXP >= 100)
        {
            currentEXP = MaxExp;

        }
    }

    public void TakeExpBar(int bar)
    {
        currentEXP+= bar;
        Debug.Log("Exp is " + currentEXP);
      


        if (currentEXP >= 100)
        {
            currentEXP = MaxExp;
        }
    }
    public void TakeBar(int bar)
    {
        currentBar += bar;
        Debug.Log("Bar is " + currentBar);
        UpdateUIBar();
        UpdateBar();
        currentBar = Mathf.Clamp(currentBar, 0, MaxBar);
        if (currentBar >= 100)
        {
            currentBar = MaxBar;
        }

    }
    public void UpdateUIExp()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = MaxExp;
            expSlider.value = currentEXP;
        }
        
    }
    public void UpdateUIBar()
    {
        if (barSlider != null)
        {
            barSlider.maxValue = MaxBar;
            barSlider.value = currentBar;
        }
        if (barFill)
        {
            barFill.color = barGradient.Evaluate(barSlider.normalizedValue);
        }
    }
    public void UpdateUIHealth()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = MaxHP;
            hpSlider.value = currentHP;
        }
        if (hpFill)
        {
            hpFill.color = hpGradient.Evaluate(hpSlider.normalizedValue);
        }
    }

    public void TakeCountEnemy()
    {
        counterEnemy++;
        UpdateCountEnemy();
    }

    public void UpdateCountEnemy()
    {
        counttextEnemy.text = " Enemy Hooked: " + counterEnemy;
    }
    public void UpdateExp()
    {
        limittextExp.text = " exp is:  " + currentEXP;

    }
    public void UpdateBar()
    {
        BarText.text = " Bar is: " + currentBar;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log(" Enemy Eat");
        }
        if (other.CompareTag("EnemyAttack"))
        {
            EnemyBee enemy = other.GetComponent<EnemyBee>();
            if (enemy != null && !enemy.isHooked) // ✅ Only take damage if enemy was NOT hooked
            {
                TakeDamage(10); // ← Dito ka lang nadadamage kapag hindi siya nahook
            }
        }
    }
    public void EnemyDestroy()
    {
        Destroy(gameObject);
    }
    IEnumerator DecreaseBarOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Wait for 1 second
            TakeBar(-1); // Decrease bar by 1
        }
    }

    void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
        if (barDecreaseCoroutine != null)
        {
            StopCoroutine(barDecreaseCoroutine);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleFishing();
    }

    void HandleMovement()
    {
        // Get horizontal input (A/D keys or Left/Right arrows)
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0)
        {
            // Calculate new position
            Vector3 newPosition = transform.position;
            newPosition.x += horizontalInput * moveSpeed * Time.deltaTime;

            // Clamp position within boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

            // Apply the constrained position
            transform.position = newPosition;
        }
    }

    void HandleFishing()
    {
        if (Input.GetMouseButtonDown(0) && !isFishing)
        {
            LaunchHook();
        }

        if (isFishing && hook != null)
        {
            if (!hook.IsReturning && hook.HasCaughtEnemy)
            {
                Debug.Log("Nakahuli ng kalaban: " + hook.HookedEnemy.name);
            }
            if (!hook.IsReturning && !hook.IsMovingForward)
            {
                isFishing = false;
            }
        }
    }

    void LaunchHook()
    {
        hookObject = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        hook = hookObject.GetComponent<HookMechanism>();
        hook.SetUpHook(transform);
        isFishing = true;
        hook.onHookReturn = HookReturned;
    }

    public void HookReturned()
    {
        isFishing = false;
    }

    // Para makita mo ang boundaries sa Scene view
    void OnDrawGizmos()
    {
        if (showBoundaries)
        {
            Gizmos.color = boundaryColor;

            // Draw left boundary line
            Vector3 leftStart = new Vector3(leftBoundary, transform.position.y - 2f, transform.position.z);
            Vector3 leftEnd = new Vector3(leftBoundary, transform.position.y + 2f, transform.position.z);
            Gizmos.DrawLine(leftStart, leftEnd);

            // Draw right boundary line
            Vector3 rightStart = new Vector3(rightBoundary, transform.position.y - 2f, transform.position.z);
            Vector3 rightEnd = new Vector3(rightBoundary, transform.position.y + 2f, transform.position.z);
            Gizmos.DrawLine(rightStart, rightEnd);

            // Draw ground line connecting the boundaries
            Vector3 groundStart = new Vector3(leftBoundary, transform.position.y - 1f, transform.position.z);
            Vector3 groundEnd = new Vector3(rightBoundary, transform.position.y - 1f, transform.position.z);
            Gizmos.DrawLine(groundStart, groundEnd);
        }
    }




}
