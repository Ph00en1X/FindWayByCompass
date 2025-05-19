using UnityEngine;

namespace Core
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void OnAwake() { }

        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }

            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }
    }
}
