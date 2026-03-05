using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;

namespace RGSK
{
    public class AINode : RouteNode
    {
        [Range(-1, 1)][SerializeField] float racingLineOffset = 0;
        public List<AIRoute> branchRoutes = new List<AIRoute>();

        public float RacingLineOffset
        {
            get => Mathf.Lerp(-leftWidth, rightWidth, (racingLineOffset + 1) / 2);
            set => racingLineOffset = Mathf.InverseLerp(-leftWidth, rightWidth, value) * 2 - 1;
        }

        public float RawRacingLineOffset
        {
            get => racingLineOffset;
            set => racingLineOffset = value;
        }

        void Awake()
        {
            branchRoutes.RemoveNullElements();
        }

        protected override void DrawGizmos()
        {
            base.DrawGizmos();
            
            if (branchRoutes.Count > 0)
            {
                foreach (var branch in branchRoutes)
                {
                    if (branch != null)
                    {
                        var closest = branch.GetClosestNode(transform.position);

                        if (closest != null)
                        {
                            Gizmos.DrawLine(transform.position, closest.transform.position);
                        }
                    }
                }
            }
        }
    }
}