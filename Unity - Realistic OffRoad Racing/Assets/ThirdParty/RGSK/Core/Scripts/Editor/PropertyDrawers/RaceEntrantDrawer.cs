using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomPropertyDrawer(typeof(RaceEntrant))]
    public class RaceEntrantDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            var spacing = 10f;
            var variableWidth = (position.width - spacing) / 3;
            EditorGUIUtility.labelWidth = 60;

            var prefabRect = new Rect(position.x, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var profileRect = new Rect(prefabRect.xMax + spacing, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var playerRect = new Rect(profileRect.xMax + spacing, position.y, variableWidth, EditorGUIUtility.singleLineHeight);
            var colorModeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);
            var colorRect = new Rect(colorModeRect.xMax + spacing, position.y + EditorGUIUtility.singleLineHeight * 1.1f, variableWidth, EditorGUIUtility.singleLineHeight);

            var prefabProp = property.FindPropertyRelative("prefab");
            var profileProp = property.FindPropertyRelative("profile");
            var playerProp = property.FindPropertyRelative("isPlayer");
            var colorModeProp = property.FindPropertyRelative("colorSelectMode");
            var colorProp = property.FindPropertyRelative("color");
            var liveryProp = property.FindPropertyRelative("livery");

            EditorGUI.PropertyField(prefabRect, prefabProp);
            EditorGUI.PropertyField(profileRect, profileProp);
            EditorGUI.PropertyField(playerRect, playerProp);
            EditorGUI.PropertyField(colorModeRect, colorModeProp, new GUIContent("Color"));

            if (colorModeProp.enumValueIndex != 0)
            {
                EditorGUI.PropertyField(colorRect, colorModeProp.enumValueIndex == 1 ? colorProp : liveryProp, new GUIContent(""));
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2.2f;
        }
    }

    [CustomPropertyDrawer(typeof(AutoPopulateEntrantOptions))]
    public class AutoPopulateEntrantOptionsDrawer : PropertyDrawer
    {
        static bool expanded;
        float maxHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var height = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            expanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, height), expanded, new GUIContent("Auto Populate Entrant Options"));

            if (expanded)
            {
                EditorGUI.indentLevel++;

                var autoPopulatePlayerRect = new Rect(position.x, position.y + (height + spacing) * 1, position.width, height);
                var autoPopulateOpponentsRect = new Rect(position.x, position.y + (height + spacing) * 2, position.width, height);
                var opponentCountRect = new Rect(position.x, position.y + (height + spacing) * 3, position.width, height);
                var opponentsClassOptionsRect = new Rect(position.x, position.y + (height + spacing) * 4, position.width, height);
                var opponentVehicleClassRect = new Rect(position.x, position.y + (height + spacing) * 5, position.width, height);

                var autoPopulatePlayerProp = property.FindPropertyRelative("autoPopulatePlayer");
                var autoPopulateOpponentsProp = property.FindPropertyRelative("autoPopulateOpponents");
                var opponentCountProp = property.FindPropertyRelative("opponentCount");
                var opponentsClassOptionsProp = property.FindPropertyRelative("opponentClassOptions");
                var opponentVehicleClassProp = property.FindPropertyRelative("opponentVehicleClass");

                EditorGUI.PropertyField(autoPopulatePlayerRect, autoPopulatePlayerProp);
                EditorGUI.PropertyField(autoPopulateOpponentsRect, autoPopulateOpponentsProp);

                EditorGUI.BeginDisabledGroup(!autoPopulateOpponentsProp.boolValue);
                {
                    EditorGUI.PropertyField(opponentCountRect, opponentCountProp);
                    EditorGUI.PropertyField(opponentsClassOptionsRect, opponentsClassOptionsProp);

                    EditorGUI.BeginDisabledGroup(opponentsClassOptionsProp.enumValueIndex != 1);
                    {
                        EditorGUI.PropertyField(opponentVehicleClassRect, opponentVehicleClassProp);
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            maxHeight = expanded ? 6 : 1;
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (maxHeight);
        }
    }

    [CustomPropertyDrawer(typeof(OpponentDifficultyIndex))]
    public class OpponentDifficultyIndexDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var difficultyList = new List<string>();
            RGSKCore.Instance.AISettings.difficulties.ForEach(x => difficultyList.Add(string.IsNullOrWhiteSpace(x.displayName) ? x.name : x.displayName));

            var indexProp = property.FindPropertyRelative("index");

            if (indexProp != null)
            {
                indexProp.intValue = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label.text, indexProp.intValue, difficultyList.ToArray());
            }

            EditorGUI.EndProperty();
        }
    }
}