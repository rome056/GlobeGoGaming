using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    

    public float speed = 2f; // Bilis ng kalaban
    public GameObject smallEnemyPrefab;
    public float spreadRadius = 1.5f;


    void Update()
    {
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

    }
    public void OnHooked()
    {
        SplitIntoSmallerEnemies();
        Destroy(gameObject); // Patayin ang kalaban
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.TakeExp(10);
            PlayerController.instance.TakeCountEnemy();
            PlayerController.instance.TakeBar(10);
            Destroy(gameObject);

        }
        if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
        if (other.CompareTag("Hook"))
        {
            HookMechanism hook = other.GetComponent<HookMechanism>();
            if (hook != null)
            {
                hook.HookBee(gameObject); // Mahuhook siya
                OnHooked(); // Trigger destroy & spawn
            }
        }
    }
    void SplitIntoSmallerEnemies()
    {
        for (int i = 0; i < 4; i++)
        {
            // Random offset position (para kumalat ng kaunti)
            Vector3 offset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                0f,
                Random.Range(-spreadRadius, spreadRadius)
            );

            Vector3 spawnPos = transform.position + offset;
            Instantiate(smallEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}