using UnityEditorInternal;
using UnityEngine;

namespace RicTools
{
    public class SetupScriptableObject : ScriptableObject
    {
        public string projectName;
        public string scriptsFolder;
        public string editorFolder;
        public string[] folders;
        public AssemblyDefinitionAsset[] assemblies;
        public AssemblyDefinitionAsset[] editorAssemblies;
    }
}
