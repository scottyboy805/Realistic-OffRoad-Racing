using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomPropertyDrawer(typeof(RaceStateAIBehaviourComposite))]
    public class AIRaceStateBehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            var spacing = 10f;
            var variableWidth = (position.width - spacing) / 2f;
            EditorGUIUtility.labelWidth = 70;

            var rect1 = new Rect(position.x, position.y, variableWidth, position.height);
            var rect2 = new Rect(rect1.xMax + spacing, position.y, variableWidth, position.height);

            var prop1 = property.FindPropertyRelative("state");
            var prop2 = property.FindPropertyRelative("behaviour");

            EditorGUI.PropertyField(rect1, prop1);
            EditorGUI.PropertyField(rect2, prop2);

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.EndProperty();
        }
    }
}