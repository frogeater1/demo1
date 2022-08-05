using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MFarm.Inventory;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MFarm.Map;
using Newtonsoft.Json;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, commodity;

    private Sprite _curSpite;

    private Image _cursorImage;

    private RectTransform _cursorRectTransform;

    private Camera _mainCamera;

    private bool _cursorEnable; //是否刷新鼠标

    private bool _cursorValid; //在当前位置是否可用

    private ItemDetails _curItemDetails;
    
    private Transform _playerTransform;

    private void OnEnable()
    {
        EventHandler.ItemSelect += OnItemSelect;
        EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
    }


    private void OnDisable()
    {
        EventHandler.ItemSelect -= OnItemSelect;
        EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
    }

    private void Start()
    {
        _cursorRectTransform = GameObject.FindGameObjectWithTag("CursorCanvas")!.GetComponent<RectTransform>();
        _cursorImage = _cursorRectTransform.Find("Cursor")!.GetComponent<Image>();
        _mainCamera = Camera.main;
        _playerTransform = FindObjectOfType<Player>().transform;

        SetCursorSprite(normal);
    }


    private void OnBeforeUnloadScene()
    {
        _cursorEnable = false;
    }


    private void Update()
    {
        if (!_cursorImage) return;
        _cursorImage.transform.position = Input.mousePosition;
        if (!InteractWithUI() && _cursorEnable)
        {
            SetCursorValid(CheckCursorValid());
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (_cursorValid && Input.GetMouseButtonDown(0))
        {
            EventHandler.CallItemUse();
        }
    }
    

    #region 设置鼠标样式

    private void SetCursorSprite(Sprite sprite)
    {
        _cursorImage.sprite = sprite;
        _cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorValid(bool bo)
    {
        _cursorValid = bo;
        _cursorImage.color = bo ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 0.4f);
    }

    #endregion

    private void OnItemSelect(ItemDetails details,int slotIndex)
    {
        Sprite sprite;
        if (InteractWithUI() && details != null)
        {
            _curItemDetails = details;
            sprite = details.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.ReapTool or ItemType.BreakTool or ItemType.ChopTool or ItemType.CollectTool or ItemType.HoeTool or ItemType.WaterTool => tool,
                ItemType.Commodity => commodity,
                _ => normal
            };
            _cursorEnable = true; //此处注意设置图片和enable的顺序
        }
        else
        {
            _curItemDetails = null;
            sprite = normal;
            _cursorEnable = false;
        }

        SetCursorSprite(sprite);
    }

    private bool CheckCursorValid()
    {
        var mouse_world_pos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));

        var current_grid = TileMapManager.Instance.currentGrid;
        Vector3Int mouse_grid_pos = current_grid.WorldToCell(mouse_world_pos);

        //判断使用范围
        Vector3Int player_grid_pos = current_grid.WorldToCell(_playerTransform.position);
        if (_curItemDetails.itemUseRadius < Mathf.Abs(mouse_grid_pos.x - player_grid_pos.x) || _curItemDetails.itemUseRadius < Mathf.Abs(mouse_grid_pos.y - player_grid_pos.y))
            return false;

        TileDetails tile_details = TileMapManager.Instance.GetTileDetails(mouse_grid_pos);
        if (tile_details == null)
            return false;

        return _curItemDetails.itemType switch
        {
            //TOADD
            ItemType.Commodity => _curItemDetails.canDropped && tile_details.canDropItem,
            ItemType.HoeTool => tile_details.canDig,
            ItemType.WaterTool => tile_details.daysSinceDug > -1 && tile_details.seedItemID == -1,
            _ => false
        };
    }

    private bool InteractWithUI()
    {
        return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    }
}