using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RicUtils.Managers
{
    public static class SingletonCreation
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnLoad()
        {
            foreach (var manager in RicUtils_Settings.singletonManagers)
            {
                var type = manager.manager.Type;
                var method = type.GetMethod("CreateManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
                method.Invoke(null, new object[] { });
            }
        }
    }
}
