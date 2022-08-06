using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    #region 物品相关

    //更新包的UI
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    //选中背包中的物品
    public static event Action<ItemDetails, int> ItemSelect;

    public static void CallItemSelect(ItemDetails details, int slotIndex)
    {
        ItemSelect?.Invoke(details, slotIndex);
    }

    //使用物品
    public static event Action ItemUse;

    public static void CallItemUse()
    {
        ItemUse?.Invoke();
    }

    public static event Action<ItemDetails, TileDetails, Vector3> ToolUse;

    public static void CallToolUse(ItemDetails itemDetails, TileDetails tileDetails, Vector3 mouseWorldPos)
    {
        ToolUse?.Invoke(itemDetails, tileDetails, mouseWorldPos);
    }

    public static event Action<int, TileDetails> Sow;

    public static void CallSow(int itemID, TileDetails tileDetails)
    {
        Sow?.Invoke(itemID, tileDetails);
    }

    #endregion

    #region 时间相关

    public static event Action<int> GameMinuteUpdate;

    public static void CallGameMinuteUpdate(int minute)
    {
        GameMinuteUpdate?.Invoke(minute);
    }

    public static event Action<int> GameHourUpdate;

    public static void CallGameHourUpdate(int hour)
    {
        GameHourUpdate?.Invoke(hour);
    }

    public static event Action<int, int, int, Season> GameDateUpdate;

    public static void CallGameDateUpdate(int year, int month, int day, Season season)
    {
        GameDateUpdate?.Invoke(year, month, day, season);
    }

    #endregion

    #region 切换场景

    public static event Action<string, Vector3> Transition;

    public static void CallTransition(string targetScene, Vector3 targetPos)
    {
        Transition?.Invoke(targetScene, targetPos);
    }

    public static event Action BeforeUnloadScene;

    public static void CallBeforeUnloadScene()
    {
        BeforeUnloadScene?.Invoke();
    }

    public static event Action AfterLoadScene;

    public static void CallAfterLoadScene()
    {
        AfterLoadScene?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;

    public static void CallMoveToPostition(Vector3 pos)
    {
        MoveToPosition?.Invoke(pos);
    }

    #endregion
    
}