using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public List<T> Items
        {
            get
            {
                _items.RemoveNullElements();
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        List<T> _items = new List<T>();

        public void AddItem(T item)
        {
            if (Items.Contains(item))
                return;

            Items.Add(item);
        }

        public void RemoveItem(T item)
        {
            if (!Items.Contains(item))
                return;

            Items.Remove(item);
        }

        public T GetItem(int index)
        {
            if (index < 0 || index >= Items.Count)
                return default(T);

            return Items[index];
        }
    }
}