using UnityEngine;

public class FaceMouseIn3D : MonoBehaviour
{
    public LayerMask raycastLayers; // Layers to raycast against (e.g., ground or objects)
    public float rotationSpeed = 10f;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayers))
        {
            Vector3 targetPoint = hit.point;
            Vector3 direction = targetPoint - transform.position;

            direction.y = 0f; // Freeze vertical rotation

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
