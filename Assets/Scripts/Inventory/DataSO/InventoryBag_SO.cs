using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryDataList")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;
}
//存数据系统未开发之前暂时在此存背包数据