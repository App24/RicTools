using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RicUtils.Editor
{
    public abstract class GenericEditorWindow<T, D> : EditorWindow where T : GenericScriptableObject where D : AvailableScriptableObject<T>
    {
        public T scriptableObject;

        protected virtual string AvailableSOPath => RicUtilities.GetAvailableScriptableObjectPath(typeof(D));

        protected virtual string SavePath => RicUtilities.GetScriptableObjectPath(typeof(T));

        protected string spawnableId;

        protected SerializedObject serializedObject;
        private Dictionary<string, (SerializedProperty, object)> data = new Dictionary<string, (SerializedProperty, object)>();

        private void OnGUI()
        {
            EditorUtilities.DrawObjectField(ref scriptableObject, "Scriptable Object", () =>
            {
                LoadScriptableObject(scriptableObject, scriptableObject == null);
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

        protected void DrawIDInput(ref string id)
        {
            GUI.enabled = scriptableObject == null;
            EditorUtilities.DrawStringInput(ref id, "ID");
            GUI.enabled = true;
        }

        protected void DrawSaveDeleteButtons(bool checkExists = true)
        {
            EditorUtilities.DrawSeparator();
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


                //EditorUtility.SetDirty(item);

                //AssetDatabase.SaveAssetIfDirty(available);

                scriptableObject = item;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(scriptableObject == null);
            if (GUILayout.Button("Delete Asset"))
            {
                if (!EditorUtility.DisplayDialog("Warning", "You sure you want to delete this asset?", "Continue", "Cancel"))
                    return;

                /*D available = GetAvailableAsset();

                List<T> items = new List<T>(available.items);

                items.Remove(scriptableObject);

                available.items = items.ToArray();


                EditorUtility.SetDirty(available);

                AssetDatabase.SaveAssets();*/

                //AssetDatabase.SaveAssetIfDirty(available);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(scriptableObject));

                scriptableObject = null;
                LoadScriptableObject(scriptableObject, true);
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

        public virtual void LoadScriptableObject(T so, bool isNull)
        {
            if (isNull)
            {
                spawnableId = "";
            }
            else
            {
                spawnableId = so.id;
            }
        }

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
