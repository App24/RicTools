using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class ToolUtilities
    {
        public static List<ScriptableEditor> GetScriptableEditors()
        {
            return new List<ScriptableEditor>(RicUtils_EditorSettings.scriptableEditors);
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

        public static bool HasCustomEditor(System.Type type)
        {
            return GetScriptableEditors().Exists(se => se.HasCustomEditor(type));
        }

        public static System.Type GetCustomEditorType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(type)) return keyValuePair.EditorType;
            }
            return null;
        }

        public static System.Type GetAvailableScriptableObjectType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasAvailableScriptableObject(type)) return keyValuePair.AvailableScriptableObjectType;
            }
            return null;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var temp = EditorUtility.InstanceIDToObject(instanceId) as CustomScriptableObject;
            if (temp != null)
            {
                return OpenScriptableObjectFile(temp);
            }
            return false;
        }

        private static bool OpenScriptableObjectFile(CustomScriptableObject so)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(so.GetType()))
                {
                    //var actualSo = System.Convert.ChangeType(so, keyValuePair.Key);
                    var showWindow = keyValuePair.EditorType.GetMethod("ShowWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if(showWindow == null)
                    {
                        Debug.LogError(keyValuePair.EditorType + " has no ShowWindow static function");
                        return false;
                    }
                    var temp = showWindow.Invoke(null, null);
                    var data = System.Convert.ChangeType(temp, keyValuePair.EditorType);
                    keyValuePair.EditorType.GetField("scriptableObject").SetValue(data, so);
                    keyValuePair.EditorType.GetMethod("LoadScriptableObject").Invoke(data, new object[] { so, so == null });
                    return true;
                }
            }
            return false;
        }
    }

    internal class CustomScriptableObjectProcessing : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            var temp = AssetDatabase.LoadMainAssetAtPath(AssetPath);
            if (temp == null) return AssetDeleteResult.DidNotDelete;
            if (ToolUtilities.HasCustomEditor(temp.GetType()))
            {
                var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(temp.GetType());
                var availableScriptableObject = AssetDatabase.LoadMainAssetAtPath(RicUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));
                var baseType = temp.GetType();
                while(baseType.BaseType != null && baseType.BaseType != typeof(CustomScriptableObject))
                {
                    baseType = baseType.BaseType;
                }
                if (availableScriptableObject != null)
                {
                    var itemsField = availableScriptableObjectType.GetField("items");
                    var itemsArray = (object[])itemsField.GetValue(availableScriptableObject);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(baseType));
                    foreach (var item in itemsArray)
                    {
                        items.Add(item);
                    }
                    items.Remove(temp);
                    availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                    EditorUtility.SetDirty(availableScriptableObject);

                    //AssetDatabase.SaveAssets();
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }

        /*public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                var temp = AssetDatabase.LoadMainAssetAtPath(path);
                if (temp == null) continue;
                if (ToolUtilities.HasCustomEditor(temp.GetType()))
                {
                    var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(temp.GetType());
                    var availableScriptableObject = AssetDatabase.LoadMainAssetAtPath(ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));
                    if (availableScriptableObject == null)
                    {
                        availableScriptableObject = ScriptableObject.CreateInstance(availableScriptableObjectType);
                        var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                        availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                        AssetDatabase.CreateAsset(availableScriptableObject, ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));

                        AssetDatabase.SaveAssets();
                    }
                    if (availableScriptableObject != null)
                    {
                        var itemsField = availableScriptableObjectType.GetField("items");
                        var itemsArray = (object[])itemsField.GetValue(availableScriptableObject);
                        var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                        foreach (var item in itemsArray)
                        {
                            items.Add(item);
                        }
                        items.Add(temp);
                        availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                        EditorUtility.SetDirty(availableScriptableObject);
                    }
                }
            }
            return paths;
        }*/

        /*public static string[] OnWillSaveAssets(string[] paths)
        {
            Debug.Log("OnWillSaveAssets");
            foreach (string path in paths)
            {
                Debug.Log(path);
                if (!path.EndsWith(".meta")) continue;
                var assetName = path.Replace(".meta", "");
                var temp = AssetDatabase.LoadMainAssetAtPath(assetName);
                Debug.Log(temp);
                continue;
                if (temp == null) continue;
                if (ToolUtilities.HasCustomEditor(temp.GetType()))
                {
                    var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(temp.GetType());
                    var availableScriptableObject = AssetDatabase.LoadMainAssetAtPath(ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));
                    if (availableScriptableObject == null)
                    {
                        availableScriptableObject = ScriptableObject.CreateInstance(availableScriptableObjectType);
                        var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                        availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                        AssetDatabase.CreateAsset(availableScriptableObject, ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));

                        AssetDatabase.SaveAssets();
                    }
                    if (availableScriptableObject != null)
                    {
                        var itemsField = availableScriptableObjectType.GetField("items");
                        var itemsArray = (object[])itemsField.GetValue(availableScriptableObject);
                        var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                        foreach (var item in itemsArray)
                        {
                            items.Add(item);
                        }
                        items.Add(temp);
                        availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                        EditorUtility.SetDirty(availableScriptableObject);

                        //AssetDatabase.SaveAssets();
                    }
                }
            }
            return paths;
        }*/

        /*public static void OnWillCreateAsset(string assetName)
        {
            if (!assetName.EndsWith(".meta")) return;
            assetName = assetName.Replace(".meta", "");
            AssetDatabase.Refresh();
            var temp = AssetDatabase.LoadMainAssetAtPath(assetName);
            Debug.Log(temp + "__");
            if (temp == null) return;
            EditorUtility.SetDirty(temp);
            AssetDatabase.SaveAssets();
            //AssetDatabase.ImportAsset(assetName);
        }*/

        /*public static void OnWillCreateAsset(string assetName)
        {
            if (!assetName.EndsWith(".meta")) return;
            assetName = assetName.Replace(".meta", "");
            AssetDatabase.Refresh();
            var temp = AssetDatabase.LoadMainAssetAtPath(assetName);
            Debug.Log(temp);
            if(temp == null) return;
            if (ToolUtilities.HasCustomEditor(temp.GetType()))
            {
                var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(temp.GetType());
                var availableScriptableObject = AssetDatabase.LoadMainAssetAtPath(ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));
                if(availableScriptableObject == null)
                {
                    availableScriptableObject = ScriptableObject.CreateInstance(availableScriptableObjectType);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                    availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                    AssetDatabase.CreateAsset(availableScriptableObject, ToolUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));

                    AssetDatabase.SaveAssets();
                }
                if (availableScriptableObject != null)
                {
                    var itemsField = availableScriptableObjectType.GetField("items");
                    var itemsArray = (object[])itemsField.GetValue(availableScriptableObject);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(temp.GetType()));
                    foreach (var item in itemsArray)
                    {
                        items.Add(item);
                    }
                    items.Add(temp);
                    availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                    EditorUtility.SetDirty(availableScriptableObject);

                    AssetDatabase.SaveAssets();
                }
            }
        }*/
    }

}
