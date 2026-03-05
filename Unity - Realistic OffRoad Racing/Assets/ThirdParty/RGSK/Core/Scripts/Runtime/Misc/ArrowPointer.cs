using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class ArrowPointer : MonoBehaviour
    {
        [SerializeField] Transform arrow;
        [SerializeField] Transform target;
        [SerializeField] bool ignoreYAxis = true;

        void Update()
        {
            if (arrow == null || target == null)
                return;

            var dir = target.position - arrow.position;

            if (ignoreYAxis)
            {
                dir.y = 0;
            }

            arrow.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void ToggleVisible(bool toggle)
        {
            arrow.gameObject.SetActive(toggle);
        }
    }
}