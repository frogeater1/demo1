public enum ItemType
{
    Seed,                  
    Commodity,              //商品
    Furniture,              //家具

    HoeTool,                //锄地
    ChopTool,               //砍树
    BreakTool,              //砸石头
    ReapTool,               //割草
    WaterTool,              //浇水
    CollectTool,            //收集

    ReapableScenery,        //可以被割的杂草
}
public enum SlotType        //一个格子的类型
{
    Bag,
    Box,
    Shop,
}
public enum InventoryLocation   //整个包裹的类型
{
    Bag,
    Box,
}
public enum BodyState
{
    None,
    Hold,
    Hoe,
    Water,
    Collect,
    Break,
    //TOADD
}

public enum BodyPart
{
    Body,
    Hair,
    Arm,
    Tool,
}

public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter,
}

public enum TileType
{
    canDig,
    canDropItem,
    canPlaceFurniture,
    NpcObstacle,
}