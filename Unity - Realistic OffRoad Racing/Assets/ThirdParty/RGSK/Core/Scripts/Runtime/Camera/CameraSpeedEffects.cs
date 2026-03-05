using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RGSK.Extensions;

namespace RGSK
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraSpeedEffects : MonoBehaviour
    {
        [SerializeField] AnimationCurve fovCurve = AnimationCurve.Linear(0, 55, 200, 70);
        [SerializeField] AnimationCurve amplitudeGainCurve = AnimationCurve.Linear(0, 0, 200, 0.25f);
        [SerializeField] AnimationCurve frequencyGainCurve = AnimationCurve.Linear(0, 0, 200, 0.25f);

        CinemachineVirtualCamera _camera;
        CinemachineBasicMultiChannelPerlin _noise;
        Rigidbody _rigid;

        void OnEnable()
        {
            RGSKEvents.OnCameraTargetChanged.AddListener(OnCameraTargetChanged);
        }

        void OnDisable()
        {
            RGSKEvents.OnCameraTargetChanged.RemoveListener(OnCameraTargetChanged);
        }

        void OnCameraTargetChanged(Transform t)
        {
            _rigid = t.GetComponent<Rigidbody>();
        }

        void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
            _noise = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        void Update()
        {
            if (_rigid == null)
                return;

            var speed = _rigid.SpeedInKPH();

            _camera.m_Lens.FieldOfView = fovCurve.Evaluate(speed);

            if (_noise != null)
            {
                _noise.m_AmplitudeGain = amplitudeGainCurve.Evaluate(speed);
                _noise.m_FrequencyGain = frequencyGainCurve.Evaluate(speed);
            }
        }
    }
}