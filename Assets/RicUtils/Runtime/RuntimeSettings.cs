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
                    RuntimeSettings.s_Instance = Resources.Load<RuntimeSettings>(PathConstants.RUNTIME_SETTINGS_NAME);
#if UNITY_EDITOR
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RuntimeSettings>();
                        RicUtilities.CreateAssetFolder(PathConstants.RUNTIME_SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{PathConstants.RUNTIME_SETTINGS_PATH}/{PathConstants.RUNTIME_SETTINGS_NAME}.asset");
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
                RuntimeSettings settings = Resources.Load<RuntimeSettings>(PathConstants.RUNTIME_SETTINGS_NAME);
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

#if UNITY_EDITOR
        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }
#endif
    }
}
