using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using UnityEngine.Serialization;
using System.Linq;

namespace RGSK
{
    public class AIRoute : Route
    {
        public List<AISpeedZone> speedZones = new List<AISpeedZone>();

        public float speedLimit = -1;
        public float priority = 10;
        [Range(0, 1)] public float branchProbability = 0.5f;

        [Tooltip("How far ahead the AI's follow target will be projected (in meters) compared to its speed.")]
        public AnimationCurve lookAheadCurve = AnimationCurveExtensions.CreateLinearCurve(new Keyframe[]
        {
            new Keyframe(0, 5),
            new Keyframe(100, 20),
            new Keyframe(150, 20),
            new Keyframe(300, 50)
        });

        [Tooltip("How close the AI need to be (in meters) to pass a node.")]
        public float nodeReachedDistance = 15;

        public float cornerSampleDistance = 25f;
        public float minCornerAngle = 5f;
        public float minCornerDistance = 10f;
        public float minStraightDistance = 100f;
        public Vector2 cornerSpeedRange = new Vector2(80, 160);
        public Vector2 cornerBrakeTimeRange = new Vector2(0.25f, 1f);

        public AIRouteToolMode toolMode;
        public bool showRacingLineGizmos = true;
        public bool showSpeedZoneGizmos = true;

        public override void Reverse()
        {
            base.Reverse();
            UpdateNodeRotation();

            foreach (var node in nodes)
            {
                var aiNode = (AINode)node;
                aiNode.RacingLineOffset *= -1;
            }
        }

        protected override void DrawRoute()
        {
            if (showRoute)
            {
                DrawWidth();
            }

            if (showRacingLineGizmos)
            {
                DrawRacingLine();
            }
        }

        void DrawRacingLine()
        {
            Gizmos.color = RGSKEditorSettings.Instance.racingLineColor;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (i < nodes.Count - 1)
                {
                    var current = (AINode)nodes[i];
                    var next = (AINode)nodes[i + 1];

                    Gizmos.DrawLine(current.transform.position + (current.transform.right * current.RacingLineOffset),
                                    next.transform.position + (next.transform.right * next.RacingLineOffset));
                }
            }

            if (loop && nodes.Count > 2)
            {
                var last = (AINode)nodes[nodes.Count - 1];
                var first = (AINode)nodes[0];

                Gizmos.DrawLine(last.transform.position + (last.transform.right * last.RacingLineOffset),
                                first.transform.position + (first.transform.right * first.RacingLineOffset));
            }
        }

        void DrawWidth()
        {
            Gizmos.color = gizmoColor;

            if (nodes.Count < 1)
                return;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Gizmos.DrawLine(nodes[i].transform.position + (nodes[i].transform.right * nodes[i].rightWidth),
                    nodes[i + 1].transform.position + (nodes[i + 1].transform.right * nodes[i + 1].rightWidth));

                Gizmos.DrawLine(nodes[i].transform.position + (-nodes[i].transform.right * nodes[i].leftWidth),
                   nodes[i + 1].transform.position + (-nodes[i + 1].transform.right * nodes[i + 1].leftWidth));
            }

