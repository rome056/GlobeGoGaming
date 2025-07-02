using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBee : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban

    public float chargeSpeed = 10f;        // Bilis kapag umatake
    public float detectionRange = 5f;      // Range para ma-detect ang player
    public float chargeDelay = 1f;         // Gaano katagal titigil bago umatake

    private GameObject player;
    private bool isCharging = false;
    private bool isWaitingToCharge = false;
    private float chargeTimer = 0f;
    public bool isHooked = false;  // ← Idedetect kung nahook na


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (isCharging)
        {
            // Dash papunta sa player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * chargeSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Player detected
            if (!isWaitingToCharge)
            {
                isWaitingToCharge = true;
                chargeTimer = chargeDelay;
            }
            else
            {
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f)
                {
                    isCharging = true;
                    isWaitingToCharge = false;
                }
            }
        }
        else
        {
            // Normal movement to the left (patrol)
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Destroy(gameObject);

        }
        if (other.CompareTag("Base"))
        {
            Destroy(gameObject);
        }
        
        {
            if (other.CompareTag("Hook"))
            {
                HookMechanism hook = other.GetComponent<HookMechanism>();
                if (hook != null)
                {
                    isHooked = true;                 // ← I-flag na nahook na ito
                    hook.HookBee(gameObject);        // ← I-hook ang sarili
                    PlayerController.instance.TakeBar(10);
                    PlayerController.instance.TakeExp(10);
                    PlayerController.instance.TakeCountEnemy();
                }
            }
        }

    }

}
