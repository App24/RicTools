using RicTools.Editor.Utilities;
using RicTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicTools.Editor.Toolbar
{
    public static class CreateAvailableScriptableObjects
    {
        [MenuItem("RicTools/Create Available Scriptable Objects", priority = 1)]
        public static void CreateAvailableScripts()
        {
            foreach (var keyValuePair in ToolUtilities.GetSOsTypes())
            {
                var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(keyValuePair);

                if (availableScriptableObjectType == null) continue;

                var path = RicUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType);
                RicUtilities.CreateAssetFolder(path);

                var available = AssetDatabase.LoadAssetAtPath(path, availableScriptableObjectType);
                if (available == null)
                {
                    available = ScriptableObject.CreateInstance(availableScriptableObjectType);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(keyValuePair));
                    availableScriptableObjectType.GetMethodRecursive("SetItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(available, new object[] { items });

                    AssetDatabase.CreateAsset(available, path);

                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
