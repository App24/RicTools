using RicTools.ScriptableObjects;
using RicTools.Settings;
using RicTools.Utilities;
using System.Reflection;
using UnityEngine;

namespace RicTools.Managers
{
    internal static class SingletonCreation
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnLoad()
        {
            CreateManager(typeof(TimerManager));
            foreach (var singletonManager in RicTools_RuntimeSettings.singletonManagers)
            {
                var type = singletonManager.manager.Type;
                if (type == null)
                {
                    Debug.LogWarning($"Could not find type: {singletonManager.manager.TypeNameAndAssembly}");
                    continue;
                }
                CreateManager(type, singletonManager.data);
            }
        }

        private static void CreateManager(System.Type type, DataManagerScriptableObject data = null)
        {
            var method = type.GetMethod("CreateManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
            var manager = method.Invoke(null, new object[] { });
            if (RicUtilities.IsSubclassOfRawGeneric(typeof(DataGenericManager<,>), type))
            {
                type.GetField("data", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).SetValue(manager, data);
            }
            type.GetMethod("OnCreation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy).Invoke(manager, new object[] { });
        }
    }
}
