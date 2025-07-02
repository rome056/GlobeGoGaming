using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{

    public Text limittext;
    public float maxLimit = 15f;
    public float currentLimit = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentLimit = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentLimit++;
            UpdateLimit();
            if (currentLimit >= maxLimit)
            {
                currentLimit = maxLimit;
                Destroy(gameObject);
            }
           
        }
    }

    public void UpdateLimit()
    {
        limittext.text = currentLimit + " / 15";
    }


}
