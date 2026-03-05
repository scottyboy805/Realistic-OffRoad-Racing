using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class RouteNodeData : ScriptableObject
    {
        [Serializable]
        public class NodeData
        {
            public Vector3 position;
            public Quaternion rotation;
            public float leftWidth;
            public float rightWidth;
        }

        public List<NodeData> nodeData = new List<NodeData>();
    }
}