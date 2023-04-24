using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class RicUtils_EditorSettings : ScriptableObject
    {
        private static RicUtils_EditorSettings s_Instance;

        private const string SETTINGS_PATH = "Assets/RicUtils/Editor/Resources";

        public static string version
        {
            get { return "1.4.0"; }
        }

        public static ScriptableEditor[] scriptableEditors
        {
            get { return instance.m_scriptableEditors; }
        }
        public ScriptableEditor[] m_scriptableEditors;

        public static RicUtils_EditorSettings instance
        {
            get
            {
                if (RicUtils_EditorSettings.s_Instance == null)
                {
                    RicUtils_EditorSettings.s_Instance = Resources.Load<RicUtils_EditorSettings>("RicUtils Editor Settings");
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RicUtils_EditorSettings>();
                        RicUtilities.CreateAssetFolder(SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{SETTINGS_PATH}/RicUtils Editor Settings.asset");
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
                RicUtils_EditorSettings settings = Resources.Load<RicUtils_EditorSettings>("RicUtils Editor Settings");
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
    }
}
