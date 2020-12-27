using System;
using UnityEngine;

public class MEHitColliderController : MonoBehaviour
{
    [SerializeField] private int hitDamage = 250;
    private MegaEnemyController _megaEnemyController;
    private bool _damaged;
    private void Awake()
    {
        _megaEnemyController = transform.GetComponentInParent<MegaEnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_damaged) return;

        var player = other.GetComponent<PlayerController>();
        
        if (!player) return;
        
        _megaEnemyController.OnTakeDamage(hitDamage);
        _damaged = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _damaged = false;
    }
}
