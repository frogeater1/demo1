using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHold : MonoBehaviour
{
    [SerializeField]
    [Header("当前持有物品的sprite")]
    private SpriteRenderer _holdItemSpriteRenderer;
    
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

    private void OnItemSelect(ItemDetails details,int slotIndex)
    {
        if (details is { canCarried: true })
        {
            _holdItemSpriteRenderer.sprite = details.itemOnWorldSprite ? details.itemOnWorldSprite : details.itemIcon;
            _holdItemSpriteRenderer.enabled = true;
        }
        else
        {
            _holdItemSpriteRenderer.enabled = false;
        }
    }
    private void OnBeforeUnloadScene()
    {
        _holdItemSpriteRenderer.enabled = false;
    }
}
