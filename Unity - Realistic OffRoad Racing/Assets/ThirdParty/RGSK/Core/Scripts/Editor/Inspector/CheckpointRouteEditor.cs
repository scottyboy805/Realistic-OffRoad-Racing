using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(CheckpointRoute))]
    public class CheckpointRouteEditor : RouteEditor
    {
        protected override void DrawNodeHandles()
        {
            base.DrawNodeHandles();

            if (_target.showNodeInfo)
            {
                for (int i = 0; i < _target.nodes.Count; ++i)
                {
                    var node = (CheckpointNode)_target.nodes[i];
                    var pos = node.transform.position;
                    var percent = $"\n{UIHelper.FormatDistanceText(node.normalizedDistance * _target.Distance)}({(node.normalizedDistance * 100).ToString("F1")}%)";
                    var startfinish = "";
                    var type = "";

                    if (_target.loop && i == 0)
                    {
                        startfinish = "\n[Start/Finish]";
                    }

                    if (!_target.loop)
                    {
                        if (i == 0)
                        {
                            startfinish += "\n[Start]";
                        }
                        else if (i == _target.nodes.Count - 1)
                        {
                            startfinish += "\n[Finish]";
                        }
                    }

                    if (node.IsSector)
                    {
                        type += "\n[Sector]";
                    }

                    if (node.IsSpeedtrap)
                    {
                        type += "\n[Speedtrap]";
                    }

                    if (node.IsTimeExtend)
                    {
                        type += "\n[Time Extend]";
                    }

                    EditorHelper.DrawLabelWithinDistance(pos + Vector3.up * 3, $"{i.ToString()}{percent}{startfinish}{type}", CustomEditorStyles.NodeLabel);
                }
            }
        }

        public override RouteNode CreateNode(GameObject node = null)
        {
            var n = node != null ? node.GetOrAddComponent<CheckpointNode>() : new GameObject("CheckpointNode").AddComponent<CheckpointNode>();
            n.route = _target;
            return n;
        }
    }
}