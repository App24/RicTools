using RicUtils.Editor.Settings;
using RicUtils.ScriptableObjects;
using RicUtils.Utilities;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicUtils.Editor.Utilities
{
    public static class ToolUtilities
    {
        internal static List<ScriptableEditor> GetScriptableEditors()
        {
            return new List<ScriptableEditor>(Settings.RicUtils_EditorSettings.scriptableEditors);
        }

        public static List<T> FindAssetsByType<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        internal static bool HasCustomEditor(System.Type type)
        {
            return GetScriptableEditors().Exists(se => se.HasCustomEditor(type));
        }

        internal static System.Type GetCustomEditorType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(type)) return keyValuePair.EditorType;
            }
            return null;
        }

        internal static System.Type GetAvailableScriptableObjectType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasAvailableScriptableObject(type)) return keyValuePair.AvailableScriptableObjectType;
            }
            return null;
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            var temp = EditorUtility.InstanceIDToObject(instanceId) as GenericScriptableObject;
            if (temp != null)
            {
                return OpenScriptableObjectFile(temp);
            }
            return false;
        }

        private static bool OpenScriptableObjectFile(GenericScriptableObject so)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(so.GetType()))
                {
                    var showWindow = keyValuePair.EditorType.GetMethod("ShowWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (showWindow == null)
                    {
                        Debug.LogError(keyValuePair.EditorType + " has no ShowWindow static function");
                        return false;
                    }
                    var temp = showWindow.Invoke(null, null);
                    var data = System.Convert.ChangeType(temp, keyValuePair.EditorType);
                    {
                        var editorContainer = keyValuePair.EditorType.GetFieldRecursive("scriptableObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(data);
                        editorContainer.GetType().GetProperty("Value").SetValue(editorContainer, so);
                    }
                    keyValuePair.EditorType.GetMethodRecursive("LoadScriptableObjectInternal", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(data, new object[] { so });
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetActiveFolderPath(out string path)
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            path = (string)args[0];

            return found;
        }

        public static void AddStylesheet(this VisualElement root, params string[] styleSheets)
        {
            foreach (var sheet in styleSheets)
            {
                var stylesheet = (StyleSheet)EditorGUIUtility.Load(sheet);

                if (stylesheet == null)
                {
                    Debug.LogError($"Couldnt load stylesheet: '{sheet}'");
                    continue;
                }

                root.styleSheets.Add(stylesheet);
            }
        }
    }
}
