using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class CreateAvailableScriptableObjects
    {
        [MenuItem("RicUtils/Create Available Scriptable Objects", priority = 1)]
        public static void CreateAvailableScripts()
        {
            foreach (var keyValuePair in ToolUtilities.GetScriptableEditors())
            {
                var path = RicUtilities.GetAvailableScriptableObjectPath(keyValuePair.Value.Item1);
                RicUtilities.CreateAssetFolder(path);

                var available = AssetDatabase.LoadAssetAtPath(path, keyValuePair.Value.Item1);
                if (available == null)
                {
                    available = ScriptableObject.CreateInstance(keyValuePair.Value.Item1);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(keyValuePair.Key));
                    keyValuePair.Value.Item1.GetMethod("SetItems").Invoke(available, new object[] { items });

                    AssetDatabase.CreateAsset(available, path);

                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}
