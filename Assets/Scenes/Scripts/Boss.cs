using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 2f; // Bilis ng kalaban
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Gumagalaw ang kalaban papunta sa kaliwa (direksyon ng bahay)
        transform.position += Vector3.left * speed * Time.deltaTime;

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
    }
    }
