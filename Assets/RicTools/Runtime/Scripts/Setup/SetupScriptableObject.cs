using UnityEditorInternal;
using UnityEngine;

namespace RicTools
{
    public class SetupScriptableObject : ScriptableObject
    {
        public bool createAssemblyDefinitions;
        public string projectName;
        public string rootNamespace;
        public AssemblyDefinitionAsset[] assemblyDefinitions;
        public AssemblyDefinitionAsset[] editorAssemblyDefinitions;

        public string scriptsFolder;
        public string editorFolder;
        public string[] folders;
        /*public string projectName;
        public string scriptsFolder;
        public string editorFolder;
        public string[] folders;
        public AssemblyDefinitionAsset[] assemblies;
        public AssemblyDefinitionAsset[] editorAssemblies;*/
    }
}
