using UnityEngine;
//在资源面板右键Create，创建该类对应的Asset文件
//[CreateAssetMenu(fileName = "GameDataAsset", menuName = "Creat GameData Asset")]
[System.Serializable]
public class GameData:ScriptableObject
{
    [Header("玩家名")]
    public string testStr = "龙傲天";
    [Header("测试关卡")]
    public int Level = 1;
    [Header("测试关卡")]
    public int gameLevel = 1;
    public long coin = 0;
}

