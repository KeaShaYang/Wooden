
public class Singleton<T> where T : class, new()
{
    static T _instance;
    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
    public virtual void Destry()
    {

    }
}
