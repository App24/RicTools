using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace RicUtils
{
    public static class RicUtilities
    {
        public static T GetAvailableScriptableObject<T, D>() where T : AvailableScriptableObject<D> where D : GenericScriptableObject
        {
            return Resources.Load<T>("Availables/" + GetAvailableScriptableObjectName(typeof(T)));
        }

        public static string GetAvailableScriptableObjectName(System.Type type)
        {
            var name = type.Name;
            name = name.Replace("ScriptableObject", "");
            return name;
        }

        public static string GetAvailableScriptableObjectPath(System.Type type)
        {
            return $"Assets/ScriptableObjects/Resources/Availables/{GetAvailableScriptableObjectName(type)}.asset";
        }

        public static string GetScriptableObjectPath(System.Type type)
        {
            var name = type.Name;
            name = name.Replace("ScriptableObject", "");
            return $"Assets/ScriptableObjects/{name}s";
        }

        public static string ToFriendlyCase(this string PascalString)
        {
            return Regex.Replace(PascalString, "(?!^)([A-Z])", " $1");
        }

#if UNITY_EDITOR
        public static void CreateAssetFolder(string folderPath)
        {
            if (folderPath.EndsWith(".asset"))
                folderPath = Path.GetDirectoryName(folderPath);
            folderPath = folderPath.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string[] dirs = folderPath.Split('/');
                string path = dirs[0];
                for (int i = 1; i < dirs.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder(path + $"/{dirs[i]}"))
                    {
                        AssetDatabase.CreateFolder(path, dirs[i]);
                    }
                    path += $"/{dirs[i]}";
                }
            }
        }
#endif
    }
}
