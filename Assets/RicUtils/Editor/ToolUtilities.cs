using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class ToolUtilities
    {
        public static List<ScriptableEditor> GetScriptableEditors()
        {
            return new List<ScriptableEditor>(EditorSettings.scriptableEditors);
        }

        public static List<T> FindAssetsByType<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static bool HasCustomEditor(System.Type type)
        {
            return GetScriptableEditors().Exists(se => se.HasCustomEditor(type));
        }

        public static System.Type GetCustomEditorType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(type)) return keyValuePair.EditorType;
            }
            return null;
        }

        public static System.Type GetAvailableScriptableObjectType(System.Type type)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasAvailableScriptableObject(type)) return keyValuePair.AvailableScriptableObjectType;
            }
            return null;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var temp = EditorUtility.InstanceIDToObject(instanceId) as GenericScriptableObject;
            if (temp != null)
            {
                return OpenScriptableObjectFile(temp);
            }
            return false;
        }

        private static bool OpenScriptableObjectFile(GenericScriptableObject so)
        {
            foreach (var keyValuePair in GetScriptableEditors())
            {
                if (keyValuePair.HasCustomEditor(so.GetType()))
                {
                    var showWindow = keyValuePair.EditorType.GetMethod("ShowWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (showWindow == null)
                    {
                        Debug.LogError(keyValuePair.EditorType + " has no ShowWindow static function");
                        return false;
                    }
                    var temp = showWindow.Invoke(null, null);
                    var data = System.Convert.ChangeType(temp, keyValuePair.EditorType);
                    keyValuePair.EditorType.GetField("scriptableObject").SetValue(data, so);
                    keyValuePair.EditorType.GetMethod("LoadScriptableObject").Invoke(data, new object[] { so, so == null });
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetActiveFolderPath(out string path)
        {
            var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] args = new object[] { null };
            bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
            path = (string)args[0];

            return found;
        }

        public static void CreateNewScript(string defaultNewFileName, string templatePath)
        {
            CreateNewScript<DoCreateScriptAsset>(defaultNewFileName, templatePath);
        }

        public static void CreateNewScript<T>(string defaultNewFileName, string templatePath) where T : EndNameEditAction
        {
            var endAction = ScriptableObject.CreateInstance<T>();
            CreateNewScript(endAction, defaultNewFileName, templatePath);
        }

        public static void CreateNewScript<T>(T endAction, string defaultNewFileName, string templatePath) where T : EndNameEditAction
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(icon: Path.GetExtension(defaultNewFileName) switch
            {
                ".cs" => EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                ".shader" => EditorGUIUtility.IconContent("Shader Icon").image as Texture2D,
                ".asmdef" => EditorGUIUtility.IconContent("AssemblyDefinitionAsset Icon").image as Texture2D,
                ".asmref" => EditorGUIUtility.IconContent("AssemblyDefinitionReferenceAsset Icon").image as Texture2D,
                _ => EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D,
            }, instanceID: 0, endAction: endAction, pathName: defaultNewFileName, resourceFile: templatePath);
            AssetDatabase.Refresh();
        }
    }

    internal class CustomScriptableObjectProcessing : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            var temp = AssetDatabase.LoadMainAssetAtPath(AssetPath);
            if (temp == null) return AssetDeleteResult.DidNotDelete;
            if (ToolUtilities.HasCustomEditor(temp.GetType()))
            {
                var availableScriptableObjectType = ToolUtilities.GetAvailableScriptableObjectType(temp.GetType());
                var availableScriptableObject = AssetDatabase.LoadMainAssetAtPath(RicUtilities.GetAvailableScriptableObjectPath(availableScriptableObjectType));
                var baseType = temp.GetType();
                while (baseType.BaseType != null && baseType.BaseType != typeof(GenericScriptableObject))
                {
                    baseType = baseType.BaseType;
                }
                if (availableScriptableObject != null)
                {
                    var itemsField = availableScriptableObjectType.GetField("items");
                    var itemsArray = (object[])itemsField.GetValue(availableScriptableObject);
                    var items = (IList)System.Activator.CreateInstance(typeof(List<>).MakeGenericType(baseType));
                    foreach (var item in itemsArray)
                    {
                        items.Add(item);
                    }
                    items.Remove(temp);
                    availableScriptableObjectType.GetMethod("SetItems").Invoke(availableScriptableObject, new object[] { items });

                    EditorUtility.SetDirty(availableScriptableObject);

                    //AssetDatabase.SaveAssets();
                }
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }

    public class DoCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        private Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string content = File.ReadAllText(resourceFile);
            var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", BindingFlags.Static | BindingFlags.NonPublic);
            return (Object)method.Invoke(null, new object[] { pathName, PreprocessScriptAssetTemplate(pathName, content) });
        }

        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ProjectWindow/ProjectWindowUtil.cs
        private string PreprocessScriptAssetTemplate(string pathName, string resourceContent)
        {
            string rootNamespace = null;

            if (Path.GetExtension(pathName) == ".cs")
            {
                rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(pathName);
            }

            string content = resourceContent;

            // #NOTRIM# is a special marker that is used to mark the end of a line where we want to leave whitespace. prevent editors auto-stripping it by accident.
            content = content.Replace("#NOTRIM#", "");

            // macro replacement
            string baseFile = Path.GetFileNameWithoutExtension(pathName);

            content = content.Replace("#NAME#", baseFile);
            string baseFileNoSpaces = baseFile.Replace(" ", "");
            content = content.Replace("#SCRIPTNAME#", baseFileNoSpaces);

            content = CustomReplaces(content);

            content = RemoveOrInsertNamespace(content, rootNamespace);

            // if the script name begins with an uppercase character we support a lowercase substitution variant
            if (char.IsUpper(baseFileNoSpaces, 0))
            {
                baseFileNoSpaces = char.ToLower(baseFileNoSpaces[0]) + baseFileNoSpaces.Substring(1);
                content = content.Replace("#SCRIPTNAME_LOWER#", baseFileNoSpaces);
            }
            else
            {
                // still allow the variant, but change the first character to upper and prefix with "my"
                baseFileNoSpaces = "my" + char.ToUpper(baseFileNoSpaces[0]) + baseFileNoSpaces.Substring(1);
                content = content.Replace("#SCRIPTNAME_LOWER#", baseFileNoSpaces);
            }

            return content;
        }

        protected virtual string CustomReplaces(string content)
        {
            return content;
        }

        private string RemoveOrInsertNamespace(string content, string rootNamespace)
        {
            var rootNamespaceBeginTag = "#ROOTNAMESPACEBEGIN#";
            var rootNamespaceEndTag = "#ROOTNAMESPACEEND#";

            if (!content.Contains(rootNamespaceBeginTag) || !content.Contains(rootNamespaceEndTag))
                return content;

            if (string.IsNullOrEmpty(rootNamespace))
            {
                content = Regex.Replace(content, $"((\\r\\n)|\\n)[ \\t]*{rootNamespaceBeginTag}[ \\t]*", string.Empty);
                content = Regex.Replace(content, $"((\\r\\n)|\\n)[ \\t]*{rootNamespaceEndTag}[ \\t]*", string.Empty);

                return content;
            }

            // Use first found newline character as newline for entire file after replace.
            var newline = content.Contains("\r\n") ? "\r\n" : "\n";
            var contentLines = new List<string>(content.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None));

            int i = 0;

            for (; i < contentLines.Count; ++i)
            {
                if (contentLines[i].Contains(rootNamespaceBeginTag))
                    break;
            }

            var beginTagLine = contentLines[i];

            // Use the whitespace between beginning of line and #ROOTNAMESPACEBEGIN# as identation.
            var indentationString = beginTagLine.Substring(0, beginTagLine.IndexOf("#"));

            contentLines[i] = $"namespace {rootNamespace}";
            contentLines.Insert(i + 1, "{");

            i += 2;

            for (; i < contentLines.Count; ++i)
            {
                var line = contentLines[i];

                if (string.IsNullOrEmpty(line) || line.Trim().Length == 0)
                    continue;

                if (line.Contains(rootNamespaceEndTag))
                {
                    contentLines[i] = "}";
                    break;
                }

                contentLines[i] = $"{indentationString}{line}";
            }

            return string.Join(newline, contentLines.ToArray());
        }
    }
}
