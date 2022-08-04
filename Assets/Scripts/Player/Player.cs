using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;

    private Animator[] _animators;
    
    public float speed;
    private float _inputX;
    private float _inputY;
    private bool _isMoving;
    private bool _canInput; //是否允许操作

    private Vector2 _movement;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
        _canInput = true;
    }

    private void OnEnable()
    {
        EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
        EventHandler.AfterLoadScene += OnAfterLoadScene;
        EventHandler.MoveToPosition += OnMoveToPosition;
    }

    private void OnDisable()
    {
        EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
        EventHandler.AfterLoadScene -= OnAfterLoadScene;
        EventHandler.MoveToPosition -= OnMoveToPosition;
    }

    private void OnBeforeUnloadScene()
    {
        _canInput = false;
        _movement = Vector2.zero;
    }
    private void OnAfterLoadScene()
    {
        _canInput = true;
    }

    private void OnMoveToPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        if (_canInput)
        {
            PlayerInput();
        }
        _isMoving = _movement != Vector2.zero;
        SwitchAnimation();
    }
    
    private void FixedUpdate()
    {
        if (_isMoving)
            Movement();
    }

    private void PlayerInput()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");

        var tmp_input = new Vector2 (_inputX, _inputY);
        if (_inputX != 0 && _inputY != 0)
        {
            tmp_input.Normalize();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            tmp_input *= 0.5f;
        }

        _movement = tmp_input;
    }

    private void Movement()
    {
        _rb.MovePosition(_rb.position + speed * Time.deltaTime * _movement);
    }

    private void SwitchAnimation()
    {
        foreach (Animator anim in _animators )
        {
            anim.SetBool("isMoving", _isMoving);
            if (!_isMoving) continue;
            anim.SetFloat("InputX",_movement.x);
            anim.SetFloat("InputY",_movement.y);
        }
    }
    
}
