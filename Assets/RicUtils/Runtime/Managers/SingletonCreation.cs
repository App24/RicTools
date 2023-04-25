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
            foreach (var singletonManager in RuntimeSettings.singletonManagers)
            {
                var type = singletonManager.manager.Type;
                if (type == null)
                {
                    Debug.LogWarning($"Could not find type: {singletonManager.manager.TypeNameAndAssembly}");
                    continue;
                }
                var method = type.GetMethod("CreateManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
                var manager = method.Invoke(null, new object[] { });
                if (RicUtilities.IsSubclassOfRawGeneric(typeof(DataGenericManager<,>), type))
                {
                    type.GetField("data", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).SetValue(manager, singletonManager.data);
                }
                type.GetMethod("OnCreation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy).Invoke(manager, new object[] { });
            }
        }
    }
}
