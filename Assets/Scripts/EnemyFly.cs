using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban
    public float jumpHeight = 1f;          // Taas ng jump sa Y
    public float jumpDuration = 0.5f;      // Gaano katagal ang isang jump
    public float zigzagOffsetZ = 1f;       // Layo ng left/right zigzag
    private float jumpStartTime;
    private Vector3 jumpStartPos;
    private Vector3 jumpTargetPos;
    private bool isJumping = false;
    public bool isHooked = false; // ← Flag kung nahook na

    private bool jumpRight = true;         
    

    void Update()
    {
        if (isHooked) return;
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (!isJumping)
        {
            // Start new zigzag jump
            isJumping = true;
            jumpStartTime = Time.time;
            jumpStartPos = transform.position;

            float targetZ = jumpStartPos.z + (jumpRight ? zigzagOffsetZ : -zigzagOffsetZ);
            jumpTargetPos = new Vector3(jumpStartPos.x, jumpStartPos.y, targetZ);

            // Next time, other direction naman
            jumpRight = !jumpRight;
        }
        else
        {
            // Calculate progress
            float elapsed = Time.time - jumpStartTime;
            float percent = elapsed / jumpDuration;

            if (percent >= 1f)
            {
                // Done jumping
                isJumping = false;
                transform.position = new Vector3(transform.position.x, jumpStartPos.y, jumpTargetPos.z);
            }
            else
            {
                // Jump motion: pa-curve (Y axis) + zigzag (Z axis)
                float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                float newZ = Mathf.Lerp(jumpStartPos.z, jumpTargetPos.z, percent);
                transform.position = new Vector3(transform.position.x, jumpStartPos.y + yOffset, newZ);
            }
        }
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
                isHooked = true; // ✅ Hinto ang movement
                hook.HookBee(gameObject); // Ipasa ang sarili sa hook
            }
        }
    }



}