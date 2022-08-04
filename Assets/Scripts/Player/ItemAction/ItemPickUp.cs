using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Item item)) return;
            if (item.itemDetails.canPickedUp)
            {
                InventoryManager.Instance.PickUpItem(item, true);
            }
        }
    }

}