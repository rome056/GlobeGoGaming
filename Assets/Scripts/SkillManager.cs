using UnityEngine;
using System.Collections;
using System;

public class SkillManager : MonoBehaviour
{
    [Header("Stun Skill")]
    public float baseStunDuration = 1f;
    public float currentStunValue = 0f;
    public float maxStunValue = 5f;

    [Header("Slow Skill")]
    public float baseSlowDuration = 1f;
    public float newSlowSpeed = 0.5f;
    public float currentSlowValue = 0f;
    public float maxSlowValue = 5f;

    [Header("Heal Skill")]
    public int baseHealAmount = 10;
    public int currentHealValue = 0;
    public float maxHealValue = 20f;

    [Header("Clone Skill")]
    public GameObject clonePrefab;  // ✅ DRAG your CloneSkill prefab here
    public Transform playerTransform;

    private void Start()
    {
        currentStunValue = baseStunDuration;
        currentSlowValue = baseSlowDuration;
        currentHealValue = baseHealAmount;
    }

    #region Clone Skill
    public void ActivateClone()
    {
        if (clonePrefab != null && playerTransform != null)
        {
            Vector3 spawnPoint = playerTransform.position + playerTransform.forward * 1.5f;
            GameObject clone = Instantiate(clonePrefab, spawnPoint, playerTransform.rotation);

            // Add upgrades
            CloneSkill hooker = clone.GetComponent<CloneSkill>();
            if (hooker != null)
            {
                hooker.maxHooks += PlayerController.instance.cloneLevel;
            }
        }
    }
    #endregion

    #region Stun Skill

    public void NewStunValue(float addStunValue)
    {
        currentStunValue += addStunValue;
        currentStunValue = Mathf.Clamp(currentStunValue, 0, maxStunValue);
    }

    public void ActivateStunField()
    {
        //float upgradedStun = baseStunDuration + (PlayerController.instance.stunLevel * 0.5f);
        StartCoroutine(StunAllEnemies());
    }

    IEnumerator StunAllEnemies()
    {
        EnemyFly[] flies = FindObjectsOfType<EnemyFly>();
        EnemySlime[] slimes = FindObjectsOfType<EnemySlime>();
        EnemyBee[] bees = FindObjectsOfType<EnemyBee>();
        EnemyBug[] bugs = FindObjectsOfType<EnemyBug>();
        EnemyHopper[] hoppers = FindObjectsOfType<EnemyHopper>();

        foreach (EnemyFly enemy in flies)
            enemy.Stun(currentStunValue);

        foreach (EnemySlime enemy in slimes)
            enemy.Stun(currentStunValue);

        foreach (EnemyBee enemy in bees)
            enemy.Stun(currentStunValue);
        foreach (EnemyBug enemy in bugs)
            enemy.Stun(currentStunValue);
        foreach (EnemyHopper enemy in hoppers)
            enemy.Stun(currentStunValue);

        yield return null;
    }
    #endregion

    #region Slow Skill

    public void NewSlowValue(float addSlowValue)
    {
        currentSlowValue += addSlowValue;
        currentSlowValue = Mathf.Clamp(currentSlowValue, 0, maxSlowValue);
    }


    public void ActivateSlowField()
    {
        //float upgradedDuration = baseSlowDuration + (PlayerController.instance.slowLevel * 0.5f);
        StartCoroutine(SlowAllEnemies());
    }

    IEnumerator SlowAllEnemies()
    {
        // Hanapin lahat ng enemies na merong SlowEffect method
        EnemySlime[] slimes = FindObjectsOfType<EnemySlime>();
        EnemyFly[] flies = FindObjectsOfType<EnemyFly>();
        EnemyBug[] bugs = FindObjectsOfType<EnemyBug>();
        EnemyHopper[] hoppers = FindObjectsOfType<EnemyHopper>();
        EnemyBee[] bees = FindObjectsOfType<EnemyBee>();

        foreach (EnemySlime slime in slimes)
        {
            slime.SlowEffect(newSlowSpeed, currentSlowValue);
        }

        foreach (EnemyFly fly in flies)
        {
            fly.SlowEffect(newSlowSpeed, currentSlowValue);
        }

        foreach (EnemyBee bee in bees)
        {
            bee.SlowEffect(newSlowSpeed, currentSlowValue);
        }
        foreach (EnemyBug bug in bugs)
        {
            bug.SlowEffect(newSlowSpeed, currentSlowValue);
        }
        foreach (EnemyHopper hopper in hoppers)
        {
            hopper.SlowEffect(newSlowSpeed, currentSlowValue);
        }

        yield return null;
    }
    #endregion

    #region Heal Skill

    public void NewHealValue(int addHealValue)
    {
        currentHealValue += addHealValue;
        currentHealValue = Mathf.Clamp(currentHealValue, 0, Mathf.RoundToInt(maxHealValue));
    }

    public void HealPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            int bonusHeal = currentHealValue + (PlayerController.instance.healLevel * 10);
            player.Heal(bonusHeal);
        }
        else
        {
            Debug.LogWarning("No PlayerController found!");
        }
    }
    #endregion

}
