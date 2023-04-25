using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    [System.Serializable]
    [ExcludeFromPreset]
    internal class EditorSettings : ScriptableObject
    {
        private static EditorSettings s_Instance;

        private const string SETTINGS_PATH = "Assets/RicUtils/Editor/Resources";

        public static string version
        {
            get { return "1.4.1"; }
        }

        public static ScriptableEditor[] scriptableEditors
        {
            get { return instance.m_scriptableEditors; }
        }
        public ScriptableEditor[] m_scriptableEditors;

        public static EditorSettings instance
        {
            get
            {
                if (EditorSettings.s_Instance == null)
                {
                    EditorSettings.s_Instance = Resources.Load<EditorSettings>("RicUtils Editor Settings");
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<EditorSettings>();
                        RicUtilities.CreateAssetFolder(SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{SETTINGS_PATH}/RicUtils Editor Settings.asset");
                        AssetDatabase.SaveAssets();
                    }
                }

                return EditorSettings.s_Instance;
            }
        }

        public static EditorSettings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                EditorSettings settings = Resources.Load<EditorSettings>("RicUtils Editor Settings");
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }

        public static EditorSettings GetSettings()
        {
            if (EditorSettings.instance == null) return null;

            return EditorSettings.instance;
        }

        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }
    }
}
