using RicUtils.Editor.Utilities;
using RicUtils.ScriptableObjects;
using RicUtils.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RicUtils.Editor.Windows
{
    public abstract class GenericEditorWindow<T, D> : EditorWindow where T : GenericScriptableObject where D : AvailableScriptableObject<T>
    {
        protected T scriptableObject;

        protected virtual string AvailableSOPath => RicUtilities.GetAvailableScriptableObjectPath(typeof(D));

        protected virtual string SavePath => RicUtilities.GetScriptableObjectPath(typeof(T));

        protected string spawnableId;

        protected SerializedObject serializedObject;

        private void OnGUI()
        {
            EditorGUIHelper.DrawObjectField(ref scriptableObject, "Scriptable Object", () =>
            {
                LoadScriptableObjectInternal(scriptableObject);
            });

            DrawIDInput(ref spawnableId);

            DrawGUI();

            DrawSaveDeleteButtons();
        }

        protected abstract void DrawGUI();

        protected virtual void OnEnable()
        {
            serializedObject = new SerializedObject(this);
        }

        private void DrawIDInput(ref string id)
        {
            GUI.enabled = scriptableObject == null;
            EditorGUIHelper.DrawStringInput(ref id, "ID");
            GUI.enabled = true;
        }

        private void DrawSaveDeleteButtons(bool checkExists = true)
        {
            EditorGUIHelper.DrawSeparator();
            List<CompleteCriteria> criteria = new List<CompleteCriteria>(GetInbuiltCompleteCriteria());
            criteria.AddRange(GetCompleteCriteria());
            string tooltip = "";
            bool complete = true;
            foreach (var c in criteria)
            {
                if (!c.isComplete)
                {
                    complete = false;
                    tooltip += $"{c.tooltip}\n";
                }
            }
            tooltip = tooltip.Trim();
            EditorGUI.BeginDisabledGroup(!complete);
            if (GUILayout.Button(new GUIContent("Save Asset", tooltip)))
            {
                D available = GetAvailableAsset();

                List<T> items = new List<T>(available.items);

                int index = -1;

                if (AssetDatabase.FindAssets($"{spawnableId}", new string[] { SavePath }).Length > 0)
                {
                    if (checkExists)
                    {
                        if (!EditorUtility.DisplayDialog("Error", "There is an asset by that ID already, you sure you want to replace it?", "Continue", "Cancel"))
                            return;
                    }
                    var asset = AssetDatabase.LoadAssetAtPath<T>($"{SavePath}/{spawnableId}.asset");
                    index = items.IndexOf(asset);
                }

                var item = ScriptableObject.CreateInstance<T>();


                CreateAsset(ref item);

                item.id = spawnableId;

                SaveAsset(item, spawnableId);

                if (index < 0)
                    items.Add(item);
                else
                    items[index] = item;

                available.items = items.ToArray();

                EditorUtility.SetDirty(available);

                AssetDatabase.SaveAssets();

                scriptableObject = item;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(scriptableObject == null);
            if (GUILayout.Button("Delete Asset"))
            {
                if (!EditorUtility.DisplayDialog("Warning", "You sure you want to delete this asset?", "Continue", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(scriptableObject));

                scriptableObject = null;
                LoadScriptableObjectInternal(scriptableObject);
            }
            EditorGUI.EndDisabledGroup();
        }

        private D GetAvailableAsset()
        {
            RicUtilities.CreateAssetFolder(AvailableSOPath);

            D available = AssetDatabase.LoadAssetAtPath<D>(AvailableSOPath);
            if (available == null)
            {

                available = ScriptableObject.CreateInstance<D>();

                available.items = new T[] { };

                AssetDatabase.CreateAsset(available, AvailableSOPath);

                AssetDatabase.SaveAssets();

                available = AssetDatabase.LoadAssetAtPath<D>(AvailableSOPath);
            }
            return available;
        }

        private void SaveAsset(T asset, string saveName)
        {
            RicUtilities.CreateAssetFolder(SavePath);
            if (!AssetDatabase.Contains(asset))
                AssetDatabase.CreateAsset(asset, $"{SavePath}/{saveName}.asset");
        }

        private void LoadScriptableObjectInternal(T so)
        {
            bool isNull = so == null;
            if (isNull)
            {
                spawnableId = "";
            }
            else
            {
                spawnableId = so.id;
            }

            LoadScriptableObject(so, isNull);
        }

        protected abstract void LoadScriptableObject(T so, bool isNull);

        protected abstract void CreateAsset(ref T asset);

        private IEnumerable<CompleteCriteria> GetInbuiltCompleteCriteria()
        {
            yield return new CompleteCriteria(!string.IsNullOrWhiteSpace(spawnableId), "Empty ID");
        }

        protected virtual IEnumerable<CompleteCriteria> GetCompleteCriteria()
        {
            yield return new CompleteCriteria(true, "");
        }
    }

    public class CompleteCriteria
    {
        public readonly bool isComplete;
        public readonly string tooltip;

        public CompleteCriteria(bool isComplete, string tooltip)
        {
            this.isComplete = isComplete;
            this.tooltip = tooltip;
        }
    }
}
