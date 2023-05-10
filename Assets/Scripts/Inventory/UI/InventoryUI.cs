using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("用于显示tip的对象")] 
        public ItemTip itemTip;
        
        [Header("用于显示拖拽效果的Image")]
        public Image dragImage;

        [SerializeField]
        [Header("玩家背包UI")]
        private GameObject _bagUI;
        private bool _isOpened;


        [SerializeField]
        private List<SlotUI> _playerSlots;

        private void Start()
        {
            for(int i = 0; i < _playerSlots.Count; i++)
            {
                _playerSlots[i].slotIndex = i;
            }

            _isOpened = _bagUI.activeInHierarchy;
        }


        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
        }


        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
        }
        

        private void OnBeforeUnloadScene()
        {
            UpdateSlotHighlight(-1);
        }

        /// <summary>
        /// 刷新格子里的所有物品
        /// </summary>
        /// <param name="location">包类型</param>
        /// <param name="list">物品列表</param>
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Bag:
                    for (int i = 0; i < _playerSlots.Count; i++)
                    {
                        if (i < list.Count && list[i].itemAmount > 0)
                        {
                            _playerSlots[i].UpdateItem(list[i]);
                        }
                        else
                        {
                            _playerSlots[i].SetEmpty();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    break;
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.B))
            {
                OnOpenBagUI();
            }
        }


        public void OnOpenBagUI()
        {
            _isOpened = !_isOpened;

            _bagUI.SetActive(_isOpened);
        }
        
        public void UpdateSlotHighlight(int index)
        {
            foreach (SlotUI slot in _playerSlots)
            {
                if (index != slot.slotIndex)
                {
                    slot.SetSelected(false);
                }
                else
                {
                    slot.SetSelected(!slot.isSelected);
                }
            }
        }
    }
}
