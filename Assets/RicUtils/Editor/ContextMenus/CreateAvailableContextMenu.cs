using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class CreateAvailableContextMenu
    {
        [MenuItem("Assets/Create/Available Scriptable Object", priority = -9)]
        public static void Create()
        {
            var mono = Selection.activeObject as MonoScript;

            string className = $"Available{mono.GetClass().Name}";

            ToolUtilities.TryGetActiveFolderPath(out string path);

            string defaultNewFileName = Path.Combine(path, className + ".cs");

            string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewAvailableScriptableObject.cs.txt";

            var endAction = ScriptableObject.CreateInstance<DoCreateAvailableAsset>();

            endAction.scriptableObject = mono.GetClass().Name;

            ToolUtilities.CreateNewScript(endAction, defaultNewFileName, templatePath);
        }

        [MenuItem("Assets/Create/Available Scriptable Object", true)]
        public static bool IsValid()
        {
            if (!(Selection.activeObject is MonoScript mono)) return false;
            if (mono.GetClass() == null) return false;
            if (!mono.GetClass().IsSubclassOf(typeof(GenericScriptableObject))) return false;
            return true;
        }
    }

    internal class DoCreateAvailableAsset : DoCreateScriptAsset
    {
        internal string scriptableObject;

        protected override string CustomReplaces(string content)
        {
            content = content.Replace("#SCRIPTABLEOBJECT#", scriptableObject);
            return content;
        }
    }
}
