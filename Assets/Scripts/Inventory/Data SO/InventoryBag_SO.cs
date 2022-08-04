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

// var array =  new InventoryItem[Settings.BagSize];
// Array.Fill(array,new InventoryItem());
// itemList  = array.ToList();