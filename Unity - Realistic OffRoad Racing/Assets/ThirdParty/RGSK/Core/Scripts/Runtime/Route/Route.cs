using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class Route : MonoBehaviour
    {
        public List<RouteNode> nodes = new List<RouteNode>();
        public bool loop = false;
        public float defaultNodeWidth = 20f;
        public bool showRoute = true;
        public bool showNodeInfo = true;
        public Color gizmoColor = Color.yellow;
        public RouteNodeData routeNodeData;

        public float Distance => _distance;

        Vector3[] _points;
        float[] _distances;
        float _distance;

        void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            if (nodes.Count < 2)
            {
                _distance = 0f;
                return;
            }

            _points = loop ? new Vector3[nodes.Count + 1] : new Vector3[nodes.Count];
            _distances = loop ? _distances = new float[nodes.Count + 1] : new float[nodes.Count];

            var accumulateDistance = 0f;
            for (int i = 0; i < _points.Length; ++i)
            {
                var t1 = nodes[i % nodes.Count];
                var t2 = nodes[(i + 1) % nodes.Count];
                if (t1 != null && t2 != null)
                {
                    Vector3 p1 = t1.transform.position;
                    Vector3 p2 = t2.transform.position;
                    _points[i] = nodes[i % nodes.Count].transform.position;
                    _distances[i] = accumulateDistance;
                    accumulateDistance += (p1 - p2).magnitude;
                }
            }

            var distance = 0f;
            for (var i = 1; i < nodes.Count; i++)
            {
                distance += Vector3.Distance(nodes[i - 1].transform.position, nodes[i].transform.position);
            }

            if (loop)
            {
                distance += Vector3.Distance(nodes[0].transform.position, nodes[nodes.Count - 1].transform.position);
            }
            else
            {
                nodes[0].distance = nodes[0].normalizedDistance = 0;
            }

            var percentage = 0f;
            for (var i = 1; i < nodes.Count; i++)
            {
                var segment = Vector3.Distance(nodes[i - 1].transform.position, nodes[i].transform.position);
                percentage += segment / distance;
                nodes[i].normalizedDistance = percentage;
                nodes[i].distance = percentage * _distance;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].route = this;
                nodes[i].index = i;
                nodes[i].nextNode = GetNextNode(nodes[i]);
                nodes[i].previousNode = GetPreviousNode(nodes[i]);
                nodes[0].previousNode = loop ? nodes.LastOrDefault() : null;

                if (!loop)
                {
                    nodes.LastOrDefault().nextNode = null;
                }
            }

            _distance = distance;
        }

        public virtual void Reverse()
        {
            if (nodes.Count < 2)
                return;

            nodes.Reverse();

            if (loop)
            {
                nodes.Insert(0, nodes[nodes.Count - 1]);
                nodes.RemoveAt(nodes.Count - 1);
            }

            foreach (var n in nodes)
            {
                n.transform.SetSiblingIndex(nodes.IndexOf(n));
            }

            Setup();
        }

        public void UpdateNodeRotation()
        {
            if (nodes.Count < 2)
                return;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                SetRotation(nodes[i].transform, nodes[i + 1].transform);
            }

            if (loop)
            {
                SetRotation(nodes[nodes.Count - 1].transform, nodes[0].transform);
            }
            else
            {
                SetRotation(nodes[nodes.Count - 1].transform, nodes[nodes.Count - 2].transform);
                nodes[nodes.Count - 1].transform.Rotate(0, 180, 0);
            }

            void SetRotation(Transform curr, Transform next)
            {
                var dir = (next.position - curr.position).normalized;
                var proj = Vector3.ProjectOnPlane(dir, curr.transform.up).normalized;

                if (proj != Vector3.zero)
                {
                    curr.transform.rotation = Quaternion.LookRotation(proj, curr.transform.up);
                }
            }
        }

        public void UpdateNodeWidth()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].combineWidth = true;
                nodes[i].combinedWidth = defaultNodeWidth;
            }
        }

        public float GetPercentageAtPosition(Vector3 point, RouteNode closest = null)
        {
            var closestPoint = GetClosestPointOnPath(point, out Tuple<RouteNode, RouteNode> nodes, closest);
            var segmentPercentage = (loop ? 1f : nodes.Item2.normalizedDistance) - nodes.Item1.normalizedDistance;
            var segmentDistance = _distance * segmentPercentage;
            var positionSegmentPercentage = Vector3.Distance(closestPoint, nodes.Item1.transform.position) / segmentDistance;
            return Mathf.Lerp(nodes.Item1.normalizedDistance, (loop ? 1f : nodes.Item2.normalizedDistance), positionSegmentPercentage);
        }

        public Vector3 GetPositionAtPercentage(float percent)
        {
            if (_distances == null)
                return Vector3.zero;

            percent = Mathf.Clamp(percent, 0, 1);
            var point = 0;

            while ((_distances[point] / _distance) < percent)
            {
                point++;
            }

            var p1 = loop ? ((point - 1) + nodes.Count) % nodes.Count :
                     point - 1 >= 0 ? point - 1 :
                     point;

            return Vector3.Lerp(_points[p1], _points[point],
                                Mathf.InverseLerp(_distances[p1] / _distance,
                                _distances[point] / _distance,
                                percent));
        }

        public Vector3 GetClosestPointOnPath(Vector3 point, out Tuple<RouteNode, RouteNode> nodeIndices, RouteNode closest = null)
        {
            if (closest == null)
            {
                closest = GetClosestNode(point);
            }

            var next = closest.nextNode != null ? closest.nextNode : closest;
            var previous = closest.previousNode != null ? closest.previousNode : closest;

            var nextLine = FindNearestPointOnLine(closest.transform.position, next.transform.position, point);
            var prevLine = FindNearestPointOnLine(closest.transform.position, previous.transform.position, point);

            var nextIsClosest = Vector3.Distance(point, nextLine) < Vector3.Distance(point, prevLine);

            nodeIndices = new Tuple<RouteNode, RouteNode>(nextIsClosest ? closest : previous, nextIsClosest ? next : closest);

            return nextIsClosest ? nextLine : prevLine;
        }

        Vector3 FindNearestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
        {
            var line = (end - start);
            var len = line.magnitude;
            line.Normalize();

            var v = point - start;
            var d = Vector3.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return start + line * d;
        }

        public RouteNode GetNode(int index)
        {
            if (nodes.Count == 0)
                return null;

            if (loop)
            {
                return nodes[(int)Mathf.Repeat(index, nodes.Count)];
            }
            else
            {
                if (index < 0 || index >= nodes.Count)
                    return null;

                return nodes[index];
            }
        }

        public RouteNode GetNextNode(RouteNode node)
        {
            return GetNode(nodes.IndexOf(node) + 1);
        }

        public RouteNode GetPreviousNode(RouteNode node)
        {
            return GetNode(nodes.IndexOf(node) - 1);
        }

        public RouteNode GetFirstNode() => GetNode(loop ? 1 : 0);

        public RouteNode GetLastNode() => GetNode(loop ? 0 : nodes.Count - 1);

        public RouteNode GetClosestNode(Vector3 point)
        {
            if (nodes.Count == 0)
                return null;

            var closestDistanceSqr = Mathf.Infinity;
            RouteNode result = null;

            foreach (var n in nodes)
            {
                var distance = (n.transform.position - point).sqrMagnitude;

                if (distance < closestDistanceSqr)
                {
                    result = n;
                    closestDistanceSqr = distance;
                }
            }

            return result;
        }

        public float GetDistanceAtPosition(Vector3 point)
        {
            return GetPercentageAtPosition(point) * _distance;
        }

        public Vector3 GetPositionAtDistance(float distance)
        {
            return GetPositionAtPercentage(distance / _distance);
        }

        void OnDrawGizmos()
        {
            Setup();
            DrawRoute();
        }

        protected virtual void DrawRoute()
        {
            if (showRoute)
            {
                Gizmos.color = gizmoColor;

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i < nodes.Count - 1)
                    {
                        Gizmos.DrawLine(nodes[i].transform.position, nodes[i + 1].transform.position);
                    }
                }

                if (loop && nodes.Count > 2)
                {
                    Gizmos.DrawLine(nodes[nodes.Count - 1].transform.position, nodes[0].transform.position);
                }
            }
        }
    }
}