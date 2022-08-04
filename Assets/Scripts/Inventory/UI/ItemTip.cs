using UnityEngine;
using TMPro;

namespace MFarm.Inventory
{
    public class ItemTip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private GameObject bottom;


        public void SetTip(SlotType slotType,ItemDetails details)
        {
            nameText.text = details.itemName;
            typeText.text = HelperFunc.GetText(details.itemType);
            descriptionText.text = details.itemDescription;
            if (details.itemType is ItemType.Commodity or ItemType.Seed or ItemType.Furniture)
            {
                bottom.SetActive(true);

                valueText.text = slotType switch
                {
                    SlotType.Shop => details.itemPrice.ToString(),
                    SlotType.Bag => (Mathf.FloorToInt(details.itemPrice * details.sellPercentage)).ToString(),
                    SlotType.Box => (Mathf.FloorToInt(details.itemPrice * details.sellPercentage)).ToString(),
                    _ => "0"
                };
            }
            else
            {
                bottom.SetActive(false);
            }
        }
    }
}