using AsmdefHelper.CustomCreate.Editor;
using RicTools.Editor.Utilities;
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
            var setup = (SetupScriptableObject)target;

            string previousProjectName = setup.projectName;

            base.OnInspectorGUI();

            if(setup.rootNamespace == previousProjectName)
            {
                setup.rootNamespace = setup.projectName;
            }


            if (GUILayout.Button("Delete Setup"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Delete setup?", "Yes", "No"))
                    DeleteSetup();
            }

            if (GUILayout.Button("Setup Project"))
            {
                if (setup.createAssemblyDefinitions && string.IsNullOrEmpty(setup.projectName))
                {
                    EditorUtility.DisplayDialog("Error", "Project Name cannot be null", "Ok");
                    return;
                }

                if (string.IsNullOrEmpty(setup.rootNamespace))
                {
                    setup.rootNamespace = setup.projectName;
                }

                CreateFolders();
                CreateAssemblyDefinitions();

                //DeleteSetup();
            }
        }

        private void CreateFolders()
        {
            var setup = (SetupScriptableObject)target;
            var scriptsFolder = $"Assets/{setup.scriptsFolder}";
            var editorFolder = $"Assets/{setup.editorFolder}";
            RicUtilities.CreateAssetFolder(scriptsFolder);
            RicUtilities.CreateAssetFolder(editorFolder);

            foreach (var folder in setup.folders)
            {
                RicUtilities.CreateAssetFolder($"Assets/{folder}");
            }
        }

        private void CreateAssemblyDefinitions()
        {
            var setup = (SetupScriptableObject)target;
            if (!setup.createAssemblyDefinitions) return;
            var scriptsFolder = $"Assets/{setup.scriptsFolder}";
            var editorFolder = $"Assets/{setup.editorFolder}";

            List<string> references = new List<string>();

            foreach (var assemblyDef in setup.assemblyDefinitions)
            {
                references.Add("GUID:" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(assemblyDef)).ToString());
            }

            List<string> editorReferences = new List<string>();

            editorReferences.AddRange(references);

            foreach (var assemblyDef in setup.editorAssemblyDefinitions)
            {
                editorReferences.Add("GUID:" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(assemblyDef)).ToString());
            }

            FileUtilities.CreateAsmDef(new AssemblyDefinitionJson()
            {
                name = setup.projectName,
#if UNITY_2020_2_OR_NEWER
                rootNamespace = setup.rootNamespace,
#endif
                allowUnsafeCode = false,
                autoReferenced = true,
                overrideReferences = false,
                noEngineReferences = false,
                references = references.ToArray(),
                includePlatforms = new string[0]
            }, scriptsFolder);

            editorReferences.Add("GUID:" + AssetDatabase.AssetPathToGUID($"{scriptsFolder}/{setup.projectName}.asmdef").ToString());

            FileUtilities.CreateAsmDef(new AssemblyDefinitionJson()
            {
                name = setup.projectName + ".Editor",
#if UNITY_2020_2_OR_NEWER
                rootNamespace = setup.rootNamespace + ".Editor",
#endif
                allowUnsafeCode = false,
                autoReferenced = true,
                overrideReferences = false,
                noEngineReferences = false,
                references = editorReferences.ToArray(),
                includePlatforms = new string[] { "Editor" }
            }, editorFolder);
        }
    }
}
