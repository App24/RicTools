using RicUtils.Editor.Utilities;
using RicUtils.ScriptableObjects;
using RicUtils.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor.ContextMenu
{
    public static class CreateEditorContextMenu
    {

        /*[MenuItem("Assets/Create/Editor Window", priority = -8)]
        public static void Create()
        {
            string scriptableObject = "";
            string availableScriptableObject = "";

            foreach (var obj in Selection.objects)
            {
                var mono = obj as MonoScript;
                var @class = mono.GetClass();
                if (@class.IsSubclassOf(typeof(GenericScriptableObject))) scriptableObject = @class.Name;
                else if (RicUtilities.IsSubclassOfRawGeneric(typeof(AvailableScriptableObject<>), @class)) availableScriptableObject = @class.Name;
            }

            string className = $"{scriptableObject}EditorWindow";

            ToolUtilities.TryGetActiveFolderPath(out string path);

            string defaultNewFileName = Path.Combine(path, className + ".cs");

            string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericEditorWindow.cs.txt";

            var endAction = ScriptableObject.CreateInstance<DoCreateEditorAsset>();

            endAction.scriptableObject = scriptableObject;
            endAction.availableScriptableObject = availableScriptableObject;

            ToolUtilities.CreateNewScript(endAction, defaultNewFileName, templatePath);
        }

        [MenuItem("Assets/Create/Editor Window", true)]
        public static bool IsValid()
        {
            if (Selection.objects.Length < 2) return false;

            bool hasGenericScriptableObject = false;
            bool hasAvailableScriptableObject = false;

            foreach (var obj in Selection.objects)
            {
                if (obj is not MonoScript mono) return false;
                var @class = mono.GetClass();
                if (mono.GetClass() == null) continue;
                if (@class.IsSubclassOf(typeof(GenericScriptableObject))) hasGenericScriptableObject = true;
                else if (RicUtilities.IsSubclassOfRawGeneric(typeof(AvailableScriptableObject<>), @class)) hasAvailableScriptableObject = true;
            }

            return hasGenericScriptableObject && hasAvailableScriptableObject;
        }*/
    }

    internal class DoCreateEditorAsset : DoCreateScriptAsset
    {
        internal string scriptableObject;
        internal string availableScriptableObject;

        protected override string CustomReplaces(string content)
        {
            content = content.Replace("#SCRIPTABLEOBJECT#", scriptableObject);
            content = content.Replace("#AVAILABLESCRIPTABLEOBJECT#", availableScriptableObject);
            return content;
        }
    }
}
