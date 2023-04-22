using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class RicUtils_Settings : ScriptableObject
    {
        private static RicUtils_Settings s_Instance;

        private const string SETTINGS_PATH = "Assets/RicUtils/Resources";


        public static SingletonManager[] singletonManagers
        {
            get { return instance.m_singletonManagers; }
        }
        public SingletonManager[] m_singletonManagers;

        public static RicUtils_Settings instance
        {
            get
            {
                if (RicUtils_Settings.s_Instance == null)
                {
                    RicUtils_Settings.s_Instance = Resources.Load<RicUtils_Settings>("RicUtils Settings");
#if UNITY_EDITOR
                    if (!s_Instance)
                    {
                        s_Instance = ScriptableObject.CreateInstance<RicUtils_Settings>();
                        RicUtilities.CreateAssetFolder(SETTINGS_PATH);
                        AssetDatabase.CreateAsset(s_Instance, $"{SETTINGS_PATH}/RicUtils Settings.asset");
                        AssetDatabase.SaveAssets();
                    }
#endif
                }

                return RicUtils_Settings.s_Instance;
            }
        }

        public static RicUtils_Settings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                RicUtils_Settings settings = Resources.Load<RicUtils_Settings>("RicUtils Settings");
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }

        public static RicUtils_Settings GetSettings()
        {
            if (RicUtils_Settings.instance == null) return null;

            return RicUtils_Settings.instance;
        }
    }
}
