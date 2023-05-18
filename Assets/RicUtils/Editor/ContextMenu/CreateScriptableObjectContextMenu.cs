using RicUtils.Editor.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEngine;

namespace RicUtils.Editor.ContextMenu
{
    public static class CreateScriptableObjectContextMenu
    {

        [MenuItem("Assets/Create/Generic Scriptable Object", priority = -10)]
        public static void Create()
        {
            ToolUtilities.TryGetActiveFolderPath(out string path);

            string defaultNewFileName = Path.Combine(path, "NewGenericScriptableObject.cs");

            string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericScriptableObject.cs.txt";

            ToolUtilities.CreateNewScript(defaultNewFileName, templatePath);

            //ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/RicUtils/Editor/Templates/Script-NewGenericScriptableObject.cs.txt", Path.Combine(path, "NewGenericScriptableObject.cs"));

        }
    }
}
