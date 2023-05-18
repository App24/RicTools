using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Managers
{
    /// <summary>
    /// Managers that will be created through the singleton generator part of RicUtils
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonGenericManager<T> : GenericManager<T> where T : SingletonGenericManager<T>
    {
        internal static T CreateManager()
        {
            var gameObject = new GameObject();
            gameObject.name = $"{typeof(T)} Manager";
            var comp = gameObject.AddComponent<T>();
            if (comp.SetInstance())
            {
                DontDestroyOnLoad(gameObject);
            }
            return comp;
        }

        protected virtual void OnCreation()
        {

        }
    }
}
