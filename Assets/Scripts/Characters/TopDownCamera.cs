using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;         

    [Header("Положение камеры")]
    [Range(0f, 45f)]
    public float tiltAngle = 30f;       
    public float distance = 12f;        
    public float smoothSpeed = 5f;      

    void Start()
    {
        transform.rotation = Quaternion.Euler(90f - tiltAngle, 0f, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Смещение "назад и вверх" вдоль оси камеры
        Vector3 offset = Quaternion.Euler(90f - tiltAngle, 0f, 0f) * Vector3.back * distance;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}