using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetHolder;
    public float followIntensity;

    private float cameraZDepth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraZDepth = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = targetHolder.position;
        targetPosition.z = cameraZDepth;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followIntensity * Time.deltaTime);

    }
}
