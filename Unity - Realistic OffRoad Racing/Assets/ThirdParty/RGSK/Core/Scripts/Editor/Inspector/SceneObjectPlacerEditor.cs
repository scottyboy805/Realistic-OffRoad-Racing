using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    [CustomEditor(typeof(SceneObjectPlacer))]
    public class SceneObjectPlacerEditor : SceneViewRaycaster
    {
        SceneObjectPlacer _target; 
        SerializedProperty prefab;
        SerializedProperty offset;

        void OnEnable()
        {
            _target = (SceneObjectPlacer)target;

            prefab = serializedObject.FindProperty(nameof(prefab));
            offset = serializedObject.FindProperty(nameof(offset));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Assign a prefab and use Shift + Left Mouse Button to place it in the scene.", MessageType.Info);

            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(offset);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnHit(Vector3 position, Vector3 normal)
        {
            if (_target.prefab == null)
                return;

            var obj = (GameObject)PrefabUtility.InstantiatePrefab(_target.prefab);
            obj.transform.SetPositionAndRotation(position + _target.offset, Quaternion.FromToRotation(Vector3.up, normal));
            obj.transform.SetParent(_target.transform, true);
            Undo.RegisterCreatedObjectUndo(obj.gameObject, "created_sceneobject");
        }
    }
}