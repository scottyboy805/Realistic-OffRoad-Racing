using UnityEngine;
using System;
using System.Linq;

namespace RGSK
{
    public class UniqueIDScriptableObject : ScriptableObject
    {
        [SerializeField] UniqueID id;
        public string ID => id.value;

        protected virtual void OnEnable() => CreateUniqueID();

        protected virtual void OnValidate() => CreateUniqueID();

        void CreateUniqueID()
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                GenerateID();
            }

            if (!IsUnique(ID))
            {
                Logger.LogWarning($"A duplicate ID was found for {name}! A new ID will be generated for this object.");
                GenerateID();
            }

            bool IsUnique(string ID)
            {
                if (Application.isPlaying)
                    return true;

                return Resources.FindObjectsOfTypeAll<UniqueIDScriptableObject>().Count(x => x.ID == ID) == 1;
            }

            void GenerateID() => id.value = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
        private class UniqueIdDrawer : UnityEditor.PropertyDrawer
        {
            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                UnityEditor.EditorGUI.BeginProperty(position, label, property);

                position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                GUI.enabled = false;
                Rect valueRect = position;
                UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("value");
                UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);
                GUI.enabled = true;

                UnityEditor.EditorGUI.EndProperty();
            }
        }
#endif
    }

    [Serializable]
    public struct UniqueID
    {
        public string value;
    }
}