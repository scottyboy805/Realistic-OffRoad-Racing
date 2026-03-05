using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RGSK
{
    public class BillboardObject : MonoBehaviour
    {
        [SerializeField] bool ignoreYAxis;
        CinemachineBrain _camera;

        void Start()
        {
            _camera = CameraManager.Instance?.Camera;
        }

        void Update()
        {
            UpdateRotation();
        }

        void UpdateRotation()
        {
            if (_camera == null)
                return;

            if (_camera.transform.InverseTransformPoint(transform.position).z >= 0)
            {
                var dir = _camera.transform.position - transform.position;

                if (ignoreYAxis)
                {
                    dir.y = 0;
                }

                if (dir != Vector3.zero)
                {
                    var rot = Quaternion.LookRotation(-dir);
                    transform.rotation = rot;
                }
            }
        }
    }
}