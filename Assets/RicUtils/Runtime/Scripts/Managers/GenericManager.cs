using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Managers
{
    /// <summary>
    /// A generic manager that can be expanded on by other classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericManager<T> : MonoBehaviour where T : GenericManager<T>
    {
        private static T _instance;

        public static T Instance => _instance;

        protected virtual bool DestroyIfFound => true;

        protected virtual void Awake()
        {
            SetInstance();
        }

        protected bool SetInstance()
        {
            if (_instance != null && DestroyIfFound && _instance != this)
            {
                Destroy(this);
                return false;
            }
            _instance = this as T;
            return true;
        }
    }
}
