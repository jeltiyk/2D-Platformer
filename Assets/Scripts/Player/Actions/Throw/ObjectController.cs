using System;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private int objectDamage = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemyController = other.GetComponent<EnemyController>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        MegaEnemyController megaEnemyController = other.GetComponent<MegaEnemyController>();
        
        // 1 << 16 'Enemy Action' layer
        if (enemyController != null && enemyController.Throws && gameObject.layer != 1 << 16)
        {
            enemyController.OnTakeDamage(objectDamage);
            Destroy(gameObject);
            return;
        }

        // 1 << 9 'Player Actions' layer
        if (playerController != null && gameObject.layer != 1 << 9)
        {
            playerController.OnChangeHealth(-objectDamage);
            Destroy(gameObject);
            return;
        }
        
        // 1 << 16 'Enemy Action' layer
        if (megaEnemyController != null && gameObject.layer != 1 << 16 && megaEnemyController.CurrentAttackType != MegaEnemyController.AttackType.Defense)
        {
            megaEnemyController.OnTakeDamage(objectDamage);
            Destroy(gameObject);
            return;
        }
        
        Destroy(gameObject);
    }
}
