using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    public float slowDuration = 5f;      // Gaano katagal ang slow effect
    public float newSlowSpeed = 0.5f;    // Bagong speed ng kalaban habang slow

    public void Activate()
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
