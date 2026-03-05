using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class CenterOfMassAssigner : MonoBehaviour
    {
        Rigidbody _rigid;

        void Start()
        {
            _rigid = GetComponentInParent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (_rigid == null)
                return;

            var com = _rigid.transform.InverseTransformPoint(transform.position);

            if (_rigid.centerOfMass != com)
            {
                _rigid.centerOfMass = com;
            }
        }
    }
}