using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MFarm.Map;
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

    private float _mouseX;
    private float _mouseY;
    private bool _usingTool;

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
        EventHandler.ToolUse += OnToolUse;
    }

    private void OnDisable()
    {
        EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
        EventHandler.AfterLoadScene -= OnAfterLoadScene;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.ToolUse -= OnToolUse;
    }

    private void OnToolUse(ItemDetails itemDetails, TileDetails tileDetails, Vector3 mouseWorldPos)
    {
        if (_usingTool) return;
        _mouseX = mouseWorldPos.x - transform.position.x;
        _mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);

        if (Mathf.Abs(_mouseX) > Mathf.Abs(_mouseY))
            _mouseY = 0;
        else
            _mouseX = 0;
        UniTask.Void(async () =>
        {
            var source = new UniTaskCompletionSource();
            UseToolAnimation(source).Forget();
            await source.Task;
            switch (itemDetails.itemType)
            {
                //TOADD: 其他工具使用
                case ItemType.HoeTool:
                    TileMapManager.Instance.Dig(tileDetails);
                    break;
                case ItemType.WaterTool:
                    TileMapManager.Instance.Water(tileDetails);
                    break;
                case ItemType.BreakTool:
                    break;
                case ItemType.ReapTool:
                    break;
                case ItemType.ChopTool:
                    break;
            }
        });
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

        var tmp_input = new Vector2(_inputX, _inputY);
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


    private async UniTask UseToolAnimation(UniTaskCompletionSource source)
    {
        _usingTool = true;
        _canInput = false;
        //强制设置movement,避免按住移动键时使用工具的滑行问题
        _movement = Vector2.zero;
        foreach (var anim in _animators)
        {
            anim.SetTrigger("useTool");
            //人物的面朝方向
            anim.SetFloat("inputX", _mouseX);
            anim.SetFloat("inputY", _mouseY);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.45f));
        source.TrySetResult();
        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
        //等待动画结束
        _usingTool = false;
        _canInput = true;
    }

    private void SwitchAnimation()
    {
        foreach (Animator anim in _animators)
        {
            anim.SetBool("isMoving", _isMoving);
            anim.SetFloat("mouseX", _mouseX);
            anim.SetFloat("mouseY", _mouseY);
            if (!_isMoving) continue;
            anim.SetFloat("inputX", _movement.x);
            anim.SetFloat("inputY", _movement.y);
        }
    }
}