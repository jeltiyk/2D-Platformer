using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ActionsController), typeof(Throw))]
public class PCController : Movement
{
    private ActionsController _actionsController;
    
    private float _move;
    private bool _jump;
    private bool _throw;
    private bool _throwInRange;

    private bool _throwKeyPressed;
    private DateTime _keyHoldTime;
    private float _keyTime;

    private void Awake()
    {
        // fields in movement class
        PlayerRb = GetComponent<Rigidbody2D>();
        PlayerAnimator = GetComponent<Animator>();
        
        CellCheckSize = new Vector2(cellSizeX, 0.025f);
        GroundCheckSize = new Vector2(groundSizeX, 0.025f);
        // ===
        
        _actionsController = GetComponent<ActionsController>();
    }
    
    private void Update()
    {
        #region Move

        _move = Input.GetAxis("Horizontal");

        #endregion

        #region Jump

        if (Input.GetKeyDown(KeyCode.Space))
            _jump = true;

        #endregion

        #region Throw

        if (Input.GetKeyDown(KeyCode.E))
        {
            _keyHoldTime = DateTime.Now;
            _throwKeyPressed = true;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (_throwKeyPressed)
            {
                if ((DateTime.Now - _keyHoldTime).TotalSeconds > _actionsController.Throw.MAXTime)
                {
                    _keyTime = (float)(DateTime.Now - _keyHoldTime).TotalSeconds;

                    _throw = true;
                    _throwKeyPressed = false;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E) && _throwKeyPressed)
        {
            if ((DateTime.Now - _keyHoldTime).TotalSeconds > _actionsController.Throw.MINTime)
            {
                _keyTime = (float)(DateTime.Now - _keyHoldTime).TotalSeconds;
            }

            _throw = true;
            _throwKeyPressed = false;
        }

        #endregion

        #region Throw in range

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_actionsController.Ignore)
            {
                _throwInRange = true;
                return;
            }
            
            _keyHoldTime = DateTime.Now;
            _throwKeyPressed = true;
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (_throwKeyPressed)
            {
                if ((DateTime.Now - _keyHoldTime).TotalSeconds >= _actionsController.Throw.MAXTime)
                {
                    _keyTime = (float)(DateTime.Now - _keyHoldTime).TotalSeconds;

                    _throwInRange = true;
                    _throwKeyPressed = false;
                }
            }
        }

        #endregion
    }
    
    private void FixedUpdate()
    {
        Move(_move, _jump);
        _actionsController.Actions(_throw, _throwInRange, _keyTime);

        _keyTime = 0;
        _jump = _throw = _throwInRange = false;
    }
}
