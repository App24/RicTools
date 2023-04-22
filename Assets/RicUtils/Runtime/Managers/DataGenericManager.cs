using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Managers
{
    public abstract class DataGenericManager<T> : GenericManager<T> where T : DataGenericManager<T>
    {
        protected virtual bool DontDestroyGameObjectOnLoad => true;
        protected override bool DestroyIfFound => DontDestroyGameObjectOnLoad;

        private void Awake()
        {
            SetInstance();
            if (DontDestroyGameObjectOnLoad)
                DontDestroyOnLoad(gameObject);
            OnCreation();
        }
    }
}
