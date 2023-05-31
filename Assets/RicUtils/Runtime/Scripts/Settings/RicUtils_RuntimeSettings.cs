using RicUtils.Utilities;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Settings
{
    [System.Serializable]
    [ExcludeFromPreset]
    internal class RicUtils_RuntimeSettings : ScriptableObject
    {
        private static RicUtils_RuntimeSettings s_Instance;


        public static SingletonManager[] singletonManagers
        {
            get { return instance.m_singletonManagers; }
        }
        public SingletonManager[] m_singletonManagers = new SingletonManager[] { };

        public static RicUtils_RuntimeSettings instance
        {
            get
            {
                if (RicUtils_RuntimeSettings.s_Instance == null)
                {
                    RicUtils_RuntimeSettings.s_Instance = Resources.Load<RicUtils_RuntimeSettings>(PathConstants.RUNTIME_SETTINGS_FILE_PATH);
#if UNITY_EDITOR
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RicUtils_RuntimeSettings>();
                        RicUtilities.CreateAssetFolder(PathConstants.RUNTIME_SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{PathConstants.RUNTIME_SETTINGS_PATH}/{PathConstants.RUNTIME_SETTINGS_NAME}.asset");
                        AssetDatabase.SaveAssets();
                    }
#endif
                }

                return RicUtils_RuntimeSettings.s_Instance;
            }
        }

        public static RicUtils_RuntimeSettings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                RicUtils_RuntimeSettings settings = Resources.Load<RicUtils_RuntimeSettings>(PathConstants.RUNTIME_SETTINGS_FILE_PATH);
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }

        public static RicUtils_RuntimeSettings GetSettings()
        {
            if (RicUtils_RuntimeSettings.instance == null) return null;

            return RicUtils_RuntimeSettings.instance;
        }

#if UNITY_EDITOR
        public static SerializedObject GetSerializedObject()
        {
            return new SerializedObject(instance);
        }
#endif
    }
}
