using System;
using System.Linq;
using MFarm.Plant;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedUp;
    public bool canDropped;
    public bool canHolded;
    public int itemPrice;

    [Range(0, 1)]
    public float sellPercentage;
}

[Serializable]
public class InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[Serializable]
public class AnimatorType
{
    public BodyState BodyState;
    public BodyPart bodyPart;
    public AnimatorOverrideController overrideController;
}

//普通的Vector3不能序列化
[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }
}

[Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[Serializable]
public class TileProperty
{
    public Vector2Int tilePos;
    public TileType tileType;
}

[Serializable]
public class TileDetails
{
    public Vector2Int pos;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNpcObstacle;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvested = -1;
    public int beUsedToolCount;
    public int beHarvestedCount;

    public void ResetCropInfo()
    {
        seedItemID = -1;
        growthDays = -1;
        daysSinceLastHarvested = -1;
        beUsedToolCount = 0;
        beHarvestedCount = 0;
    }
}

[Serializable]
public class CropDetails
{
    public int seedItemID;

    [Header("不同阶段需要的天数")]
    public int[] growthDays;

    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }

            return amount;
        }
    }

    [Header("不同生长阶段物品Prefab")]
    public Crop[] growthPrefabs;

    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;

    [Header("可种植的季节")]
    public Season[] seasons;

    [FormerlySerializedAs("harvestToolItemID")]
    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemIDs;

    [FormerlySerializedAs("ToolEffect")]
    [Header("每种工具产生的使用次数(数值越大工具越好)")]
    public int[] ToolEffects;

    [Header("总共需要被使用工具的次数")]
    public int needUseToolCount;

    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemIDs;

    public int[] producedMinAmounts;
    public int[] producedMaxAmounts;
    public int spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;

    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;

    public bool hasAnimation;
    public bool hasParticalEffect;

    // public ParticleEffectType effectType;
    // public Vector3 effectPos;
    // public SoundName soundEffect;

    public bool ToolMatched(int toolID)
    {
        return harvestToolItemIDs.Contains(toolID);
    }

    public int GetToolEffect(int toolID)
    {
        int i = Array.IndexOf(harvestToolItemIDs, toolID);
        return i > -1 ? ToolEffects[i] : 0;
    }
}