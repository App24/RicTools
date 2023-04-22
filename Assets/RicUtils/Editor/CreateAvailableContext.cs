using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    public class CreateAvailableContext : MonoBehaviour
    {
        [MenuItem("Assets/Create Available Scriptable Object", priority = -10)]
        public static void Create()
        {
            var mono = Selection.activeObject as MonoScript;

            string className = $"Available{mono.GetClass().Name}";

            string s = "using RicUtils;\n\n";

            s += "public class " + className + " : AvailableScriptableObject<" + mono.GetClass().Name + ">\n{\n";

            s += "}\n";

            var path = "";
            var obj = Selection.activeObject;
            if (obj == null) path = "Assets";
            else path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj.GetInstanceID()));

            string outfile = Path.Combine(path, className + ".cs");
            using (System.IO.StreamWriter sw =
                new System.IO.StreamWriter(outfile, false))
            {
                sw.Write(s);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create Available Scriptable Object", true)]
        public static bool IsValid()
        {
            if (!(Selection.activeObject is MonoScript mono)) return false;
            if(!mono.GetClass().IsSubclassOf(typeof(CustomScriptableObject))) return false;
            return true;
        }
    }
}
