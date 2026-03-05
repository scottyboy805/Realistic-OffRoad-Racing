using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    //NOTE: speed zones are now modified in the scene view GUI

    //[CustomPropertyDrawer(typeof(AISpeedZone))]
    public class AISpeedZoneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            var spacing = 10f;
            var variableWidth = (position.width - spacing) / 2;
            EditorGUIUtility.labelWidth = 60;

            var startDistanceRect = new Rect(position.x, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var endDistanceRect = new Rect(startDistanceRect.xMax + spacing, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var speedLimitRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);
            var boostProbabilityRect = new Rect(speedLimitRect.xMax + spacing, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);
            var btnRect = new Rect(speedLimitRect.xMax + spacing, position.y + EditorGUIUtility.singleLineHeight * 2.2f, variableWidth, EditorGUIUtility.singleLineHeight);

            var startDistanceProp = property.FindPropertyRelative("startDistance");
            var endDistanceProp = property.FindPropertyRelative("endDistance");
            var speedLimitProp = property.FindPropertyRelative("speedLimit");
            var boostProbabilityProp = property.FindPropertyRelative("boostProbability");

            EditorGUI.PropertyField(startDistanceRect, startDistanceProp, new GUIContent("Start"));
            EditorGUI.PropertyField(endDistanceRect, endDistanceProp, new GUIContent("End"));
            EditorGUI.PropertyField(speedLimitRect, speedLimitProp, new GUIContent("Limit"));
            EditorGUI.PropertyField(boostProbabilityRect, boostProbabilityProp, new GUIContent("Boost")); ;

            if (GUI.Button(btnRect, new GUIContent("Locate"), EditorStyles.miniButtonMid))
            {
                if (property.serializedObject.targetObject != null && SceneView.lastActiveSceneView.camera != null)
                {
                    AIRoute route = (AIRoute)property.serializedObject.targetObject;
                    SceneView.lastActiveSceneView.pivot = route.GetPositionAtDistance(startDistanceProp.floatValue);
                    SceneView.lastActiveSceneView.size = 4;
                    SceneView.lastActiveSceneView.Repaint();
                }
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3.2f;
        }
    }
}