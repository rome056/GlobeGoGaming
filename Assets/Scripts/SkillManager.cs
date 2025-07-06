using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public float stunDuration = 3f;
    public float slowDuration = 5f;      // Gaano katagal ang slow effect
    public float newSlowSpeed = 0.5f;    // Bagong speed ng kalaban habang slow
    public int healAmount = 10;
    public GameObject clonePrefab;          // Reference sa CloneSkill script
    public Transform playerTransform;      // Reference sa Player transform

    public void ActivateClone()
    {
        if (clonePrefab != null && playerTransform != null)
        {
            Vector3 spawnPoint = playerTransform.position + playerTransform.forward * 1.5f;
            GameObject clone = Instantiate(clonePrefab, spawnPoint, playerTransform.rotation);

            CloneSkill hooker = clone.GetComponent<CloneSkill>();
            if (hooker != null)
            {
                hooker.maxHooks += PlayerController.instance.cloneLevel;
            }
        }

    }


    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        ActivateSlowField();
    //    }
    //}

    public void ActivateStunField()
    {
        float upgradedStun = stunDuration + (PlayerController.instance.stunLevel * 0.5f);
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
            enemy.Stun(stunDuration);

        foreach (EnemySlime enemy in slimes)
            enemy.Stun(stunDuration);

        foreach (EnemyBee enemy in bees)
            enemy.Stun(stunDuration);
        foreach (EnemyBug enemy in bugs)
            enemy.Stun(stunDuration);
        foreach (EnemyHopper enemy in hoppers)
            enemy.Stun(stunDuration);

        yield return null;
    }

    public void ActivateSlowField()
    {
        float upgradedDuration = slowDuration + (PlayerController.instance.slowLevel * 0.5f);
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
            slime.SlowEffect(newSlowSpeed, slowDuration);
        }

        foreach (EnemyFly fly in flies)
        {
            fly.SlowEffect(newSlowSpeed, slowDuration);
        }

        foreach (EnemyBee bee in bees)
        {
            bee.SlowEffect(newSlowSpeed, slowDuration);
        }
        foreach (EnemyBug bug in bugs)
        {
            bug.SlowEffect(newSlowSpeed, slowDuration);
        }
        foreach (EnemyHopper hopper in hoppers)
        {
            hopper.SlowEffect(newSlowSpeed, slowDuration);
        }

        yield return null;

       
    }

    public void HealPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            int bonusHeal = healAmount + (PlayerController.instance.healLevel * 10);
            player.Heal(bonusHeal);
        }
        else
        {
            Debug.LogWarning("No PlayerController found!");
        }
    }
}
