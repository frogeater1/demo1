using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using MFarm.Map;
using UnityEngine.Serialization;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("所有物品数据")]
        public ItemData_SO itemData;

        [Header("主角背包数据")]
        public InventoryBag_SO playerBag;

        private Camera _mainCamera;

        public ItemDetails CurSelectedItemDetails { get; private set; }

        public int CurSelectedSlotIndex { get; private set; }

        private void OnEnable()
        {
            EventHandler.ItemSelect += OnItemSelect;
            EventHandler.ItemUse += OnItemUse;
        }


        private void OnDisable()
        {
            EventHandler.ItemSelect -= OnItemSelect;
            EventHandler.ItemUse -= OnItemUse;
        }

        private void OnItemSelect(ItemDetails itemDetails, int slotIndex)
        {
            CurSelectedItemDetails = itemDetails;
            CurSelectedSlotIndex = slotIndex;
        }

        private void OnItemUse()
        {
            //if (_curHoldedItemDetails == null) return;//理论上不会为null
            var mouse_world_pos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));
            var grid_pos = TileMapManager.Instance.currentGrid.WorldToCell(mouse_world_pos);
            var tile_details = TileMapManager.Instance.GetTileDetails(grid_pos);
            if (tile_details != null)
            {
                //TOADD: 2. 使用道具
                switch (CurSelectedItemDetails.itemType)
                {
                    case ItemType.Commodity:
                        ItemManager.Instance.DropItemInScene(CurSelectedItemDetails.itemID, mouse_world_pos);
                        RemoveItem(CurSelectedSlotIndex, 1);
                        break;
                    case ItemType.Seed:
                        //播种
                        EventHandler.CallSow(CurSelectedItemDetails.itemID,tile_details);
                        RemoveItem(CurSelectedSlotIndex, 1);
                        break;
                    case ItemType.Furniture:
                        //放置家具
                        break;
                    case ItemType.ChopTool or ItemType.HoeTool or ItemType.WaterTool or ItemType.ReapTool or ItemType.CollectTool:
                        EventHandler.CallToolUse(CurSelectedItemDetails, tile_details, mouse_world_pos);
                        break;
                }
            }
        }

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemList);
            _mainCamera = Camera.main;
        }


        /// <summary>
        /// 通过物品id返回物品详情
        /// </summary>
        /// <param name="itemID">ItemID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int itemID)
        {
            return itemData.itemDetailsList.Find(i => i.itemID == itemID);
        }

        /// <summary>
        /// 向背包中增加物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestroy">是否销毁原有物体</param>
        public void PickUpItem(Item item, bool toDestroy)
        {
            bool add_success = AddItemAtIndex(item.ItemID, GetIndexInBag(item.ItemID), 1);

            if (add_success && toDestroy)
            {
                Destroy(item.gameObject);
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemList);
        }

        public void SwapItem(SlotType slotType, int fromIndex, int toIndex)
        {
            switch (slotType)
            {
                case SlotType.Bag:
                    (playerBag.itemList[fromIndex], playerBag.itemList[toIndex]) = (playerBag.itemList[toIndex], playerBag.itemList[fromIndex]);
                    break;
                case SlotType.Box:
                    break;
                case SlotType.Shop:
                    break;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemList);
        }

        private void RemoveItem(int slotIndex, int removeNum)
        {
            playerBag.itemList[slotIndex].itemAmount -= removeNum;

            if (playerBag.itemList[slotIndex].itemAmount == 0)
            {
                playerBag.itemList[slotIndex].itemID = 0;
                CurSelectedItemDetails = null;
                CurSelectedSlotIndex = -1;
                EventHandler.CallItemSelect(null, -1);
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Bag, playerBag.itemList);
        }


        /// <summary>
        /// 根据itemID查物品在背包中的位置
        /// </summary>
        /// <param name="ID">若传入0即查询空位置</param>
        /// <returns>-1即没查到,查0返回-1即背包已满</returns>
        private int GetIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }

            return -1;
        }

        private bool AddItemAtIndex(int ID, int index, int amount)
        {
            //原来没这个物品
            if (index == -1)
            {
                int empty_index = GetIndexInBag(0);
                //没空位
                if (empty_index == -1) return false;
                //有空位
                playerBag.itemList[empty_index].itemID = ID;
                playerBag.itemList[empty_index].itemAmount = amount;
                return true;
            }

            //原来有这个物品
            playerBag.itemList[index].itemAmount += amount;
            return true;

            //下面这种是struct使用,因为struct是值类型,playerBag.itemList[index]是一个中间表达式(类似于右值的概念),不能修改
            //amount += playerBag.itemList[index].itemAmount;

            // InventoryItem item = playerBag.itemList[index];
            // item.itemID = ID;
            // item.itemAmount = amount;
            // playerBag.itemList[index] = item;
            // return true;
        }
    }
}