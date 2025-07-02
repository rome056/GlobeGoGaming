    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyHopper : MonoBehaviour
    {
        public float speed = 2f; // Bilis ng kalaban
        public float jumpHeight = 1f;
        public float jumpDuration = 0.5f;
        public float jumpInterval = 3f;

        private float jumpTimer;
        private bool isJumping = false;
        private float jumpStartTime;
        private Vector3 originalPosition;
        void Start()
        {
            jumpTimer = jumpInterval;
            originalPosition = transform.position;
        }

        void Update()
        {
            // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
            transform.position += Vector3.left * speed * Time.deltaTime;
            jumpTimer -= Time.deltaTime;

            if (!isJumping && jumpTimer <= 0f)
            {
                isJumping = true;
                jumpStartTime = Time.time;
                originalPosition = transform.position;
                jumpTimer = jumpInterval;
            }

            // Handle fake jump (tataas-bababa lang sa Y)
            if (isJumping)
            {
                float elapsed = Time.time - jumpStartTime;
                float percent = elapsed / jumpDuration;

                if (percent >= 1f)
                {
                    // Jump done
                    isJumping = false;
                    transform.position = new Vector3(transform.position.x, originalPosition.y, transform.position.z);
                }
                else
                {
                    float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;
                    transform.position = new Vector3(transform.position.x, originalPosition.y + yOffset, transform.position.z);
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
        }


    }