using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class RuntimeSettings : ScriptableObject
    {
        private static RuntimeSettings s_Instance;

        private const string SETTINGS_PATH = "Assets/RicUtils/Resources";


        public static SingletonManager[] singletonManagers
        {
            get { return instance.m_singletonManagers; }
        }
        public SingletonManager[] m_singletonManagers;

        public static RuntimeSettings instance
        {
            get
            {
                if (RuntimeSettings.s_Instance == null)
                {
                    RuntimeSettings.s_Instance = Resources.Load<RuntimeSettings>("RicUtils Settings");
#if UNITY_EDITOR
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RuntimeSettings>();
                        RicUtilities.CreateAssetFolder(SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{SETTINGS_PATH}/RicUtils Settings.asset");
                        AssetDatabase.SaveAssets();
                    }
#endif
                }

                return RuntimeSettings.s_Instance;
            }
        }

        public static RuntimeSettings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                RuntimeSettings settings = Resources.Load<RuntimeSettings>("RicUtils Settings");
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }

        public static RuntimeSettings GetSettings()
        {
            if (RuntimeSettings.instance == null) return null;

            return RuntimeSettings.instance;
        }

        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }
    }
}
