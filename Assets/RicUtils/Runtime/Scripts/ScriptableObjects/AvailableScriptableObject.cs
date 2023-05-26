using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils.ScriptableObjects
{
    public class AvailableScriptableObject<T> : ScriptableObject where T : GenericScriptableObject
    {
        public T[] items = new T[] { };
        public List<T> Items => new List<T>(items);

        private void SetItems(IList<T> items)
        {
            var temp = new T[items.Count];
            items.CopyTo(temp, 0);
            this.items = temp;
        }

        public T this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public T this[string id]
        {
            get
            {
                var item = Items.Find(i => i.id == id);
                return item;
            }
            set
            {
                int index = Items.FindIndex(i => i.id == id);
                if (index < 0)
                {
                    Items.Add(value);
                    return;
                }

                Items.RemoveAt(index);
                Items.Insert(index, value);
            }
        }
    }
}