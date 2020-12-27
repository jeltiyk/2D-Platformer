using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class EnemyController : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Movement,
    }

    private Animator _enemyAnimator;
    private PlayerController _playerController;
    
    private State _currentState;
    private float _lastChangeStateTime;
    private float _stateChangeDelay;
    
    private bool _deadState;
    private bool _faceRight = true;

    protected bool Facing => _faceRight;
    
    private DateTime _lastEncounter;

    #region Fields
    
    [SerializeField] private int health;
    [SerializeField] protected int damage;
    [SerializeField] protected float damageDelay;

    [Header("Field of vision")]
    [SerializeField] protected Transform visionPoint;
    [SerializeField] protected float rangeOfX;
    [SerializeField] protected float rangeOfY;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected LayerMask playerLayer;
    protected bool FindPlayer;
    protected bool IsAngry;

    [SerializeField] protected LayerMask enemyLayer;

    [SerializeField] protected GameObject bordersObject;

    [Header("Colliders")]
    [SerializeField] private Collider2D hitsCollider;

    [Header("Raycast settings")]
    [SerializeField] protected float rayLength = 1f;
    [SerializeField] protected GameObject raycastPosition;
    [SerializeField] protected GameObject[] ignoreObjects;
    
    [Header("GroundCheck settings")]
    [SerializeField] protected float groundCheckSizeX = 0.3f;
    [SerializeField] protected GameObject groundPosition;
    [SerializeField] protected LayerMask groundLayer;
    protected Vector2 _groundCheckSize;

    
    [Header("Jump settings")]
    [SerializeField] protected float jumpForceMinValue;
    [SerializeField] protected float jumpForceMaxValue;
    [SerializeField] protected float jumpDelay = 2f;

    [Header("States")]
    [SerializeField] private float minChangeTime;
    [SerializeField] private float maxChangeTime;
    [SerializeField] private State[] states;
    
    [Header("Takes damage")]
    [SerializeField] private bool throws;
    [SerializeField] private bool hits;

    [Header("Destroy")] 
    [SerializeField] private float time = 0.25f;
    
    [Header("Movement")]
    [SerializeField] protected float speed = 5f;
    
    #endregion

    #region Properties
    protected Rigidbody2D EnemyRb { get; private set; }
    public bool Throws => throws;
    private Collider2D HitsCollider => hitsCollider;

    private Collider2D player;
    private Collider2D enemy;
    
    #endregion

    private void Awake()
    {
        EnemyRb = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
        _groundCheckSize = new Vector2(groundCheckSizeX, 0.025f);
        player = FindObjectOfType<PlayerController>().GetComponent<Collider2D>();
        enemy = EnemyRb.GetComponent<Collider2D>();
    }
    
    private void FixedUpdate()
    {
        if (!bordersObject.activeSelf)
        {
            EnemyRb.simulated = false;
            return;
        }

        EnemyRb.simulated = true;

        if (Time.time - _lastChangeStateTime > _stateChangeDelay)
        {
            GetRandomState();
        }
        
        FindPlayer = Physics2D.OverlapBox(visionPoint.position, new Vector2(rangeOfX, rangeOfY), 0, playerLayer);

        if(FindPlayer && _currentState != State.Movement)
            ChangeState(State.Movement);
        
        if(_currentState == State.Movement)
            Move();
        
        _enemyAnimator.SetBool("Ground", isGround());
        _enemyAnimator.SetFloat("VSpeed", EnemyRb.velocity.y);
        _enemyAnimator.SetFloat("Speed", Mathf.Abs(EnemyRb.velocity.x));
    }
    
    protected virtual void Move()
    {
        if(_deadState) return;
    }

    protected virtual void RaycastCheck()
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
    
    protected virtual void GroundCheck()
    {
        if (!isGround())
            Flip();
    }
    
    protected virtual void JumpMove()
    {
        RaycastCheck();

        if (!isGround()) return;

        EnemyRb.velocity = new Vector2(transform.right.x * speed, transform.up.y * Random.Range(jumpForceMinValue, jumpForceMaxValue));
    }

    protected void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0f, 180, 0);
    }

    protected bool isGround()
    {
        return Physics2D.OverlapBox(groundPosition.transform.position, _groundCheckSize, 0, groundLayer);
    }

    protected bool isEnemy()
    {
        return Physics2D.OverlapBox(groundPosition.transform.position, _groundCheckSize, 0, enemyLayer);
    }
    
    private void GetRandomState()
    {
        int state = Random.Range(0, states.Length);

        if(_currentState == State.Idle && states[state] == State.Idle)
            GetRandomState();

        ChangeState(states[state]);
    }

    protected void ChangeState(State state)
    {
        // if(_currentState != State.Idle)
        //     _enemyAnimator.SetBool(_currentState.ToString(), false);
        //
        // if (state != State.Idle)
        // {
        //     _enemyAnimator.SetBool(state.ToString(), true);
        // }

        _currentState = state;
        _lastChangeStateTime = Time.time;
        _stateChangeDelay = Random.Range(minChangeTime, maxChangeTime);
    }
    
    public void OnTakeDamage(int damageCount)
    {
        health -= damageCount;

        if (health < 1)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        _deadState = true;
        _enemyAnimator.SetBool("Death", true);
        
        Destroy(gameObject, time);
    }

    #region Check trigger
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hits && other.GetComponent<PlayerController>() != null)
        {
            OnTakeDamage(health);
        }
    }

    #endregion
    

    #region Check collision

    private void OnCollisionEnter2D(Collision2D other)
    {
        _playerController = other.collider.GetComponent<PlayerController>();
        if(_playerController == null || !((DateTime.Now - _lastEncounter).TotalSeconds > damageDelay)) return;
        
        _lastEncounter = DateTime.Now;
        if(!hitsCollider.IsTouching(other.collider))
            _playerController.OnChangeHealth(-damage);
        // Debug.Log(_playerController._currentHealth);
        
        // if(other.collider.GetComponent<PlayerController>() == null) return;
        // other.collider.GetComponent<PlayerController>().OnChangeHealth(-damage);
        // Debug.Log(other.collider.GetComponent<PlayerController>()._currentHealth);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        _playerController = other.collider.GetComponent<PlayerController>();
        if(_playerController == null) return;
        
        HitsCollider.enabled = false; // Enemy collider

        if(!((DateTime.Now - _lastEncounter).TotalSeconds > damageDelay)) return;
        
        _lastEncounter = DateTime.Now;
        if(!hitsCollider.IsTouching(other.collider))
            _playerController.OnChangeHealth(-damage);
        // Debug.Log(_playerController._currentHealth);

        // if (other.collider.GetComponent<PlayerController>() == null) return;
        // other.collider.GetComponent<PlayerController>().OnChangeHealth(-damage);
        // HitsCollider.enabled = false;
        // Debug.Log(other.collider.GetComponent<PlayerController>()._currentHealth);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _playerController = other.collider.GetComponent<PlayerController>();
        if(_playerController == null) return;
        
        HitsCollider.enabled = true; // Enemy collider
        
        // if(other.collider.GetComponent<PlayerController>() == null) return;
        // HitsCollider.enabled = true;
    }

    #endregion
    
    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(groundPosition.transform.position, checkRadius);
        Gizmos.DrawWireCube(groundPosition.transform.position, new Vector3(groundCheckSizeX, 0.025f, transform.position.z));
        // Gizmos.DrawWireSphere(visionPoint.transform.position, fieldOfVision);
        Gizmos.DrawWireCube(visionPoint.transform.position, new Vector2(rangeOfX, rangeOfY));
    }
}
