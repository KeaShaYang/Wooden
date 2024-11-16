using System.Collections;

public class LevelMgr : Singleton<LevelMgr>
{
    public LevelModel V_Model;
    public  LevelMgr()
    {
        V_Model = new LevelModel();
    }
}
