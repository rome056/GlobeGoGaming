using System.Collections;
using UnityEngine;

public class StunField : MonoBehaviour
{
    public float stunDuration = 3f;

    public void Activate()
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
}
