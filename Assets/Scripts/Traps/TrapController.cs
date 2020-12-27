using UnityEngine;
using System;

public class TrapController : MonoBehaviour
{
    [SerializeField] private int trapDamage;
    [SerializeField] private float damageDelay;

    private DateTime _lastEncounter;
    private PlayerController _playerController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _playerController = other.GetComponent<PlayerController>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_playerController == other.GetComponent<PlayerController>())
            _playerController = null;
    }

    private void LateUpdate()
    {
        if (_playerController == null || !((DateTime.Now - _lastEncounter).TotalSeconds > damageDelay)) return;
        
        _lastEncounter = DateTime.Now;
        _playerController.OnChangeHealth(-trapDamage);
    }
}
