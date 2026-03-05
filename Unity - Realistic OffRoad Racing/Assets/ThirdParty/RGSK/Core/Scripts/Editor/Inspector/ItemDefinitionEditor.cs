using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : UnityEditor.Editor
    {
        protected ItemDefinition _target;

        SerializedProperty id;
        SerializedProperty objectName;
        SerializedProperty description;
        SerializedProperty icon;
        SerializedProperty previewPhoto;

        SerializedProperty unlockMode;
        SerializedProperty unlockPrice;
        SerializedProperty unlockXPLevel;
        SerializedProperty unlockCondition;

        protected virtual void OnEnable()
        {
            _target = (ItemDefinition)target;

            description = serializedObject.FindProperty(nameof(description));
            objectName = serializedObject.FindProperty(nameof(objectName));
            icon = serializedObject.FindProperty(nameof(icon));
            previewPhoto = serializedObject.FindProperty(nameof(previewPhoto));
            id = serializedObject.FindProperty(nameof(id));

            unlockMode = serializedObject.FindProperty(nameof(unlockMode));
            unlockPrice = serializedObject.FindProperty(nameof(unlockPrice));
            unlockXPLevel = serializedObject.FindProperty(nameof(unlockXPLevel));
            unlockCondition = serializedObject.FindProperty(nameof(unlockCondition));
        }

        protected virtual void DrawInfo()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(objectName);
                EditorGUILayout.PropertyField(description);
                EditorGUILayout.PropertyField(icon);
                EditorGUILayout.PropertyField(previewPhoto);
            }

            using (new GUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.PropertyField(unlockMode);

                EditorGUI.BeginDisabledGroup(_target.unlockMode != ItemUnlockMode.Purchase);
                {
                    EditorGUILayout.PropertyField(unlockPrice);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.BeginDisabledGroup(_target.unlockMode != ItemUnlockMode.XPLevel);
                {
                    EditorGUILayout.PropertyField(unlockXPLevel);
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUI.BeginDisabledGroup(_target.unlockMode != ItemUnlockMode.None);
                {
                    EditorGUILayout.PropertyField(unlockCondition);
                    EditorGUI.EndDisabledGroup();
                }
            }
        }
    }
}