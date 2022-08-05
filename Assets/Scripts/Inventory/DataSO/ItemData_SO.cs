using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData_SO", menuName = "Inventory/ItemData")]
public class ItemData_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList; 
}
