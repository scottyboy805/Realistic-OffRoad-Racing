using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class TransformFollower : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, 0);
        public bool updateRotation = true;

        void Update()
        {
            if (target == null)
                return;

            var pos = target.position;
            pos += offset;
            transform.position = pos;

            if(updateRotation)
            {
                var rot = transform.eulerAngles;
                rot.y = target.eulerAngles.y;
                transform.eulerAngles = rot;           
            }
        }
    }
}