            if (loop)
            {
                Gizmos.DrawLine(nodes[nodes.Count - 1].transform.position + (nodes[nodes.Count - 1].transform.right * nodes[nodes.Count - 1].rightWidth),
                nodes[0].transform.position + (nodes[0].transform.right * nodes[0].rightWidth));

                Gizmos.DrawLine(nodes[nodes.Count - 1].transform.position + (-nodes[nodes.Count - 1].transform.right * nodes[nodes.Count - 1].leftWidth),
                nodes[0].transform.position + (-nodes[0].transform.right * nodes[0].leftWidth));
            }
        }

        public void SmoothRacingLine(int smoothness = 5, float smoothThreshold = 0.1f, float smoothAmount = 0.5f)
        {
            for (int i = 0; i < smoothness; i++)
            {
                for (int j = 1; j < nodes.Count - 1; j++)
                {
                    var prev = (AINode)nodes[j - 1];
                    var curr = (AINode)nodes[j];
                    var next = (AINode)nodes[j + 1];

                    if (Mathf.Abs(curr.RawRacingLineOffset - prev.RawRacingLineOffset) > smoothThreshold)
                    {
                        prev.RawRacingLineOffset = Mathf.Lerp(prev.RawRacingLineOffset, curr.RawRacingLineOffset, smoothAmount);
                    }

                    if (Mathf.Abs(curr.RawRacingLineOffset - next.RawRacingLineOffset) > smoothThreshold)
                    {
                        next.RawRacingLineOffset = Mathf.Lerp(next.RawRacingLineOffset, curr.RawRacingLineOffset, smoothAmount);
                    }
                }
            }
        }

        public void ResetRacingLine()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = (AINode)nodes[i];
                node.RawRacingLineOffset = 0;
            }
        }

        public void CreateSpeedZones()
        {
            var corners = CreateCorners();
            speedZones.Clear();
            AISpeedZone prevSpeedZone = null;
            Corner prevCorner = null;

            foreach (var corner in corners)
            {
                var zone = new AISpeedZone
                {
                    corner = corner,
                    startDistance = corner.sharpness < 0.1f ? corner.startDistance : corner.entryDistance,
                    endDistance = corner.sharpness < 0.1f ? corner.endDistance : corner.exitDistance
                };

                speedZones.Add(zone);
                prevSpeedZone = zone;
                prevCorner = corner;
            }

            UpdateSpeedZones();
        }

        public void UpdateSpeedZones()
        {
            foreach (var zone in speedZones)
            {
                zone.Update(this);
            }

            SortSpeedzones();
        }

        public void SortSpeedzones()
        {
            speedZones = speedZones.OrderBy(x => x.startDistance).ToList();

            for (int i = 0; i < speedZones.Count - 1; i++)
            {
                var current = speedZones[i];
                var next = speedZones[i + 1];

                if (current.endDistance > next.startDistance)
                {
                    current.endDistance = next.startDistance;
                }
            }
        }

        public List<Corner> CreateCorners()
        {
            var corners = new List<Corner>();
            var inCorner = false;
            var cornerStart = 0f;
            var cornerDistance = 0f;
            var lastCornerEnd = 0f;

            for (float i = cornerSampleDistance * 2f; i <= Distance; i += cornerSampleDistance)
            {
                var angle = Corner.GetAngle
                            (GetPositionAtDistance(i - cornerSampleDistance * 2f),
                             GetPositionAtDistance(i - cornerSampleDistance),
                             GetPositionAtDistance(i));

                if (angle > minCornerAngle)
                {
                    if (!inCorner)
                    {
                        var straightStart = lastCornerEnd;
                        var straightEnd = i - cornerSampleDistance * 2f;
                        var straightLength = straightEnd - straightStart;

                        if (straightLength >= minStraightDistance)
                        {
                            var corner = new Corner
                            {
                                startDistance = straightStart,
                                endDistance = straightEnd,
                            };

                            corner.Update(this);
                            corners.Add(corner);
                        }

                        cornerStart = straightEnd;
                        inCorner = true;
                    }
                }
                else
                {
                    if (inCorner)
                    {
                        cornerDistance += cornerSampleDistance;

                        var cornerEnd = i - cornerDistance;

                        var start = GetPositionAtDistance(cornerStart);
                        var end = GetPositionAtDistance(cornerEnd);
                        var cornerLength = cornerEnd - cornerStart;

                        if (cornerLength >= minCornerDistance)
                        {
                            var corner = new Corner
                            {
                                startDistance = cornerStart,
                                endDistance = cornerEnd,
                            };

                            corner.Update(this);

                            if (corner.direction != Corner.CornerDirection.Straight)
                            {
                                corners.Add(corner);
                            }
                        }

                        lastCornerEnd = cornerEnd;
                        cornerDistance = 0f;
                        inCorner = false;
                    }
                }
            }

            if (inCorner)
            {
                var corner = new Corner
                {
                    startDistance = cornerStart,
                    endDistance = Distance,
                };

                corner.Update(this);
                corners.Add(corner);

                lastCornerEnd = Distance;
            }

            if ((Distance - minStraightDistance) > lastCornerEnd)
            {
                var corner = new Corner
                {
                    startDistance = lastCornerEnd,
                    endDistance = Distance,
                };

                corner.Update(this);
                corners.Add(corner);
            }

            return corners;
        }

        public (AISpeedZone, AISpeedZone) GetClosestSpeedZones(float distance)
        {
            AISpeedZone current = null;
            AISpeedZone next = null;

            foreach (var zone in speedZones)
            {
                if (distance.IsBetween(zone.startDistance, zone.endDistance))
                {
                    current = zone;
                }

                if (zone.startDistance > distance)
                {
                    next = zone;
                    break;
                }

                if (loop && speedZones.Count > 0)
                {
                    next = speedZones[0];
                }
            }

            return (current, next);
        }
    }

    [System.Serializable]
    public class AISpeedZone
    {
        public float startDistance;
        public float endDistance;
        public float speedLimit = -1;

        [Tooltip("How many seconds before reaching the speed zone that the AI should begin braking to match the \"Speed Limit\". Leave at 0 to ignore braking.")]
        [Range(0, 5)] public float brakeTime = 0f;
        [Range(0, 1)] public float boostProbability = 0f;

        [HideInInspector] public Corner corner;

        public void Update(AIRoute route)
        {
            if (corner == null || corner.startDistance == 0 && corner.endDistance == 0)
            {
                corner = new Corner
                {
                    startDistance = startDistance,
                    endDistance = endDistance,
                };

                corner.Update(route);
            }

            if (corner.direction == Corner.CornerDirection.Straight)
            {
                speedLimit = -1;
                brakeTime = 0;
                boostProbability = Random.Range(0.5f, 1f);
            }
            else
            {
                speedLimit = Mathf.Lerp(route.cornerSpeedRange.y, route.cornerSpeedRange.x, corner.sharpness);
                brakeTime = Mathf.Lerp(route.cornerBrakeTimeRange.x, route.cornerBrakeTimeRange.y, corner.sharpness);
                boostProbability = 0;
            }
        }
    }

    [System.Serializable]
    public class Corner
    {
        public enum CornerDirection
        {
            Straight,
            Left,
            Right,
        }

        public CornerDirection direction;
        public float sharpness;
        public float startDistance;
        public float endDistance;
        public float entryDistance;
        public float apexDistance;
        public float exitDistance;

        public float Length => endDistance - startDistance;

        public void Update(AIRoute route)
        {
            var start = route.GetPositionAtDistance(startDistance);
            var mid = route.GetPositionAtDistance((startDistance + endDistance) * 0.5f);
            var end = route.GetPositionAtDistance(endDistance);
            var angle = GetAngle(start, mid, end);

            entryDistance = (startDistance + (Length * 0.25f));
            apexDistance = (startDistance + (Length * 0.5f));
            exitDistance = (startDistance + (Length * 0.75f));

            sharpness = Mathf.InverseLerp(route.minCornerAngle, 60, angle);
            direction = GetDirection(route.GetClosestNode(start), start, end);
        }

        public CornerDirection GetDirection(RouteNode node, Vector3 start, Vector3 end)
        {
            if (node == null || sharpness <= 0.1f)
                return CornerDirection.Straight;

            var dir = end - node.transform.position;
            var dot = Vector3.Dot(node.transform.right, dir);

            return dot > 0 ? CornerDirection.Right : CornerDirection.Left;
        }

        public static float GetAngle(Vector3 start, Vector3 middle, Vector3 end)
        {
            var dir1 = GetFlatDirection(start, middle);
            var dir2 = GetFlatDirection(middle, end);

            return Vector3.Angle(dir1, dir2);
        }

        public static Vector3 GetFlatDirection(Vector3 from, Vector3 to)
        {
            var dir = to - from;
            dir.y = 0f;
            return dir.normalized;
        }
    }
}