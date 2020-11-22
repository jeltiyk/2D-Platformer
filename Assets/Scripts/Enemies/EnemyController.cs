using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private Animator _enemyAnimator;
    private bool _deadState;
    private bool _faceRight = true;
    
    #region Properties

    protected Rigidbody2D EnemyRb { get; private set; }
    public bool Throws => throws;
    protected Collider2D HitsCollider => hitsCollider;

    #endregion
    
    [Header("Colliders")]
    [SerializeField] private Collider2D hitsCollider;
    
    [Header("Enemy health")]
    [SerializeField] private int health = 100;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private bool raycast;
    [SerializeField] private bool groundCheck;

    [Header("Raycast movement settings")]
    [SerializeField] private GameObject[] ignoreObjects;
    [SerializeField] private Transform raycastPosition;
    [SerializeField] private float rayLength = 1f;

    [Header("Ground Check movement settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private float checkRadius = 0.3f;

    [Header("Destroy")] 
    [SerializeField] private float time = 0.25f;
    
    [Header("Takes damage")]
    [SerializeField] private bool throws;
    [SerializeField] private bool hits;
    
    private void Awake()
    {
        EnemyRb = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_deadState) return;
        if (!raycast && !groundCheck) return;

        EnemyRb.velocity = new Vector2(transform.right.x * speed, EnemyRb.velocity.y);

        if (raycast)
        {
            RaycastHit2D info = Physics2D.Raycast(raycastPosition.position, raycastPosition.right, rayLength);
            
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

        if (groundCheck)
        {
            if (!isGround())
                Flip();
        }
    }

    private bool isGround()
    {
        return Physics2D.OverlapCircle(groundPosition.transform.position, checkRadius, groundLayer);
    }
    
    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0f, 180, 0);
    }

    public void OnTakeDamage(int damage)
    {
        health -= damage;

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
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundPosition.transform.position, checkRadius);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hits && other.GetComponent<PlayerController>() != null)
        {
            OnTakeDamage(health);
        }
    }
}
