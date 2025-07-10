using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    public Slider expSlider;
    public Image expFill;

    [Header("Skill Bar System")]
    public float MaxBar = 100f;
    public float currentBar = 0f;
    public Slider barSlider;
    public Image barFill;
    public Gradient barGradient;

    [Header("HP Bar")]
    public Slider hpSlider;
    public Image hpFill;
    public Gradient hpGradient;

    [Header("Skill Unlock System")]
    public Button slowButton;
    public Button stunButton;
    public Button healButton;
    public Button cloneButton;

    private List<Button> skillButtons = new List<Button>();
    private Button unlockedSkill = null;

    private Coroutine barDecreaseCoroutine;

    public GameObject skillUpgradePanel;


    [Header("Skill System")]
    public int slowLevel = 0;
    public int stunLevel = 0;
    public int cloneLevel = 0;
    public int healLevel = 0;

    public float hookRange = 6f;
    public float hookSpeed = 20f;
    private bool isSkillReady = false;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHP = MaxHP;
        currentEXP = 0f;
        currentBar = 0f;

        // Initialize skill buttons list
        skillButtons.Clear();
        skillButtons.Add(slowButton);
        skillButtons.Add(stunButton);
        skillButtons.Add(healButton);
        skillButtons.Add(cloneButton);
        LockAllSkills();

        barDecreaseCoroutine = StartCoroutine(DecreaseBarOverTime());

        UpdateUIHealth();
        UpdateUIExp();
        UpdateUIBar();
    }

    public void ExpTake(int exp)
    {
        currentEXP = exp;
        
    }


    private void Update()
    {
        HandleMovement();
        HandleFishing();

        //Para sa Skill sa pagunlock para magamit
        if (currentBar >= MaxBar && unlockedSkill == null)
        {
            UnlockRandomSkill();
            currentBar = 0;
            UpdateUIBar();
        }

        //Para sa pagupgrade kang Skill
        if (currentEXP >= MaxExp) 
        {
            ShowUpgradePanel();
            currentEXP = 0; // Reset EXP
            UpdateUIExp();
        }
        
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

    //for heal...........
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

        if (currentBar >= MaxBar && !isSkillReady && unlockedSkill == null)
        {
            isSkillReady = true;
            UnlockRandomSkill();
        }
    }

    #endregion

    #region UI Updates
    public void LockAllSkills()
    {
        foreach (Button btn in skillButtons)
        {
            if (btn != null)
                btn.interactable = false;
        }
        unlockedSkill = null;
    }
    public void UnlockRandomSkill()
    {
        // Filter out null buttons
        List<Button> validButtons = new List<Button>();
        foreach (Button btn in skillButtons)
        {
            if (btn != null)
                validButtons.Add(btn);
        }

        if (validButtons.Count > 0)
        {
            int rand = Random.Range(0, validButtons.Count);
            unlockedSkill = validButtons[rand];
            unlockedSkill.interactable = true;

            // Remove any existing listeners to prevent duplicates
            unlockedSkill.onClick.RemoveAllListeners();
            unlockedSkill.onClick.AddListener(OnSkillUsed);

            Debug.Log($"Unlocked skill: {unlockedSkill.name}");
        }
    }
    public void OnSkillUsed()
    {
        Debug.Log("Skill used!");

        // Apply effect based on unlocked skill
        if (unlockedSkill == slowButton)
        {
            ApplySlowEffect();
        }
        else if (unlockedSkill == stunButton)
        {
            ApplyStunEffect();
        }
        else if (unlockedSkill == healButton)
        {
            ApplyHealEffect();
        }
        else if (unlockedSkill == cloneButton)
        {
            ApplyCloneEffect();
        }

        // Lock skills and reset bar
        LockAllSkills();
        currentBar = 0;
        isSkillReady = false;
        UpdateUIBar();
    }


    #endregion

    #region Skill Effects

    private void ApplySlowEffect()
    {
        // Implement slow effect logic here
        Debug.Log("Slow effect applied!");
        // Example: Slow down enemies for a duration
    }

    private void ApplyStunEffect()
    {
        // Implement stun effect logic here
        Debug.Log("Stun effect applied!");
        // Example: Stun enemies for a duration
    }

    private void ApplyHealEffect()
    {
        // Implement heal effect logic here
        Debug.Log("Heal effect applied!");
        int healAmount = 20 + (healLevel * 5); // Scale with level
        Heal(healAmount);
    }

    private void ApplyCloneEffect()
    {
        // Implement clone effect logic here
        Debug.Log("Clone effect applied!");
        // Example: Create temporary clones or extra hooks
    }
    public void UpdateUIHealth()
    {
        if (hpSlider)
        {
            hpSlider.maxValue = MaxHP;
            hpSlider.value = currentHP;
            hpFill.color = hpGradient.Evaluate(hpSlider.normalizedValue);
        }
    }

    public void UpdateUIExp()
    {
        if (expSlider)
        {
            expSlider.maxValue = MaxExp;
            expSlider.value = currentEXP;
        }
    }

    public void UpdateUIBar()
    {
        if (barSlider)
        {
            barSlider.maxValue = MaxBar;
            barSlider.value = currentBar;
            barFill.color = barGradient.Evaluate(barSlider.normalizedValue);
        }
    }

    #endregion

    #region Misc Logic

    public void OnTriggerEnter(Collider other)
    {
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
            yield return new WaitForSeconds(1f);
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
            Gizmos.DrawLine(new Vector3(leftBoundary, transform.position.y - 2, transform.position.z),
                            new Vector3(leftBoundary, transform.position.y + 2, transform.position.z));
            Gizmos.DrawLine(new Vector3(rightBoundary, transform.position.y - 2, transform.position.z),
                            new Vector3(rightBoundary, transform.position.y + 2, transform.position.z));
            Gizmos.DrawLine(new Vector3(leftBoundary, transform.position.y - 1, transform.position.z),
                            new Vector3(rightBoundary, transform.position.y - 1, transform.position.z));
        }
    }

    #endregion

    #region Upgrade System

    void ShowUpgradePanel()
    {
        if (skillUpgradePanel != null)
        {
            skillUpgradePanel.SetActive(true);
            
        }
    }

    #endregion
}
