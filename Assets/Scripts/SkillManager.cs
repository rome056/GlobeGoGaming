using UnityEngine;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public float stunDuration = 3f;
    public float slowDuration = 5f;      // Gaano katagal ang slow effect
    public float newSlowSpeed = 0.5f;    // Bagong speed ng kalaban habang slow

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        ActivateSlowField();
    //    }
    //}

    public void ActivateStunField()
    {
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
}
