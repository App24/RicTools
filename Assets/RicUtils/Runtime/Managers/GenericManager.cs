using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Managers
{
    public abstract class GenericManager<T> : MonoBehaviour where T : GenericManager<T>
    {
        private static T _instance;

        public static T Instance => _instance;

        protected virtual bool DestroyIfFound => true;

        internal static void CreateManager()
        {
            var gameObject = new GameObject();
            gameObject.name = $"{typeof(T)} Manager";
            var comp = gameObject.AddComponent<T>();
            comp.SetInstance();
            DontDestroyOnLoad(gameObject);
            comp.OnCreation();
        }

        protected virtual void OnCreation()
        {

        }

        protected void SetInstance()
        {
            if (_instance != null && DestroyIfFound)
            {
                Destroy(this);
                return;
            }
            _instance = this as T;
        }
    }
}
