using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class HandPoseSync : MonoBehaviour
    {
        [SerializeField] Transform reference;
        
        Transform[] _fingers;
        Transform[] _fingersReference;

        void Start()
        {
            if(reference == null)
                return;
            
            _fingers = GetComponentsInChildren<Transform>();
            _fingersReference = reference.GetComponentsInChildren<Transform>();
        }

        void LateUpdate()
        {  
            for (int i = 1; i < _fingers.Length; i++)
            {
                _fingers[i].localRotation = _fingersReference[i].localRotation;
            }
        }
    }
}
