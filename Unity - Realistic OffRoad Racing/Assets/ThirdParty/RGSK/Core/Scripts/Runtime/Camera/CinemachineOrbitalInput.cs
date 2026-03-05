using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RGSK
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineOrbitalInput : MonoBehaviour
    {
        CinemachineOrbitalTransposer _orbitalTransposer;
        Vector2 _input;

        void OnEnable()
        {
            InputManager.CameraLookEvent += OnLook;
        }

        void OnDisable()
        {
            InputManager.CameraLookEvent -= OnLook;
        }

        void Awake()
        {
            _orbitalTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        void Update()
        {
            if (_orbitalTransposer == null)
                return;

            var angle = Mathf.Atan2(_input.x, -(_input.y - 0.01f)) * Mathf.Rad2Deg;
            _orbitalTransposer.m_XAxis.Value = -angle;
        }

        void OnLook(Vector2 value)
        {
            _input = value;
        }
    }
}