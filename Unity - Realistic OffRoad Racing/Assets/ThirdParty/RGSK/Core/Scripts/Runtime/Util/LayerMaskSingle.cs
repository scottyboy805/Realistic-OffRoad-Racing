using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RGSK
{
    [System.Serializable]
    public class LayerMaskSingle
    {
        [SerializeField] int index = 0;

        public int Index
        {
            get
            {
                return index;
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerMaskSingle))]
    public class LayerMaskSingleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty index = property.FindPropertyRelative("index");

            if (index != null)
            {
                index.intValue = EditorGUI.LayerField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label, index.intValue);
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}