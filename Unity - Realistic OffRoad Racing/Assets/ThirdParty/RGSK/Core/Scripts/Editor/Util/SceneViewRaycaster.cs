using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    public class SceneViewRaycaster : UnityEditor.Editor
    {
        public LayerMask placeableLayers = ~0;
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        public virtual void OnSceneGUI()
        {
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (e.shift && e.button == 0)
                {
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);

                    var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    
                    if (Physics.Raycast(ray, out var hit, 10000, placeableLayers, triggerInteraction))
                    {
                        OnHit(hit.point, hit.normal);
                    }
                    else
                    {
                        Logger.LogWarning("The surface you are trying to place on does not have a collider! Please add a collider to it and try again.");
                    }

                    e.Use();
                }
            }
        }

        public virtual void OnHit(Vector3 position, Vector3 normal) { }
    }
}