using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{

    public class Item : MonoBehaviour
    {
        public int itemID;
        public ItemDetails itemDetails;

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _coll;


        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID != 0)
            {
                Init(itemID);
            }
        }

        private void Init(int id)
        {
            itemID = id;

            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails == null) return;
            var sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;
            _spriteRenderer.sprite = sprite;

            Vector2 newSize = new(sprite.bounds.size.x, sprite.bounds.size.y);
            _coll.size = newSize;
            _coll.offset = new Vector2(0, sprite.bounds.center.y);

        }
    }
}
