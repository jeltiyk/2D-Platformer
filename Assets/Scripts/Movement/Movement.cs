using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    private Animator _playerAnimator;
    private bool _faceRight = true;
    private bool _doubleJump = true;
    private bool _isGround;
    
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Colliders")]
    [SerializeField] private Collider2D headCollider;
    
    [Header("Checks")]
    [SerializeField] private Transform cellCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;

    [Header("Movement")]
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float jumpForce = 13.0f;

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    public void Move(float move, bool jump)
    {
        #region Move

        _playerRb.velocity = new Vector2(move * speed, _playerRb.velocity.y);

        #region Move animations

        _playerAnimator.SetFloat("Speed", Mathf.Abs(move));

        #endregion

        #endregion

        #region Face

        if (!_faceRight && move > 0)
            Flip();
        else if (_faceRight && move < 0)
            Flip();

        #endregion

        #region Jump

        _isGround = Physics2D.OverlapCircle(groundCheck.transform.position, checkRadius, groundLayer);

        #region Jump animations

        _playerAnimator.SetBool("Ground", _isGround);
        _playerAnimator.SetFloat("VSpeed", _playerRb.velocity.y);

        #endregion

        if (jump)
        {
            if (_isGround)
            {
                //playerRB.AddForce(new Vector2(0f, jumpForce));
                _playerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                _doubleJump = true;
            }
            else if(_doubleJump)
            {
                //playerRB.AddForce(new Vector2(0f, jumpForce));
                _playerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                _doubleJump = false;
            }
        }

        #endregion

        #region Crouch

        _isGround = Physics2D.OverlapCircle(cellCheck.transform.position, checkRadius, groundLayer);

        if (!_isGround)
            headCollider.enabled = !Input.GetKey(KeyCode.S);

        #region Crouch animations

        _playerAnimator.SetBool("Crouch", !headCollider.enabled);

        #endregion

        #endregion
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0f, 180f, 0);

        //Vector3 Scale = transform.localScale;
        //Scale.x *= -1;
        //transform.localScale = Scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(cellCheck.transform.position, checkRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.transform.position, checkRadius);
    }
}
