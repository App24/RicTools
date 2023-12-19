using AsmdefHelper.CustomCreate.Editor;
using RicTools.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RicTools
{
    [CustomEditor(typeof(SetupScriptableObject))]
    public class SetupScriptableObjectCustomEditor : UnityEditor.Editor
    {
        private void DeleteSetup()
        {
            FileUtil.DeleteFileOrDirectory("Assets/RicTools/Runtime/Scripts/Setup");
            FileUtil.DeleteFileOrDirectory("Assets/RicTools/Runtime/Scripts/Setup.meta");

            FileUtil.DeleteFileOrDirectory("Assets/RicTools/Editor/Setup");
            FileUtil.DeleteFileOrDirectory("Assets/RicTools/Editor/Setup.meta");
            var ids = AssetDatabase.FindAssets("t:SetupScriptableObject");
            if (ids.Length == 1)
            {
                var path = AssetDatabase.GUIDToAssetPath(ids[0]);
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
            }
            AssetDatabase.Refresh();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var setup = (SetupScriptableObject)target;

            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Delete setup?", "Yes", "No"))
                    DeleteSetup();
            }

            if (GUILayout.Button("Setup"))
            {
                if (string.IsNullOrEmpty(setup.projectName))
                {
                    EditorUtility.DisplayDialog("Error", "Project Name cannot be null", "Ok");
                    return;
                }
                var scriptsFolder = $"Assets/{setup.scriptsFolder}";
                var editorFolder = $"Assets/{setup.editorFolder}";
                RicUtilities.CreateAssetFolder(scriptsFolder);
                RicUtilities.CreateAssetFolder(editorFolder);

                foreach (var folder in setup.folders)
                {
                    RicUtilities.CreateAssetFolder($"Assets/{folder}");
                }

                List<string> references = new List<string>();

                foreach (var assemblyDef in setup.assemblies)
                {
                    references.Add("GUID:" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(assemblyDef)).ToString());
                }

                List<string> editorReferences = new List<string>();

                editorReferences.AddRange(references);

                foreach (var assemblyDef in setup.editorAssemblies)
                {
                    editorReferences.Add("GUID:" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(assemblyDef)).ToString());
                }

                {
                    var asmdef = new AssemblyDefinitionJson
                    {
                        name = setup.projectName,
#if UNITY_2020_2_OR_NEWER
                        rootNamespace = setup.projectName,
#endif
                        allowUnsafeCode = false,
                        autoReferenced = true,
                        overrideReferences = false,
                        noEngineReferences = false,
                        references = references.ToArray(),
                        includePlatforms = new string[0]
                    };
                    var asmdefJson = JsonUtility.ToJson(asmdef, true);
                    var asmdefPath = $"{scriptsFolder}/{setup.projectName}.asmdef";
                    File.WriteAllText(asmdefPath, asmdefJson, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }

                {
                    editorReferences.Add("GUID:" + AssetDatabase.AssetPathToGUID($"{scriptsFolder}/{setup.projectName}.asmdef").ToString());
                    var editorAsmdef = new AssemblyDefinitionJson
                    {
                        name = setup.projectName + ".Editor",
#if UNITY_2020_2_OR_NEWER
                        rootNamespace = setup.projectName + ".Editor",
#endif
                        allowUnsafeCode = false,
                        autoReferenced = true,
                        overrideReferences = false,
                        noEngineReferences = false,
                        references = editorReferences.ToArray(),
                        includePlatforms = new string[] { "Editor" }
                    };
                    var asmdefJson = JsonUtility.ToJson(editorAsmdef, true);
                    var asmdefPath = $"{editorFolder}/{setup.projectName}.Editor.asmdef";
                    File.WriteAllText(asmdefPath, asmdefJson, Encoding.UTF8);
                    AssetDatabase.Refresh();
                }

                DeleteSetup();
            }
        }
    }
}
