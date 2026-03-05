using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RGSK
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class RouteCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera VirtualCamera
        {
            get
            {
                return _camera ?? (_camera = GetComponent<CinemachineVirtualCamera>());
            }
        }

        public float routeDistance;
        CinemachineVirtualCamera _camera;
    }
}