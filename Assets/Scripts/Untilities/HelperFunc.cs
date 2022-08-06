using Newtonsoft.Json;
using UnityEngine;

public static class HelperFunc
{
    #region 获取汉字
    public static string GetText(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.Seed => "种子",
            ItemType.BreakTool => "镐",
            ItemType.ChopTool => "斧子",
            ItemType.CollectTool => "篮子",
            ItemType.HoeTool => "锄头",
            ItemType.ReapTool => "镰刀",
            ItemType.WaterTool => "水壶",
            _ => "无"
        };
    }

    public static string GetText(Season season)
    {
        return season switch
        {
            Season.Spring => "春天",
            Season.Summer => "夏天",
            Season.Fall => "秋天",
            Season.Winter => "冬天",
            _ => "无"
        };
    }

    public static string GetClockText(int hour, int minute)
    {
        return hour.ToString("00") + ":" + minute.ToString("00");
    }
    
    public static string GetDateText(int year, int month, int day)
    {
        //var date = new DateTime(year, month, day);
        //return date.ToString("D");
        return year + "年" + month + "月" + day + "日";
    }
    #endregion
    
    #region 打印
    public static void PrintGreen(params object[] objs)
    {
        var s = "";
        foreach (object t in objs)
        {
            s += t.ToString();
        }
        Debug.Log("<color=#00ff00ff>\t" + s + "\t</color>");
    }

    public static void PrintPurple(params object[] objs)
    {
        var s = "";
        foreach (object t in objs)
        {
            s += t.ToString();
        }
        Debug.Log("<color=#ff00ffff>\t" + s + "\t</color>");
    }

    public static void PrintBlue(params object[] objs)
    {
        var s = "";
        foreach (object t in objs)
        {
            s += t.ToString();
        }
        Debug.Log("<color=#0000ffff>\t" + s + "\t</color>");
    }

    public static string ToJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
    
    #endregion

}

// public static class YieldHelper
// {
//     #region 避免每次yield return都new
//     public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
//
//     public static IEnumerator WaitForSeconds(float totalTime, bool ignoreTimeScale = false)
//     {
//         float time = 0;
//         while (time < totalTime)
//         {
//             time += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
//             yield return null;
//         }
//     }
//
//     public static IEnumerator WaitForFrame(int i)
//     {
//         int count = 0;
//         while (count < i)
//         {
//             yield return null;
//             count++;
//         }
//     }
//     #endregion
// }