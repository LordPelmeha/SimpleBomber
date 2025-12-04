using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] public float lifeTime;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Die();
            }
        }
    }
}