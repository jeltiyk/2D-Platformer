using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int throwableObjects = 10;
    private int _currentHealth;

    #region Properties

    public int ThrowableObjects
    {
        get => throwableObjects;
        set => throwableObjects = value;
    }

    #endregion

    private void Start()
    {
        _currentHealth = health;
    }

    public void OnChangeHealth(int healthCount)
    {
        _currentHealth += healthCount;

        if (_currentHealth > health)
            _currentHealth = health;

        if (_currentHealth < 1)
            OnDeath();
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }
}
