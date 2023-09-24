using RicTools.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace RicTools.Editor.ContextMenu
{
    public static class CreateScriptableObjectContextMenu
    {
        [MenuItem("Assets/Create/RicTools/Scriptable Object", priority = -10)]
        public static void Create()
        {
            var window = ScriptableObject.CreateInstance<CreateScriptableObjectEditorWindow>();

            window.ShowUtility();
        }
    }
}
