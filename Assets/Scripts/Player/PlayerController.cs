using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PCController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHealths = 100;
    [SerializeField] private int startHealth = 100;
    [SerializeField] private int maxObjects = 128;
    [SerializeField] private int startCountObjects = 10;
    [SerializeField] private float shieldBoostTime = 5.0f;

    [SerializeField] private Slider health;
    [SerializeField] private TextMeshProUGUI countObjects;
    [SerializeField] private TextMeshProUGUI countCoins;

    private SceneController _sceneController;

    private int _currentHealth;
    private int _currentObjects;
    private int _currentCoins;
    private bool _toNextLevel;
    private bool _damaged;

    #region Properties
    
    public int CurrentHealth => _currentHealth;
    public int CurrentObjects => _currentObjects;
    public int CurrentCoins => _currentCoins;
    public bool ToNextLevel => _toNextLevel;
    
    #endregion

    private float _timeLeft;
    private bool _damageResistance;
    private TextMeshProUGUI _currentShield;
    private TextMeshProUGUI _currentShieldCount;
    private int _shieldBoostCount;

    private TextMeshProUGUI _currentHealthText;
    
    public int CurrentShieldCount => _shieldBoostCount;

    private void Start()
    {
        // PlayerPrefs.DeleteAll();
        // return;

        _sceneController = SceneController.Instance;

        _currentShield = _sceneController.canvas.transform.GetChild(2).GetChild(_sceneController.canvas.transform.GetChild(2).childCount - 1).GetChild(0).GetChild(0)
            .GetComponentInChildren<TextMeshProUGUI>(true);
        _currentShieldCount = _currentShield.transform.parent.parent.GetChild(1)
            .GetComponentInChildren<TextMeshProUGUI>(true);

        _currentHealth = PlayerPrefs.GetInt("Player Health", startHealth);
        _currentObjects = PlayerPrefs.GetInt("PineCones Count", startCountObjects);
        _currentCoins = PlayerPrefs.GetInt("Coins Count", 0);
        _shieldBoostCount = PlayerPrefs.GetInt("Shield Boost Count", 0);

        _currentShield.SetText(Math.Round(shieldBoostTime, 2).ToString());

        if (_shieldBoostCount == 1)
            _currentShield.transform.parent.gameObject.SetActive(true);
        else if (_shieldBoostCount > 1)
        {
            _currentShield.transform.parent.gameObject.SetActive(true);
            _currentShieldCount.SetText( "x" + (_shieldBoostCount - 1));
            _currentShieldCount.transform.parent.gameObject.SetActive(true);
        }

        _currentHealthText = health.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        health.value = _currentHealth;
        _currentHealthText.text = _currentHealth.ToString();
        countObjects.SetText(_currentObjects.ToString());
        countCoins.SetText(_currentCoins.ToString());
    }

    private void LateUpdate()
    {
        if(_damageResistance || _shieldBoostCount < 1 || !Input.GetKeyDown(KeyCode.F)) return;
        
        StartCoroutine(BoostController(shieldBoostTime));
    }


    private IEnumerator BoostController(float time)
    {
        _damageResistance = true;
        _shieldBoostCount--;

        _currentShield.transform.parent.gameObject.SetActive(true);
        _currentShield.SetText(Math.Round(_timeLeft, 1).ToString());
        _currentShieldCount.SetText("x" + _shieldBoostCount);

        _timeLeft = time;
        
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            _currentShield.SetText(Math.Round(_timeLeft, 1).ToString());
            
            yield return new WaitForSeconds(0.001f);
        }

        if(_shieldBoostCount < 1)
            _currentShield.transform.parent.gameObject.SetActive(false);
        
        if (_shieldBoostCount < 2)
            _currentShieldCount.transform.parent.gameObject.SetActive(false);
        
        _currentShield.SetText(shieldBoostTime.ToString());
        
        _damageResistance = false;
    }

    public void OnChangeHealth(int healthCount)
    {
        if(_damageResistance && healthCount < 0) return;
        
        _currentHealth += healthCount;

        if (_currentHealth > maxHealths)
            health.value = _currentHealth = maxHealths;

        health.value = _currentHealth;
        _currentHealthText.text = _currentHealth.ToString();

        if (_currentHealth < 1)
            OnDeath();
    }

    public void OnChangePineCones(int count)
    {
        _currentObjects += count;
        
        if (_currentObjects > maxObjects)
            _currentObjects = maxObjects;
        
        countObjects.SetText(_currentObjects.ToString());
    }

    public void OnChangeCoins(int count)
    {
        _currentCoins += count;
        if (_currentCoins < 0)
            _currentCoins = 0;
        
        countCoins.SetText(_currentCoins.ToString());
    }
    
    public void OnChangeLevelState(bool state)
    {
        _toNextLevel = state;
        _sceneController.canvas.transform.GetChild(0).GetChild(_sceneController.canvas.transform.GetChild(0).childCount - 1).GetChild(0).gameObject.SetActive(true);
    }

    public void OnChangeShieldBoost()
    {
        if (_shieldBoostCount > 0)
        {
            // Damage Resistance Panel 1
            _currentShieldCount.transform.parent.gameObject.SetActive(true);
            _currentShieldCount.SetText("x" + _shieldBoostCount++);
            return;
        }
        
        _shieldBoostCount++;
        // Damage Resistance Panel
        _currentShield.transform.parent.gameObject.SetActive(true);
    }

    private void OnDeath()
    {
        // Destroy(gameObject);
        _sceneController.RestartScene();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(_damaged) return;
        _damaged = true;
        
        var megaEnemyController = other.collider.GetComponent<MegaEnemyController>();

        if (megaEnemyController != null && megaEnemyController.CurrentAttackType != MegaEnemyController.AttackType.Idle)
        {
            OnChangeHealth(-megaEnemyController.HitDamage);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _damaged = false;
    }
}
