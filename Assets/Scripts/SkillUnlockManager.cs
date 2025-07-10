using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUnlockManager : MonoBehaviour
{
    [Header("Skill Buttons")]
    public Button stunSkillButton;
    public Button slowSkillButton;
    public Button healSkillButton;
    public Button cloneSkillButton;

    [Header("Skill Bar")]
    public Slider skillBar;
    public float maxSkillValue = 100f;
    public float currentSkillValue = 0f;

    [Header("Exp Bar")]
    public Slider expBar;
    public Button plusButton;
    public float maxExpValue = 100f;
    public float currentExpValue = 0f;

    [Header("Upgrade Panel")]
    public GameObject skillUpgradePanel;

    private SkillManager skillManager;
    private List<Button> allSkillButtons = new List<Button>();
    private List<Button> lockedSkillButtons = new List<Button>();
    private Button currentUnlockedSkill = null;

    // Skill types enum for easier management
    public enum SkillType
    {
        Stun,
        Slow,
        Heal,
        Clone
    }

    private void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();
        InitializeSkillSystem();
    }

    private void InitializeSkillSystem()
    {
        // Add all skill buttons to list
        allSkillButtons.Add(stunSkillButton);
        allSkillButtons.Add(slowSkillButton);
        allSkillButtons.Add(healSkillButton);
        allSkillButtons.Add(cloneSkillButton);

        // Lock all skills initially
        LockAllSkills();

        // Initialize bars
        UpdateSkillBar();
        UpdateExpBar();

        // Hide plus button initially
        if (plusButton != null)
            plusButton.gameObject.SetActive(false);

        // Hide upgrade panel initially
        if (skillUpgradePanel != null)
            skillUpgradePanel.SetActive(false);

        // Set up button listeners
        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (stunSkillButton != null)
            stunSkillButton.onClick.AddListener(() => UseSkill(SkillType.Stun));

        if (slowSkillButton != null)
            slowSkillButton.onClick.AddListener(() => UseSkill(SkillType.Slow));

        if (healSkillButton != null)
            healSkillButton.onClick.AddListener(() => UseSkill(SkillType.Heal));

        if (cloneSkillButton != null)
            cloneSkillButton.onClick.AddListener(() => UseSkill(SkillType.Clone));

        if (plusButton != null)
            plusButton.onClick.AddListener(ShowUpgradePanel);
    }

    private void LockAllSkills()
    {
        lockedSkillButtons.Clear();
        currentUnlockedSkill = null;

        foreach (Button button in allSkillButtons)
        {
            if (button != null)
            {
                button.interactable = false;
                lockedSkillButtons.Add(button);

                // Optional: Change button appearance to show locked state
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // Darker/grayed out
                }
            }
        }
    }

    private void UnlockRandomSkill()
    {
        if (lockedSkillButtons.Count == 0)
            return;

        // Choose random skill from locked skills
        int randomIndex = Random.Range(0, lockedSkillButtons.Count);
        Button skillToUnlock = lockedSkillButtons[randomIndex];

        // Unlock the skill
        skillToUnlock.interactable = true;
        currentUnlockedSkill = skillToUnlock;
        lockedSkillButtons.RemoveAt(randomIndex);

        // Change button appearance to show unlocked state
        Image buttonImage = skillToUnlock.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white; // Normal color
        }

        Debug.Log($"Unlocked skill: {skillToUnlock.name}");
    }

    private void UseSkill(SkillType skillType)
    {
        if (currentUnlockedSkill == null)
            return;

        // Execute the skill
        switch (skillType)
        {
            case SkillType.Stun:
                if (skillManager != null)
                    skillManager.ActivateStunField();
                break;

            case SkillType.Slow:
                if (skillManager != null)
                    skillManager.ActivateSlowField();
                break;

            case SkillType.Heal:
                if (skillManager != null)
                    skillManager.HealPlayer();
                break;

            case SkillType.Clone:
                if (skillManager != null)
                    skillManager.ActivateClone();
                break;
        }

        // Lock the skill again after use
        LockSkillAfterUse();

        // Reset skill bar
        ResetSkillBar();

        Debug.Log($"Used skill: {skillType}");
    }

    private void LockSkillAfterUse()
    {
        if (currentUnlockedSkill != null)
        {
            currentUnlockedSkill.interactable = false;
            lockedSkillButtons.Add(currentUnlockedSkill);

            // Change button appearance back to locked state
            Image buttonImage = currentUnlockedSkill.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }

            currentUnlockedSkill = null;
        }
    }

    public void AddSkillValue(float value)
    {
        currentSkillValue += value;
        currentSkillValue = Mathf.Clamp(currentSkillValue, 0f, maxSkillValue);

        UpdateSkillBar();

        // Check if skill bar is full
        if (currentSkillValue >= maxSkillValue && currentUnlockedSkill == null)
        {
            UnlockRandomSkill();
            // Don't reset skill bar yet - wait for skill to be used
        }
    }

    public void AddExpValue(float value)
    {
        currentExpValue += value;
        currentExpValue = Mathf.Clamp(currentExpValue, 0f, maxExpValue);

        UpdateExpBar();

        // Check if exp bar is full
        if (currentExpValue >= maxExpValue)
        {
            ShowPlusButton();
        }
    }

    private void UpdateSkillBar()
    {
        if (skillBar != null)
        {
            skillBar.value = currentSkillValue / maxSkillValue;
        }
    }

    private void UpdateExpBar()
    {
        if (expBar != null)
        {
            expBar.value = currentExpValue / maxExpValue;
        }
    }

    private void ShowPlusButton()
    {
        if (plusButton != null)
        {
            plusButton.gameObject.SetActive(true);
        }
    }

    private void ShowUpgradePanel()
    {
        if (skillUpgradePanel != null)
        {
            skillUpgradePanel.SetActive(true);
        }

        // Reset exp bar to zero
        ResetExpBar();

        // Hide plus button
        if (plusButton != null)
        {
            plusButton.gameObject.SetActive(false);
        }
    }

    private void ResetSkillBar()
    {
        currentSkillValue = 0f;
        UpdateSkillBar();
    }

    private void ResetExpBar()
    {
        currentExpValue = 0f;
        UpdateExpBar();
    }

    // Public methods for external scripts to call
    public void OnEnemyKilled(float skillPoints, float expPoints)
    {
        AddSkillValue(skillPoints);
        AddExpValue(expPoints);
    }

    public void OnHookSuccess(float skillPoints)
    {
        AddSkillValue(skillPoints);
    }

    // Method to check if any skill is currently unlocked
    public bool HasUnlockedSkill()
    {
        return currentUnlockedSkill != null;
    }

    // Method to get current unlocked skill type
    public SkillType? GetCurrentUnlockedSkillType()
    {
        if (currentUnlockedSkill == null)
            return null;

        if (currentUnlockedSkill == stunSkillButton)
            return SkillType.Stun;
        else if (currentUnlockedSkill == slowSkillButton)
            return SkillType.Slow;
        else if (currentUnlockedSkill == healSkillButton)
            return SkillType.Heal;
        else if (currentUnlockedSkill == cloneSkillButton)
            return SkillType.Clone;

        return null;
    }
}