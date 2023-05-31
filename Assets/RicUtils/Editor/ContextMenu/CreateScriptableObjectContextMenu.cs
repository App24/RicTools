using RicUtils.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor.ContextMenu
{
    public static class CreateScriptableObjectContextMenu
    {
        [MenuItem("Assets/Create/RicUtils/Scriptable Object", priority = -10)]
        public static void Create()
        {
            var window = ScriptableObject.CreateInstance<CreateScriptableObjectEditorWindow>();

            window.ShowUtility();
        }
    }
}
