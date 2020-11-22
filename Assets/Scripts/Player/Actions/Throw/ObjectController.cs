using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] int objectDamage = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            if (enemy.Throws)
            {
                enemy.OnTakeDamage(objectDamage);
            }
        }

        Destroy(gameObject);
    }
}
