using RicTools.Editor.Utilities;
using RicTools.Editor.Windows;
using RicTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicTools.Editor.ToolbarMenuItems
{
    internal static class CreateScriptableObjectToolbar
    {
        [MenuItem("RicTools/Create New Scriptable Object", priority = 1)]
        public static void CreateAvailableScripts()
        {
            var window = ScriptableObject.CreateInstance<CreateScriptableObjectEditorWindow>();
            window.useCurrentProjectLocation = true;
            window.ShowUtility();
        }
    }
}
