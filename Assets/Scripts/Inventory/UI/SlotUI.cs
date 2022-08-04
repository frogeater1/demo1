using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//using UnityEngine.InputSystem;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //在Awake中获取有延迟,直接赋值更快
        [Header("获取组件")]
        [SerializeField]
        private Image _slotImage;

        [SerializeField]
        private TextMeshProUGUI _amountText;

        [SerializeField]
        private GameObject _slotHighlight;

        [SerializeField]
        private Button _button;

        private InventoryUI InventoryUI => GetComponentInParent<InventoryUI>();

        [Header("格子数据")]
        public SlotType slotType;

        public int slotIndex;
        public bool isSelected;

        [Header("物品数据")]
        //此处不能直接用InventoryItem,交换时直接赋值SetEmpty里没有新对象只能重新new消耗大
        [SerializeField]
        private int _itemID;
        [SerializeField]
        private int _itemAmount;

        private ItemDetails _itemDetails;


        public void UpdateItem(InventoryItem item)
        {
            _itemID = item.itemID;
            _itemAmount = item.itemAmount;
            _itemDetails = InventoryManager.Instance.GetItemDetails(item.itemID);

            _slotImage.sprite = _itemDetails.itemIcon;

            _slotImage.enabled = true;
            _amountText.text = _itemAmount.ToString();
            _button.interactable = true;
        }


        public void SetEmpty()
        {
            _itemID = 0;
            _itemAmount = 0;
            _itemDetails = null;
            isSelected = false;

            _slotImage.enabled = false;
            _slotHighlight.SetActive(false);
            _amountText.text = string.Empty;
            _button.interactable = false;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_itemAmount == 0) return;

            if (slotType == SlotType.Bag)
            {
                EventHandler.CallItemSelect(isSelected ? null : _itemDetails, isSelected ? -1 : slotIndex);
            }
            
            InventoryUI.UpdateSlotHighlight(slotIndex);
        }
        
        public void SetSelected(bool bo)
        {
            isSelected = bo;
            _slotHighlight.SetActive(bo);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_itemAmount == 0) return;

            InventoryUI.dragImage.enabled = true;
            InventoryUI.dragImage.sprite = _itemDetails.itemIcon;
            InventoryUI.dragImage.SetNativeSize();
        }

        public void OnDrag(PointerEventData eventData)
        {
            InventoryUI.dragImage.transform.position = Input.mousePosition;
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            InventoryUI.dragImage.enabled = false;

            var targetGO = eventData.pointerCurrentRaycast.gameObject;
            // if (targetGO == null)
            // {    //不能拖拽丢弃了,只能点击丢弃
            //     if (!_itemDetails.canDropped) return;
            //     var pos = Camera.main!.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //     EventHandler.CallInstantiateItemInScene(_itemID, pos);
            //     
            //     InventoryManager.Instance.RemoveItem(slotIndex, 1);
            // }
            // else 
            if (targetGO && targetGO.TryGetComponent<SlotUI>(out var targetSlotUI))
            {
                switch (slotType, targetSlotUI.slotType)
                {
                    case (SlotType.Bag, SlotType.Bag) or (SlotType.Box, SlotType.Box):
                        InventoryManager.Instance.SwapItem(slotType, slotIndex, targetSlotUI.slotIndex);
                        break;
                    case (SlotType.Bag, SlotType.Box) or (SlotType.Box, SlotType.Box):
                        break;
                    case (SlotType.Bag, SlotType.Shop):
                        break;
                    case (SlotType.Shop, SlotType.Bag):
                        break;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_itemID != 0)
            {
                InventoryUI.itemTip.gameObject.SetActive(true);
                InventoryUI.itemTip.SetTip(slotType, _itemDetails);

                InventoryUI.itemTip.transform.position = transform.position + Vector3.up * Settings.TipOffset;
                
                //强制重新绘制,但目前用不到
                //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            }
            else
            {
                InventoryUI.itemTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryUI.itemTip.gameObject.SetActive(false);
            
            
        }
    }

}