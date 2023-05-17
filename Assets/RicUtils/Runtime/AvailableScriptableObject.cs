using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicUtils
{
    public class AvailableScriptableObject<T> : ScriptableObject where T : GenericScriptableObject
    {
        public T[] items;
        public List<T> Items => new List<T>(items);

        public void SetItems(IList<T> items)
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
    }
}