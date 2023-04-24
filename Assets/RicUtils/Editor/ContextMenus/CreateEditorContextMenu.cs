using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class CreateEditorContextMenu
    {

        [MenuItem("Assets/Create/Editor Window", priority = -8)]
        public static void Create()
        {
            string scriptableObject = "";
            string availableScriptableObject = "";

            foreach (var obj in Selection.objects)
            {
                var mono = obj as MonoScript;
                var @class = mono.GetClass();
                if (@class.IsSubclassOf(typeof(GenericScriptableObject))) scriptableObject = @class.Name;
                else if (IsSubclassOfRawGeneric(typeof(AvailableScriptableObject<>), @class)) availableScriptableObject = @class.Name;
            }

            string className = $"{scriptableObject}EditorWindow";

            ToolUtilities.TryGetActiveFolderPath(out string path);

            string defaultNewFileName = Path.Combine(path, className + ".cs");

            string templatePath = "Assets/RicUtils/Editor/Templates/Script-NewGenericEditorWindow.cs.txt";

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
                if (obj is not MonoScript) return false;
                var mono = obj as MonoScript;
                var @class = mono.GetClass();
                if (mono.GetClass() == null) continue;
                if (@class.IsSubclassOf(typeof(GenericScriptableObject))) hasGenericScriptableObject = true;
                else if (IsSubclassOfRawGeneric(typeof(AvailableScriptableObject<>), @class)) hasAvailableScriptableObject = true;
            }

            return hasGenericScriptableObject && hasAvailableScriptableObject;
        }

        // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        private static bool IsSubclassOfRawGeneric(System.Type generic, System.Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
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
