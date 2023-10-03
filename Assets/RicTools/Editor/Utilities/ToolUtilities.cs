using RicTools.Editor.Settings;
using RicTools.Editor.Windows;
using RicTools.ScriptableObjects;
using RicTools.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.Utilities
{
    public static class ToolUtilities
    {
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
            return GetCustomEditorType(type) != null;
        }

        internal static System.Type GetCustomEditorType(System.Type type)
        {
            var editorTypes = GetCustomEditorTypes();
            foreach (var editorType in editorTypes)
            {
                var genericTypeArguments = editorType.GetTypeInfo().BaseType.GetTypeInfo().GenericTypeArguments;
                if (genericTypeArguments.Length == 0) continue;
                if (genericTypeArguments[0] == type) return editorType;
            }

            return null;
        }

        internal static bool HasAvailableScriptableObject(System.Type type)
        {
            return GetAvailableScriptableObjectType(type) != null;
        }

        internal static System.Type GetAvailableScriptableObjectType(System.Type type)
        {
            var editorTypes = GetAvailableScriptableObjectTypes();
            foreach (var editorType in editorTypes)
            {
                var genericTypeArguments = editorType.GetTypeInfo().BaseType.GetTypeInfo().GenericTypeArguments;
                if (genericTypeArguments.Length == 0) continue;
                if (genericTypeArguments[0] == type) return editorType;
            }

            return null;
        }

        internal static List<System.Type> GetCustomEditorTypes()
        {
            return TypeCache.GetTypesDerivedFrom(typeof(GenericEditorWindow<,>)).ToList(); ;
        }

        internal static List<System.Type> GetAvailableScriptableObjectTypes()
        {
            return TypeCache.GetTypesDerivedFrom(typeof(AvailableScriptableObject<>)).ToList(); ;
        }

        internal static List<System.Type> GetSOsTypes()
        {
            return TypeCache.GetTypesDerivedFrom(typeof(GenericScriptableObject)).ToList(); ;
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
            foreach (var keyValuePair in GetSOsTypes())
            {
                if (HasCustomEditor(keyValuePair))
                {
                    var editorType = GetCustomEditorType(keyValuePair);
                    var showWindow = editorType.GetMethod("ShowWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (showWindow == null)
                    {
                        Debug.LogError(editorType + " has no ShowWindow static function");
                        return false;
                    }
                    var temp = showWindow.Invoke(null, null);
                    var data = System.Convert.ChangeType(temp, editorType);
                    {
                        var editorContainer = editorType.GetFieldRecursive("scriptableObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(data);
                        editorContainer.GetType().GetProperty("Value").SetValue(editorContainer, so);
                    }
                    editorType.GetMethodRecursive("LoadScriptableObjectInternal", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(data, new object[] { so });
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

        public static Texture2D RenderStaticPreview(Sprite sprite, int width, int height)
        {
            if (sprite == null) return null;
            var spriteUtilityType = System.Type.GetType("UnityEditor.SpriteUtility,UnityEditor.CoreModule");
            if (spriteUtilityType == null) return null;
            var ret = spriteUtilityType
                        .GetMethod("RenderStaticPreview", new System.Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) })
                        .Invoke(null, new object[] { sprite, Color.white, width, height });
            return ret as Texture2D;
        }
    }
}
