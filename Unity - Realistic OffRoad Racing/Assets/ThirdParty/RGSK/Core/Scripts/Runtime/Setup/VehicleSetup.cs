using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class VehicleSetup : MonoBehaviour
    {
        public MeshFilter colliderBody;
        public List<Transform> frontAxleWheels = new List<Transform>();
        public List<Transform> rearAxleWheels = new List<Transform>();
        public List<CameraPerspective> cameraPerspectives = new List<CameraPerspective>();
    }
}