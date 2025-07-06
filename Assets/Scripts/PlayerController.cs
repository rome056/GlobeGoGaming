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

    [Header("Upgrade System")]
    public GameObject upgradePanel;   // ← drag mo dito ang UpgradePanel
    public Button upgradeRangeButton;
    public Button upgradeSpeedButton;

    [Header("Skill Unlock System")]
    public Button slowButton;
    public Button stunButton;
    public Button healButton;
    public Button cloneButton;

    private List<Button> skillButtons = new List<Button>();
    private Button unlockedSkill = null;
    // Hook upgrade values
    public float hookRange = 10f;
    public float hookSpeed = 20f;
    public float hookRangeIncrement = 2f;
    public float hookSpeedIncrement = 5f;

    [Header("Skill Upgrade System")]
    public GameObject skillUpgradePanel;
    public Button plusSlowButton;
    public Button plusStunButton;
    public Button plusCloneButton;
    public Button plusHealButton;

    public int slowLevel = 0;
    public int stunLevel = 0;
    public int cloneLevel = 0;
    public int healLevel = 0;

    public int maxSkillLevel = 5;


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

        skillButtons.Add(slowButton);
        skillButtons.Add(stunButton);
        skillButtons.Add(healButton);
        skillButtons.Add(cloneButton);

        LockAllSkills();
        UpdateUIHealth();
        UpdateUIExp();
        UpdateUIBar();
        UpdateCountEnemy();

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (upgradeRangeButton != null)
            upgradeRangeButton.onClick.AddListener(UpgradeRange);

        if (upgradeSpeedButton != null)
            upgradeSpeedButton.onClick.AddListener(UpgradeSpeed);

    }

    private void Update()
    {
        HandleMovement();
        HandleFishing();
        if (currentEXP >= MaxExp)
        {
            ShowUpgradePanel();
        }
        if (currentBar >= MaxBar && unlockedSkill == null)
        {
            UnlockRandomSkill();
        }
        if (currentEXP >= MaxExp && !skillUpgradePanel.activeSelf)
        {
            ShowSkillUpgradeOptions();
        }

    }
    void ShowSkillUpgradeOptions()
    {
        skillUpgradePanel.SetActive(true);
        Time.timeScale = 0f;

        plusSlowButton.interactable = (slowLevel < maxSkillLevel);
        plusStunButton.interactable = (stunLevel < maxSkillLevel);
        plusCloneButton.interactable = (cloneLevel < maxSkillLevel);
        plusHealButton.interactable = (healLevel < maxSkillLevel);
    }
    public void UpgradeSlowSkill()
    {
        if (slowLevel < maxSkillLevel)
        {
            slowLevel++;
            currentEXP = 0;
            CloseSkillUpgradePanel();
            Debug.Log("Slow skill upgraded to level " + slowLevel);
        }
    }

    public void UpgradeStunSkill()
    {
        if (stunLevel < maxSkillLevel)
        {
            stunLevel++;
            currentEXP = 0;
            CloseSkillUpgradePanel();
            Debug.Log("Stun skill upgraded to level " + stunLevel);
        }
    }

    public void UpgradeCloneSkill()
    {
        if (cloneLevel < maxSkillLevel)
        {
            cloneLevel++;
            currentEXP = 0;
            CloseSkillUpgradePanel();
            Debug.Log("Clone skill upgraded to level " + cloneLevel);
        }
    }

    public void UpgradeHealSkill()
    {
        if (healLevel < maxSkillLevel)
        {
            healLevel++;
            currentEXP = 0;
            CloseSkillUpgradePanel();
            Debug.Log("Heal skill upgraded to level " + healLevel);
        }
    }
    public void CloseSkillUpgradePanel()
    {
        skillUpgradePanel.SetActive(false);
        Time.timeScale = 1f;
        UpdateUIExp();
    }

    public void LockAllSkills()
    {
        foreach (Button btn in skillButtons)
            btn.interactable = false;

        unlockedSkill = null;
    }
    public void UnlockRandomSkill()
    {
        int rand = Random.Range(0, skillButtons.Count);
        unlockedSkill = skillButtons[rand];
        unlockedSkill.interactable = true;
        unlockedSkill.onClick.AddListener(OnSkillUsed);
    }
    public void OnSkillUsed()
    {
        LockAllSkills();
        currentBar = 0;
        UpdateUIBar();

        if (unlockedSkill != null)
            unlockedSkill.onClick.RemoveListener(OnSkillUsed);
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
    void ShowUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            Time.timeScale = 0f; // pause game habang pumipili
        }
    }

    void UpgradeRange()
    {
        hookRange += hookRangeIncrement;
        currentEXP = 0;
        CloseUpgradePanel();
        Debug.Log("✅ Upgraded hook range to " + hookRange);
    }

    void UpgradeSpeed()
    {
        hookSpeed += hookSpeedIncrement;
        currentEXP = 0;
        CloseUpgradePanel();
        Debug.Log("✅ Upgraded hook speed to " + hookSpeed);
    }

    void CloseUpgradePanel()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateUIExp();
    }
    
}
