using UnityEngine;

public class LoopRotateHorizontal : MonoBehaviour
{
    public float rotationSpeed = 50f; 

    void Update()
    {
        // Rotate around the Y-axis (horizontal spin)
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
