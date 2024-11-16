using UnityEngine;

namespace MolSpace.PhotoGraphing
{
    /// <summary>
    /// 脚本单例基类，调用instance时自动调用Init()方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        //public static T Instance { get; private set; }

        private static T instance;

        public static T Instance
        {
            //按需加载
            get
            {
                //如果没有访问过
                if (instance == null)
                {
                    //在场景中存在T类型对象
                    instance = FindObjectOfType<T>();
                    //如果没有找到
                    if (instance == null)
                    {
                        //创建
                        instance = new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
                    }
                    //instance.Init();
                }
                instance.Init();
                return instance;
            }
        }

        protected void Awake()
        {
            //instance = this as T;
            //当场景加载时，不销毁当前游戏对象
            //DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 为脚本提供初始化功能
        /// </summary>
        public virtual void Init() { }
    }
}
