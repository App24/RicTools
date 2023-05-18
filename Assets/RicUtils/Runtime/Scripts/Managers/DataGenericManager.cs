using RicUtils.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.Managers
{
    public abstract class DataGenericManager<T, D> : SingletonGenericManager<T> where T : DataGenericManager<T, D> where D : DataManagerScriptableObject
    {
        public D data;
    }
}
