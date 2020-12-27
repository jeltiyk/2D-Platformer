using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MegaEnemyController : MonoBehaviour
{
    public enum AngryState
    {
        Idle,
        Low,
        Middle,
        High
    }
    
    public enum AttackType
    {
        Idle,
        Hit,
        Throw,
        Defense,
        MultipleHit
    }
    
    public enum DefenceType
    {
        Low,
        Middle,
        High,
    }
    
    private Slider _healthSlider;
    private Rigidbody2D _enemyRb;
    private Renderer _enemyRendered;
    private Animator _enemyAnimator;
    private Collider2D _headCollider;
    private SpriteRenderer _spriteRenderer;
    private SceneController _sceneController;
    
    private AngryState _currentAngryState;
    private AttackType _currentAttackType;
    
    private float _currentHealth;
    
    private bool _hit;
    private bool _throw;
    private bool _defence;
    private int _throwHitsCount;
    private int _hitsCount;
    
    private bool _faceRight = true;
    private bool _damaged;

    private float _distanceTo;

    private float _lastChangeAttackTypeTime;
    private float _attackTypeChangeDelay;
    
    private AttackType[] _attackTypes;
    
    private TextMeshProUGUI _currentHealthText;

    [SerializeField] private int health;
    [SerializeField] protected float speed = 5f;
    [SerializeField] private float speedModifierMiddle = 1.5f;
    [SerializeField] private float speedModifierHigh = 2f;
    [SerializeField] private GameObject throwObject;
    [SerializeField] protected float hitForce = 500f;
    [SerializeField] protected int hitDamage;
    
    [Header("Field of vision")]
    [SerializeField] private Transform visionPoint;
    [SerializeField] private float rangeOfX;
    [SerializeField] private float rangeOfY;
    [SerializeField] private float throwableRadius;
    [SerializeField] private float hittableRadius;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask playerLayer;
    
    [SerializeField] private GameObject bordersObject;

    [Header("Raycast")]
    [SerializeField] private float rayLength = 1f;
    [SerializeField] private Transform raycastPosition;
    // [SerializeField] private Transform raycastBackPosition;
    [SerializeField] private GameObject[] ignoreObjects;
    
    [Header("Destroy")] 
    [SerializeField] private float time = 0.25f;
    
    public int HitDamage => hitDamage;
    
    public float CurrentHealth => _currentHealth;
    
    public AttackType CurrentAttackType => _currentAttackType;

    private void Awake()
    {
        _enemyRb = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
        _enemyRendered = GetComponent<Renderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _headCollider = GetComponentInChildren<MEHitColliderController>().GetComponentInChildren<BoxCollider2D>();
    }

    private void Start()
    {
        _sceneController = SceneController.Instance;
        //coins
        _sceneController.canvas.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
        // enemy health bar
        _sceneController.canvas.transform.GetChild(2).GetChild(1).transform.gameObject.SetActive(true);
        _healthSlider = _sceneController.canvas.transform.GetChild(2).GetChild(1).GetChild(0)
            .GetComponentInChildren<Slider>();
        _currentHealthText = _sceneController.canvas.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0)
            .GetComponentInChildren<TextMeshProUGUI>();
        
        _currentHealth = _healthSlider.value = _healthSlider.maxValue = health;
        _currentHealthText.text = _currentHealth.ToString();

        _distanceTo = throwableRadius - hittableRadius;

        _attackTypes = new AttackType[10];
        
        ChangeAngryState();
        GetRandomAttackType(true);
    }

    private void FixedUpdate()
    {
        if (!bordersObject.activeSelf)
        {
            _enemyRb.simulated = false;
            return;
        }

        _enemyRb.simulated = true;

        _enemyAnimator.SetFloat("Speed", Mathf.Abs(_enemyRb.velocity.x));

        if (Time.time - _lastChangeAttackTypeTime > _attackTypeChangeDelay)
        {
            GetRandomAttackType();
        }
        
        if (Physics2D.OverlapBox(visionPoint.position, new Vector2(rangeOfX, rangeOfY), 0, playerLayer))
        {
            if (playerTransform.position.x - 3f > transform.position.x && !_faceRight)
               Flip();
            
            if (playerTransform.position.x + 3f < transform.position.x && _faceRight)
                Flip();
        }

        if (_currentAttackType == AttackType.Idle)
            _spriteRenderer.color = Color.Lerp(Color.gray, Color.green, Mathf.PingPong(Time.time * 4, 1));

        switch (_currentAngryState)
        {
            case AngryState.Idle:
            {
                break;
            }
            case AngryState.Low:
            {
                switch (_currentAttackType)
                {
                    case AttackType.Hit:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, hittableRadius, playerLayer) && !_hit)
                        {
                            Move();
                            
                            break;
                        }
                
                        StartCoroutine(HitController(1));
                        
                        break;
                    }
                    case AttackType.Throw:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, throwableRadius, playerLayer) && !_throw)
                        {
                            Move();
                            
                            break;
                        }

                        StartCoroutine(ThrowController(AngryState.Low));
                        
                        break;
                    }
                    case AttackType.Defense:
                    {
                        #region Retreats

                        if (Physics2D.OverlapCircle(visionPoint.position, throwableRadius, playerLayer))
                        {
                            float hittableRadiusPointX;
                
                            if (_faceRight)
                                hittableRadiusPointX = visionPoint.position.x + hittableRadius - playerTransform.position.x;
                            else
                                hittableRadiusPointX = visionPoint.position.x - hittableRadius - playerTransform.position.x;
                
                            if (Mathf.Abs(hittableRadiusPointX) <= _distanceTo)
                            {
                                _enemyRb.velocity = new Vector2(transform.right.x * -speed * 4, transform.up.y * speed);
                                
                                GetRandomAttackType(true);
                                // StartCoroutine(DefenseController(DefenceType.Low));
                            }
                        }
                        

                        #endregion
                        
                        break;
                    }
                    case AttackType.MultipleHit:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, hittableRadius, playerLayer) && !_hit)
                        {
                            Move(2);
                            
                            break;
                        }

                        StartCoroutine(MultipleHitController(0.5f));
                        
                        break;
                    }
                }
                
                break;
            }
            case AngryState.Middle:
            {
                switch (_currentAttackType)
                {
                    case AttackType.Hit:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, hittableRadius, playerLayer) && !_hit)
                        {
                            Move(speedModifierMiddle);
                            
                            break;
                        }
                        
                        StartCoroutine(HitController(0.75f));
                        
                        break;
                    }

                    case AttackType.Throw:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, throwableRadius, playerLayer) && !_throw)
                        {
                            Move(speedModifierMiddle);
                            
                            break;
                        }
                        
                        StartCoroutine(ThrowController(AngryState.Middle));

                        break;
                    }

                    case AttackType.Defense:
                    {
                        StartCoroutine(DefenseController(DefenceType.Middle));
                        
                        break;
                    }
                }
                
                break;
            }
            case AngryState.High:
            {
                switch (_currentAttackType)
                {
                    case AttackType.Hit:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, hittableRadius, playerLayer) && !_hit)
                        {
                            Move(speedModifierHigh);
                            
                            break;
                        }
                        
                        StartCoroutine(HitController(0.5f));
                        
                        break;
                    }

                    case AttackType.Throw:
                    {
                        if (!Physics2D.OverlapCircle(visionPoint.position, throwableRadius, playerLayer) && !_throw)
                        {
                            Move(speedModifierHigh);
                            
                            break;
                        }
                        
                        StartCoroutine(ThrowController(AngryState.High));

                        break;
                    }

                    case AttackType.Defense:
                    {
                        StartCoroutine(DefenseController(DefenceType.High));
                        
                        break;
                    }
                }
                
                break;
            }
        }
    }
    
    private void Move(float modifier = 1)
    {
        RaycastCheck();
        _enemyRb.velocity = new Vector2(transform.right.x * speed * modifier, _enemyRb.velocity.y);
    }

    #region For next AI development

    // private IEnumerator Jump()
    // {
    // yield return new WaitForSeconds(1f);

        
    // RaycastHit2D info = Physics2D.CircleCast(
    // new Vector2(_enemyRendered.bounds.center.x, _enemyRendered.bounds.max.y + 1.5f), 1.5f, Vector2.up);

    // if(info)
    // return;
                            
    // StartCoroutine(Jump());
        
        
    // float jumpForce = 24f;
        
    // if(Physics2D.OverlapBox(new Vector2(transform.position.x, _enemyRendered.bounds.min.y - 0.1f), new Vector2(1, 0.1f), 0, 1 << 8))
    // _enemyRb.velocity = new Vector2(_enemyRb.velocity.x, transform.up.y * jumpForce);
    // }

    #endregion

    private IEnumerator HitController(float delayTime)
    {
        if(_hit) yield break;
        _hit = true;
        
        _enemyRb.velocity = new Vector2(transform.right.x * hitForce, _enemyRb.velocity.y);
        yield return new WaitForSeconds(delayTime / 2);
        
        _enemyRb.velocity = new Vector2(transform.right.x * -hitForce, _enemyRb.velocity.y);
        yield return new WaitForSeconds(delayTime);

        yield return new WaitForSeconds(1f);
        
        _hit = false;
        GetRandomAttackType(true);
    }

    private IEnumerator MultipleHitController(float delayTime)
    {
        if(_hit) yield break;
        _hit = true;
        
        int repeats = 3;

        while (repeats > 0)
        {
            _enemyRb.velocity = new Vector2(transform.right.x * hitForce, _enemyRb.velocity.y);
            yield return new WaitForSeconds(delayTime / 2);
        
            _enemyRb.velocity = new Vector2(transform.right.x * -hitForce, _enemyRb.velocity.y);
            yield return new WaitForSeconds(delayTime);
            
            repeats--;
        }

        _hit = false;
        GetRandomAttackType(true);
    }
    
    private IEnumerator ThrowController(AngryState angryState)
    {
        if(_throw) yield break;
        _throw = true;
        
        GameObject throwableObject = Instantiate(throwObject, raycastPosition.transform.position, Quaternion.identity);
        throwableObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(playerTransform.position.x - raycastPosition.position.x,
            playerTransform.position.y - raycastPosition.position.y), ForceMode2D.Impulse);
        Destroy(throwableObject, 7f);

        switch (angryState)
        {
            case AngryState.Low:
            {
                yield return new WaitForSeconds(3f);
                
                break;
            }
            case AngryState.Middle:
            {
                yield return new WaitForSeconds(2.5f);
                
                break;
            }
            case AngryState.High:
            {
                yield return new WaitForSeconds(1.5f);
                
                break;
            }
                
        }

        _throw = false;
    }
    
    private IEnumerator DefenseController(DefenceType defenceType)
    {
        _spriteRenderer.color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(Time.time * 8, 1));

        if(_defence) yield break;
        _defence = true;
        
        switch (defenceType)
        {
            case DefenceType.Low:
            {
                break;
            }
            case DefenceType.Middle:
            {
                yield return new WaitForSeconds(2f);

                int repeats = 3;

                while (repeats > 0)
                {
                    int objects = 5;
                    GameObject[] throwableObjects = new GameObject[objects + 1];

                    int angleStep = objects * 6 / objects;
                    int upperAngularBorder = objects / 2 * angleStep;
                    int currentAngle = upperAngularBorder;

                    while (objects > 0)
                    {
                        throwableObjects[objects] =
                            Instantiate(throwObject,
                                new Vector2(raycastPosition.position.x, raycastPosition.position.y + 1f),
                                Quaternion.identity);
                        throwableObjects[objects].GetComponent<Rigidbody2D>()
                            .AddForce(
                                (transform.right + transform.up * Mathf.Tan(currentAngle)) *
                                speed,
                                ForceMode2D.Impulse);

                        Destroy(throwableObjects[objects], 7f);
                        objects--;
                        currentAngle -= angleStep;
                    }

                    repeats--;
                    yield return new WaitForSeconds(0.5f);
                }

                break;
            }
            case DefenceType.High:
            {
                yield return new WaitForSeconds(1f);
                
                #region grow up

                for (float s = transform.localScale.x; s < 1.5f; s += 0.025f)
                {
                    transform.localScale = new Vector3(s, s, 0);
                    yield return new WaitForSeconds(0.125f);
                }

                #endregion

                int repeats = 3;

                while (repeats > 0)
                {
                    int objects = 9;
                    GameObject[] throwableObjects = new GameObject[objects + 1];

                    int angleStep = objects * 12 / objects;
                    int upperAngularBorder = objects / 2 * angleStep;
                    int currentAngle = upperAngularBorder;

                    while (objects > 0)
                    {
                        throwableObjects[objects] =
                            Instantiate(throwObject,
                                new Vector2(visionPoint.position.x, _enemyRendered.bounds.max.y + 0.5f),
                                Quaternion.identity);
                        throwableObjects[objects].GetComponent<Rigidbody2D>().gravityScale = 0.3f;
                        throwableObjects[objects].GetComponent<Rigidbody2D>()
                            .AddForce(
                                (transform.up + transform.right * Mathf.Tan(currentAngle)) *
                                speed,
                                ForceMode2D.Impulse);

                        Destroy(throwableObjects[objects], 7f);
                        objects--;
                        currentAngle -= angleStep;
                    }

                    repeats--;
                    yield return new WaitForSeconds(0.5f);
                }

                if (_faceRight)
                    transform.position = new Vector2(playerTransform.position.x + hittableRadius + 1,
                        transform.position.y);
                else
                    transform.position = new Vector2(playerTransform.position.x - hittableRadius - 1,
                        transform.position.y);
                
                yield return new WaitForSeconds(0.25f);

                #region grow down

                for (float s = transform.localScale.x; s > 1; s -= 0.025f)
                {
                    transform.localScale = new Vector3(s, s, 0);
                    yield return new WaitForSeconds(0.025f);
                }

                #endregion
                
                break;
            }
        }
        
        yield return new WaitForSeconds(2f);

        _defence = false;
        GetRandomAttackType(true);
        _spriteRenderer.color = Color.white;
    }
    
    private void ChangeAngryState()
    {
        // F.e: health = 3000, health / 3 * 2 = 2000, etc.
        if (_currentHealth <= health && _currentHealth > health / 3 * 2)
        {
            _currentAngryState = AngryState.Low;
            
            for (int i = 0; i < _attackTypes.Length; i++)
            {
                if(i < 3)
                    _attackTypes[i] = AttackType.Idle;
                else if (i < 5)
                    _attackTypes[i] = AttackType.Hit;
                else if (i < 9)
                    _attackTypes[i] = AttackType.Throw;
                else if (i < 10)
                    _attackTypes[i] = AttackType.Defense;
            }
            
            return;
        }

        if (_currentHealth <= health / 3 * 2 && _currentHealth > health / 3)
        {
            _currentAngryState = AngryState.Middle;
            
            for (int i = 0; i < _attackTypes.Length; i++)
            {
                if(i < 2)
                    _attackTypes[i] = AttackType.Idle;
                else if (i < 5)
                    _attackTypes[i] = AttackType.Hit;
                else if (i < 8)
                    _attackTypes[i] = AttackType.Throw;
                else if (i < 10)
                    _attackTypes[i] = AttackType.Defense;
            }
            return;
        }

        if (_currentHealth <= health / 3)
        {
            _currentAngryState = AngryState.High;

            for (int i = 0; i < _attackTypes.Length; i++)
            {
                if(i < 2)
                    _attackTypes[i] = AttackType.Idle;
                else if (i < 4)
                    _attackTypes[i] = AttackType.Hit;
                else if (i < 7)
                    _attackTypes[i] = AttackType.Throw;
                else if (i < 10)
                    _attackTypes[i] = AttackType.Defense;
            }
        }
    }
    
    private void GetRandomAttackType(bool ignore = false)
    {
        if(!ignore)
            switch (_currentAttackType)
            {
                case AttackType.Hit:
                    return;
                case AttackType.Defense:
                    return;
                case AttackType.MultipleHit:
                    return;
            }
        
        int attackType = Random.Range(0, _attackTypes.Length);
        ChangeAttackType(_attackTypes[attackType]);
    }

    private void ChangeAttackType(AttackType attackType)
    {
        if (_currentAttackType == AttackType.Idle)
        {
            _spriteRenderer.color = Color.white;
            _headCollider.enabled = false;
        }
        
        _currentAttackType = attackType;

        if (_currentAttackType == AttackType.Idle)
            _headCollider.enabled = true;

        _lastChangeAttackTypeTime = Time.time;
        _attackTypeChangeDelay = Random.Range(0, 10);

        // Debug.Log(_currentAttackType);
    }

    private void RaycastCheck()
    {
        RaycastHit2D info = Physics2D.Raycast(raycastPosition.transform.position, raycastPosition.transform.right, rayLength);
            
        if (info)
        {
            foreach (GameObject o in ignoreObjects)
            {
                if (info.transform.name == o.transform.name)
                    return;
            }
    
            Flip();
        }
    }
    
    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0f, 180, 0);
    }

    public void OnTakeDamage(int damageCount)
    {
        if (_currentAttackType == AttackType.Idle)
            _hitsCount++;
        
        if (_hitsCount > 2)
        {
            _hitsCount = 0;
            GetRandomAttackType(true);
            return;
        }

        _currentHealth -= damageCount;
        _healthSlider.value = _currentHealth;
        _currentHealthText.text = _currentHealth.ToString();
        
        if(_currentHealth < 1)
            OnDeath();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_currentAttackType == AttackType.Defense || _damaged) return;
        
        if (!other.GetComponent<ObjectController>() ||
            !Physics2D.OverlapBox(visionPoint.position, new Vector2(rangeOfX, rangeOfY), 0, playerLayer)) return;
        
        // 1 << 16 'Enemy Actions' layer
        if(other.GetComponent<ObjectController>().gameObject.layer == 1 << 16) return;

        _damaged = true;
        
        _throwHitsCount++;

        ChangeAngryState();

        switch (_currentAngryState)
        {
            case AngryState.Low:
            {
                if (_throwHitsCount > 16)
                {
                    ChangeAttackType(AttackType.MultipleHit);
                    _throwHitsCount = 0;
                }
                    
                break;
            }
            case AngryState.Middle:
            {
                if (_throwHitsCount > 12)
                {
                    ChangeAttackType(AttackType.Defense);
                    _throwHitsCount = 0;
                }
                    
                break;
            }
            case AngryState.High:
            {

                if (_throwHitsCount > 4)
                {
                    if (_faceRight)
                        transform.position = new Vector2(playerTransform.position.x + hittableRadius + 1, transform.position.y);
                    else
                        transform.position = new Vector2(playerTransform.position.x - hittableRadius - 1, transform.position.y);
                }

                if (_throwHitsCount > 8)
                {
                    ChangeAttackType(AttackType.Defense);
                    _throwHitsCount = 0;
                }
                    
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _damaged = false;
    }

    private void OnDeath()
    {
        _enemyAnimator.SetBool("Death", true);
        playerTransform.GetComponent<PlayerController>().OnChangeLevelState(true);
        _sceneController.transform.GetChild(1).gameObject.SetActive(true);
        
        // enemy health bar
        _sceneController.canvas.transform.GetChild(2).GetChild(1).transform.gameObject.SetActive(false);
        //coins
        _sceneController.canvas.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
        Destroy(gameObject, time);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(visionPoint.transform.position, new Vector2(rangeOfX, rangeOfY));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(visionPoint.transform.position, throwableRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(visionPoint.transform.position, hittableRadius);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + 2f), 1.5f);
        // Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y - 2f), new Vector2(3, 0.1f));
    }
}
