using RicUtils.Utilities;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor.Settings
{
    [System.Serializable]
    [ExcludeFromPreset]
    internal class RicUtils_EditorSettings : ScriptableObject
    {
        private static RicUtils_EditorSettings s_Instance;

        public static string version
        {
            get { return "1.6.2"; }
        }

        public static ScriptableEditor[] scriptableEditors
        {
            get { return instance.m_scriptableEditors; }
        }
        public ScriptableEditor[] m_scriptableEditors = new ScriptableEditor[] { };

        public static RicUtils_EditorSettings instance
        {
            get
            {
                if (RicUtils_EditorSettings.s_Instance == null)
                {
                    RicUtils_EditorSettings.s_Instance = Resources.Load<RicUtils_EditorSettings>(PathConstants.EDITOR_SETTINGS_FILE_PATH);
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RicUtils_EditorSettings>();
                        RicUtilities.CreateAssetFolder(PathConstants.EDITOR_SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{PathConstants.EDITOR_SETTINGS_PATH}/{PathConstants.EDITOR_SETTINGS_NAME}.asset");
                        AssetDatabase.SaveAssets();
                    }
                }

                return RicUtils_EditorSettings.s_Instance;
            }
        }

        public static RicUtils_EditorSettings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                RicUtils_EditorSettings settings = Resources.Load<RicUtils_EditorSettings>(PathConstants.EDITOR_SETTINGS_FILE_PATH);
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }

        public static RicUtils_EditorSettings GetSettings()
        {
            if (RicUtils_EditorSettings.instance == null) return null;

            return RicUtils_EditorSettings.instance;
        }

        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }
    }
}
