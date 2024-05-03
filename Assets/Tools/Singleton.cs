using UnityEngine;

namespace Tools
{
    public abstract class Singleton<T> :MonoBehaviour where T :  Singleton<T>
    {
        private static T s_instance;

        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Object.Destroy(this);
                return;
            }
            s_instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        public static T Instance 
        {
            get 
            { 
                if (s_instance == null) 
                {
                    var objs = FindObjectsOfType<T>();
                    if (objs.Length > 0)
                    {
                        s_instance = objs[0];
                    }
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                        s_instance = objs[0];
                    }
                    if (s_instance == null) 
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        s_instance = obj.AddComponent<T>();
                    }
                }
                return s_instance;
            }
        }
    }
}