using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(AudioSource))]
public class Movement : MonoBehaviour
{
    protected Rigidbody2D PlayerRb;
    protected Animator PlayerAnimator;

    protected Vector2 CellCheckSize;
    protected Vector2 GroundCheckSize;
    
    private bool _faceRight = true;
    private bool _doubleJump = true;
    private bool _isGround;

    public bool FaceRight => _faceRight;
    public GameObject BordersObject => bordersObject;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Borders")] 
    [SerializeField] private GameObject bordersObject;

    [Header("Colliders")]
    [SerializeField] private Collider2D headCollider;
    
    [Header("Checks")]
    [SerializeField] private GameObject cellCheck;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] protected float cellSizeX = 0.8f;
    [SerializeField] protected float groundSizeX = 1f;

    [Header("Movement")]
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float jumpForce = 13.0f;
    
    protected void Move(float move, bool jump)
    {
        if(!bordersObject.activeSelf) return;
        
        #region Move

        PlayerRb.velocity = new Vector2(move * speed, PlayerRb.velocity.y);

        #region Move animations

        PlayerAnimator.SetFloat("Speed", Mathf.Abs(move));

        #endregion

        #endregion

        #region Face

        if (!_faceRight && move > 0)
            Flip();
        else if (_faceRight && move < 0)
            Flip();

        #endregion

        #region Jump

        _isGround = Physics2D.OverlapBox(groundCheck.transform.position, GroundCheckSize, 0, groundLayer);

        // check enemy layer
        if(!_isGround)
            _isGround = Physics2D.OverlapBox(groundCheck.transform.position, GroundCheckSize, 0, 1 << 13);

        
        #region Jump animations

        PlayerAnimator.SetBool("Ground", _isGround);
        PlayerAnimator.SetFloat("VSpeed", PlayerRb.velocity.y);

        #endregion

        if (jump)
        {
            if (_isGround)
            {
                //playerRB.AddForce(new Vector2(0f, jumpForce));
                PlayerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                _doubleJump = true;
            } 
            else if (_doubleJump)
            {
                //playerRB.AddForce(new Vector2(0f, jumpForce));
                PlayerRb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                _doubleJump = false;
            }
        }

        #endregion

        #region Crouch
        
        _isGround = Physics2D.OverlapBox(cellCheck.transform.position, CellCheckSize, 0, groundLayer);

        if (!_isGround)
            headCollider.enabled = !Input.GetKey(KeyCode.S);

        #region Crouch animations

        PlayerAnimator.SetBool("Crouch", !headCollider.enabled);

        #endregion

        #endregion

        // if (_isGround && PlayerRb.velocity.x != 0 && !_audioSource.isPlaying)
        //     PlayAudio(runClip);
        // else if(!_isGround || PlayerRb.velocity.x == 0)
        //     StopAudio(runClip);
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0f, 180f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(cellCheck.transform.position, new Vector3(cellSizeX, 0.025f, cellCheck.transform.position.z));
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.transform.position, new Vector3(groundSizeX, 0.025f, groundCheck.transform.position.z));
    }
}
