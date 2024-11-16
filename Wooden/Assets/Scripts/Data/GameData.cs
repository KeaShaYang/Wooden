using UnityEngine;
//在资源面板右键Create，创建该类对应的Asset文件
//[CreateAssetMenu(fileName = "GameDataAsset", menuName = "Creat GameData Asset")]
[System.Serializable]
public class GameData:ScriptableObject
{
    [Header("玩家名")]
    public string testStr = "龙傲天";
    [Header("测试关卡")]
    public int wallLevel = 1;
    [Header("测试关卡")]
    public int gameLevel = 1;
    public int battleIdx = 0;

    public int[] battleHero;
    [Header("当前英雄数据")]
    public HeroData[] heroList;
    [Header("当前蔬菜数据")]
    public VegData[] vegDataList;
    [Header("当前蔬菜数据")]
    public int[] battleVegs;
    [Header("当前蔬菜数据")]
    public ItemData[] itemdataList;
    public long coin = 0;
}
[System.Serializable]
public class HeroData
{
    public string name = "";
    public int heroId = 0;
    public int Level = 1;
    public HeroData(int id, int v_Level, string v_Name)
    {
        name = v_Name;
        heroId = id;
        Level = v_Level;
    }
}
[System.Serializable]
public class VegData
{
    public string name = "";
    public int id = 0;
    public int Level = 1;
    public VegData(int id, int v_Level, string v_Name)
    {
        name = v_Name;
        this.id = id;
        Level = v_Level;
    }
}
[System.Serializable]
public class ItemData
{
    public string Name = "";
    public int id = 0;
    public int Num = 1;
    public ItemData(int id, int num, string name)
    {
        Name = name;
        this.id = id;
        Num = num;
    }
}
