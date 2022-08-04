using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using MFarm.Map;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("所有物品数据")]
        public ItemDataList_SO itemDataList;

        [Header("主角背包数据")]
        public InventoryBag_SO playerBag;

        private Camera _mainCamera;

        private ItemDetails _curHoldedItemDetails;
        private int _curHoldedSlotIndex;

        private void OnEnable()
        {
            EventHandler.ItemHold += OnItemHold;
            EventHandler.ItemUse += OnItemUse;
        }


        private void OnDisable()
        {
            EventHandler.ItemHold -= OnItemHold;
            EventHandler.ItemUse -= OnItemUse;
        }

        private void OnItemHold(ItemDetails itemDetails, int slotIndex)
        {
            _curHoldedItemDetails = itemDetails;
            _curHoldedSlotIndex = slotIndex;
        }

        private void OnItemUse()
        {
            //if (_curHoldedItemDetails == null) return;
            var mouse_world_pos = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));
            var grid_pos = TileMapManager.Instance.currentGrid.WorldToCell(mouse_world_pos);
            var tile_details = TileMapManager.Instance.GetTileDetails(grid_pos);
            if (tile_details != null)
            {
                //TOADD:增加其他物品具体使用功能
                switch (_curHoldedItemDetails.itemType)
                {
                    case ItemType.Commodity:
                        //TODO:await播动画
                        if (_curHoldedItemDetails.canDropped && playerBag.itemList[_curHoldedSlotIndex].itemAmount > 0)
                        {
                            ItemManager.Instance.DropItemInScene(_curHoldedItemDetails.itemID, mouse_world_pos);
                            RemoveItem(_curHoldedSlotIndex, 1);
                        }
                        break;
                    case ItemType.Seed:
                        //播种
                        break;
                    case ItemType.Furniture:
                        //放置家具
                        break;
                    case ItemType.ChopTool:
                        //砍树
                        break;
                    case ItemType.HoeTool:
                        //锄地
                        break;
                    case ItemType.WaterTool:
                        //浇水
                        break;
                    case ItemType.ReapTool:
                        //收割
                        break;
                    case ItemType.CollectTool:
                        //收获
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
            return itemDataList.itemDetailsList.Find(i => i.itemID == itemID);
        }

        /// <summary>
        /// 向背包中增加物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestroy">是否销毁原有物体</param>
        public void PickUpItem(Item item, bool toDestroy)
        {
            bool add_success = AddItemAtIndex(item.itemID, GetIndexInBag(item.itemID), 1);

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
                _curHoldedItemDetails = null;
                _curHoldedSlotIndex = -1;
                EventHandler.CallItemHold(null,-1);
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