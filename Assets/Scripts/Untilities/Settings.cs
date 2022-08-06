public static class Settings
{
    //逐渐半透明的时间
    public const float ItemFadeDuration = 0.35f;

    //半透明程度
    public const float FadeTarget = 0.5f;

    //tip离鼠标指针的偏移距离
    public const int TipOffset = 50;


    //时间进制
    public const float TiksInSecond = 0.012f; //即真实时间过了多少算1s
    public const int SecondsInMinute = 60;
    public const int MinutesInHour = 60;
    public const int HoursInDay = 24;
    public const int DaysInMonth = 30;
    public const int MonthInYear = 12;
    public const int MonthInSeason = 3; //Season根据月份另外计算,不在正常进位中

    //场景加载
    public const float LoadingFadeDuration = 1.5f;
    
    //背包+bar大小
    public const int BagSize = 26;
}