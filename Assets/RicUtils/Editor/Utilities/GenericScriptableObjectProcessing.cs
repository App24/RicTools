using RicUtils.ScriptableObjects;
using RicUtils.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace RicUtils.Editor.Utilities
{
    internal class GenericScriptableObjectProcessing : UnityEditor.AssetModificationProcessor
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
                while (baseType.BaseType != null && baseType.BaseType != typeof(GenericScriptableObject))
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
                    availableScriptableObjectType.GetMethod("SetItems", BindingFlags.NonPublic).Invoke(availableScriptableObject, new object[] { items });

                    EditorUtility.SetDirty(availableScriptableObject);

                    //AssetDatabase.SaveAssets();
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
