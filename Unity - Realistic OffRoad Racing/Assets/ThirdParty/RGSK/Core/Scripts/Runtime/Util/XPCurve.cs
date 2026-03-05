using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using RGSK.Extensions;
#endif

namespace RGSK
{
    [System.Serializable]
    public class XPCurve
    {
        public AnimationCurve curve = new AnimationCurve(new Keyframe(1, 0), new Keyframe(100, 100000));
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(XPCurve))]
    public class XPCurvePropertyDrawer : PropertyDrawer
    {
        Vector2 _pos;
        bool _expand;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var spacing = 10f;
            var variableWidth = (position.width - spacing) / 4f;

            var curveRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var curveProp = property.FindPropertyRelative("curve");
            EditorGUI.PropertyField(curveRect, curveProp, new GUIContent("XP Curve"));

            _expand = EditorGUI.Foldout(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), _expand, "Show XP Levels");

            var animCurve = (AnimationCurve)curveProp.animationCurveValue;
            var scrollHeight = EditorGUIUtility.singleLineHeight * animCurve.GetMaxTime() + 1;
            var scrollRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2f, position.width, EditorGUIUtility.singleLineHeight * 10);

            if (_expand)
            {
                _pos = GUI.BeginScrollView(scrollRect, _pos, new Rect(0, 0, position.width / 2, scrollHeight), false, false);
                {
                    for (int i = 1; i < scrollHeight; i++)
                    {
                        var rect1 = new Rect(position.x, EditorGUIUtility.singleLineHeight * (i - 1), variableWidth, EditorGUIUtility.singleLineHeight);
                        var rect2 = new Rect(rect1.xMax + spacing, rect1.y, variableWidth, EditorGUIUtility.singleLineHeight);
                        var level = i;
                        var points = (int)animCurve.Evaluate(i);

                        EditorGUI.LabelField(rect1, $"Level: {level}");
                        EditorGUI.LabelField(rect2, $"XP: {points.ToString("N0")}");
                    }
                    GUI.EndScrollView(true);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _expand ? EditorGUIUtility.singleLineHeight * 12 : EditorGUIUtility.singleLineHeight * 2f;
        }
    }
#endif
}