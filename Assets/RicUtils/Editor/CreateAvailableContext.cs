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

            string @namespace = mono.GetClass().Namespace;

            int indentation = 0;

            if (!string.IsNullOrEmpty(@namespace))
            {
                s += AddIndentation("namespace " + @namespace + "\n{\n", indentation);
                indentation++;
            }

            s += AddIndentation("public class " + className + " : AvailableScriptableObject<" + mono.GetClass().Name + ">\n", indentation);
            s += AddIndentation("{\n", indentation);
            indentation++;

            while(indentation > 0)
            {
                indentation--;
                s += AddIndentation("}\n", indentation);
            }

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

        private static string AddIndentation(string toAdd, int indentation)
        {
            var text = "";
            for (int i = 0; i < indentation; i++)
            {
                text += "\t";
            }
            return text + toAdd;
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
