using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace RGSK.Editor
{
    [EditorTool("Racing Line Tool", typeof(AIRoute))]
    public class AIRacingLineTool : EditorTool
    {
        AIRoute _route;
        GUIContent _iconContent;

        public override GUIContent toolbarIcon => _iconContent;

        void OnEnable()
        {
            ToolManager.activeToolChanged += ActiveToolChanged;

            _iconContent = new GUIContent()
            {
                image = RGSKEditorSettings.Instance.aiRouteRacinglineToolIcon,
                tooltip = "Racing Line Tool"
            };
        }

        void OnDisable()
        {
            ToolManager.activeToolChanged -= ActiveToolChanged;

            if (_route != null)
            {
                _route.toolMode = AIRouteToolMode.None;
            }
        }

        void ActiveToolChanged()
        {
            if (_route != null)
            {
                _route.toolMode = AIRouteToolMode.None;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView))
                return;

            foreach (var t in targets)
            {
                if (!(t is AIRoute route))
                    continue;

                _route = route;
                _route.toolMode = AIRouteToolMode.RacingLine;
            }
        }
    }

    [EditorTool("Speed Zone Tool", typeof(AIRoute))]
    public class AISpeedZoneTool : EditorTool
    {
        AIRoute _route;
        GUIContent _iconContent;

        public override GUIContent toolbarIcon => _iconContent;

        void OnEnable()
        {
            ToolManager.activeToolChanged += ActiveToolChanged;

            _iconContent = new GUIContent()
            {
                image = RGSKEditorSettings.Instance.aiRouteSpeedzoneToolIcon,
                tooltip = "Speed Zone Tool"
            };
        }

        void OnDisable()
        {
            ToolManager.activeToolChanged -= ActiveToolChanged;

            if (_route != null)
            {
                _route.toolMode = AIRouteToolMode.None;
            }
        }

        void ActiveToolChanged()
        {
            if (_route != null)
            {
                _route.toolMode = AIRouteToolMode.None;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!(window is SceneView))
                return;

            foreach (var t in targets)
            {
                if (!(t is AIRoute route))
                    continue;

                _route = route;
                _route.toolMode = AIRouteToolMode.SpeedZones;
            }
        }
    }
}