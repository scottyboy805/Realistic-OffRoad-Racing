using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class CameraPerspectiveTarget : MonoBehaviour
    {
        [Tooltip("The perspectives that will target this transform.")]
        public List<CameraPerspective> perspectives = new List<CameraPerspective>();

        [Tooltip("Whether the follow offset should be applied to the transposer when this perspective is active.")]
        public bool applyFollowOffset;

        [Tooltip("The follow offset applied to the transposer when this perspective is active.")]
        public Vector3 followOffset;
    }
}