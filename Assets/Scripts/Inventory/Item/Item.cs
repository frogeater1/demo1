using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace MFarm.Inventory
{

    public class Item : MonoBehaviour
    {
        public int ItemID
        {
            get => _itemID;
            set
            {
                if (value != 0)
                {
                    _itemID = value;
                    itemDetails = InventoryManager.Instance.GetItemDetails(_itemID);
                    if (itemDetails == null) return;
                    var sprite = itemDetails.itemOnWorldSprite ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;
                    _spriteRenderer.sprite = sprite;

                    Vector2 newSize = new(sprite.bounds.size.x, sprite.bounds.size.y);
                    coll.size = newSize;
                    coll.offset = new Vector2(0, sprite.bounds.center.y);
                }
            }
        }

        private int _itemID;
        public ItemDetails itemDetails;

        private SpriteRenderer _spriteRenderer;
        public BoxCollider2D coll;


        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }
    }
}
