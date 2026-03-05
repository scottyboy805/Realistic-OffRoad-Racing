using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    [RequireComponent(typeof(WheelCollider))]
    public class Wheel : MonoBehaviour
    {
        public WheelAxle axle;
        public bool drive;
        public bool steer;
        public WheelVisual visual;
        [Range(-1, 1)] public float tyremarkWidthOffset = 0;

        public WheelCollider WheelCollider
        {
            get
            {
                if (_wheelCollider == null)
                {
                    _wheelCollider = GetComponent<WheelCollider>();
                }

                return _wheelCollider;
            }
        }

        public bool IsGrounded { get; private set; }
        public float Rpm { get; private set; }
        public WheelSurface CurrentSurface => _currentSurface;

        VehicleController _vc;
        WheelCollider _wheelCollider;
        WheelHit _hit;
        WheelFrictionCurve _forwardCurve, _sidewaysCurve;
        AudioSource _slipAudiosource;
        WheelSurface _currentSurface;
        Collider _currentCollider;
        Terrain _activeTerrain;
        Texture2D _lastTerrainTexture;
        Dictionary<WheelSurface, ParticleSystem> _particles = new Dictionary<WheelSurface, ParticleSystem>();
        int _lastMarkIndex;
        bool _invertAlignment;
        bool _slipEmission;

        void Start()
        {
            _vc = GetComponentInParent<VehicleController>();
            _invertAlignment = transform.localPosition.x < 0;
            _slipAudiosource = AudioHelper.CreateAudioSource(null, false, true, false, 0, 1, AudioGroup.Vehicle.ToString(), transform, "audio");
            _slipAudiosource.dopplerLevel = 0;

            WheelCollider.mass = WheelCollider.attachedRigidbody.mass * 0.1f;

            if (WheelSurfaceManager.Instance != null)
            {
                foreach (var s in WheelSurfaceManager.Instance.surfaces)
                {
                    if (s.wheelslipParticles != null)
                    {
                        var ps = Instantiate(s.wheelslipParticles, transform.position, Quaternion.identity, transform);
                        ps.Stop();
                        _particles.Add(s, ps);
                    }
                }
            }
        }

        void Update()
        {
            WheelCollider.GetGroundHit(out _hit);
            IsGrounded = WheelCollider.isGrounded;
            Rpm = WheelCollider.rpm;

            //surface detection mesh
            if (_hit.collider != _currentCollider)
            {
                _currentCollider = _hit.collider;
                _activeTerrain = null;
                _lastTerrainTexture = null;

                if (!_currentCollider.TryGetComponent<Terrain>(out _activeTerrain))
                {
                    _currentSurface = WheelSurfaceManager.Instance?.GetSurface(_currentCollider.sharedMaterial);
                    UpdateWheelProperties(_currentSurface);
                }
            }

            //surface detection terrain
            if (_activeTerrain != null)
            {
                var tex = _activeTerrain.GetTextureAtPosition(_hit.point);
                if (_lastTerrainTexture != tex)
                {
                    _lastTerrainTexture = tex;
                    _currentSurface = WheelSurfaceManager.Instance?.GetSurface(_lastTerrainTexture);
                    UpdateWheelProperties(_currentSurface);
                }
            }

            if (_currentSurface != null)
            {
                var canEmit = !WheelCollider.attachedRigidbody.isKinematic && WheelCollider.isGrounded &&
                              (ForwardSlip() >= _currentSurface.minSlip || SidewaysSlip() >= _currentSurface.minSlip);

                EmitParticles(canEmit);
                GenerateTyremarks(canEmit);
                PlaySounds(canEmit);
            }

            UpdateWheelAlignment();
        }

        void UpdateWheelAlignment()
        {
            WheelCollider.GetWorldPose(out var pos, out var rot);
            visual?.UpdatePosition(pos);
            visual?.UpdateRotation(WheelCollider.rpm);
            visual?.UpdateSteerAngle(WheelCollider.steerAngle);
        }

        void EmitParticles(bool emit)
        {
            if (_particles.TryGetValue(_currentSurface, out var ps))
            {
                var em = ps.emission;

                em.rateOverTime = emit ? _slipEmission ? Mathf.Lerp(0, 100, Mathf.Abs(SidewaysSlip() + ForwardSlip())) : 0 : 0;
                em.rateOverDistance = emit ? _slipEmission ? 0 : Mathf.Lerp(0, 10, _vc.CurrentSpeed / 50) : 0;

                if (emit)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
                else
                {
                    if (ps.isPlaying)
                    {
                        ps.Stop();
                    }
                }
            }
        }

        void GenerateTyremarks(bool emit)
        {
            if (emit)
            {
                var tyremarks = WheelSurfaceManager.Instance.GetTyremarksForSurface(_currentSurface);
                if (tyremarks != null)
                {
                    var intensity = Mathf.Abs(SidewaysSlip() + ForwardSlip());
                    var point = _hit.point + (WheelCollider.attachedRigidbody.linearVelocity * Time.fixedDeltaTime);
                    var width = tyremarkWidthOffset + ((visual?.GetWidth() ?? 1f) * 0.25f);
                    _lastMarkIndex = tyremarks.Add(point, _hit.normal, intensity * 0.5f, width, _lastMarkIndex);
                }
            }
            else
            {
                ResetTyremarkIndex();
            }
        }

        void PlaySounds(bool emit)
        {
            if (emit)
            {
                if (!_slipAudiosource.isPlaying)
                {
                    _slipAudiosource.Play();
                }

                _slipAudiosource.volume = _slipEmission ?
                    Mathf.Abs(_hit.sidewaysSlip) + Mathf.Abs(_hit.forwardSlip) :
                    Mathf.Lerp(0, 1, _vc.CurrentSpeed / 50);

                _slipAudiosource.pitch = 1 + Mathf.Abs(_hit.sidewaysSlip / 2);
                _slipAudiosource.pitch = Mathf.Clamp(_slipAudiosource.pitch, 1.0f, 1.2f);
            }
            else
            {
                _slipAudiosource.volume = Mathf.Lerp(_slipAudiosource.volume, 0, Time.deltaTime * 10);
            }
        }

        void UpdateWheelProperties(WheelSurface surface)
        {
            if (surface == null)
                return;

            UpdateForwardStiffness(surface.forwardGrip);
            UpdateSidewaysStiffness(surface.sidewaysGrip);
            WheelCollider.wheelDampingRate = surface.dampingRate;
            _slipAudiosource.clip = surface.wheelslipSound;
            _slipEmission = _currentSurface.emissionType == SurfaceEmissionType.Slip;
            ResetTyremarkIndex();

            foreach (var ps in _particles.Values)
            {
                if (ps.isPlaying)
                {
                    ps.Stop();
                }
            }
        }

        public void UpdateVisualAlignment(VisualWheelAlignment alignment)
        {
            UpdatePositionOffset(_invertAlignment ? alignment.center : -alignment.center);
            visual?.UpdateCamber(_invertAlignment ? -alignment.camberAngle : alignment.camberAngle);
            visual?.UpdateToe(_invertAlignment ? -alignment.toeAngle : alignment.toeAngle);
            visual?.UpdateWidth(alignment.width);
        }

        public void UpdateForwardFriction(WheelFriction friction)
        {
            _forwardCurve = WheelCollider.forwardFriction;

            _forwardCurve.extremumSlip = friction.extremumSlip;
            _forwardCurve.extremumValue = friction.extremumValue;
            _forwardCurve.asymptoteSlip = friction.asymptoteSlip;
            _forwardCurve.asymptoteValue = friction.asymptoteValue;

            WheelCollider.forwardFriction = _forwardCurve;
        }

        public void UpdateSidewaysFriction(WheelFriction friction)
        {
            _sidewaysCurve = WheelCollider.sidewaysFriction;

            _sidewaysCurve.extremumSlip = friction.extremumSlip;
            _sidewaysCurve.extremumValue = friction.extremumValue;
            _sidewaysCurve.asymptoteSlip = friction.asymptoteSlip;
            _sidewaysCurve.asymptoteValue = friction.asymptoteValue;

            WheelCollider.sidewaysFriction = _sidewaysCurve;
        }

        public void UpdateForwardStiffness(float value)
        {
            _forwardCurve = WheelCollider.forwardFriction;
            _forwardCurve.stiffness = value;
            WheelCollider.forwardFriction = _forwardCurve;
        }

        public void UpdateSidewaysStiffness(float value)
        {
            _sidewaysCurve = WheelCollider.sidewaysFriction;
            _sidewaysCurve.stiffness = value;
            WheelCollider.sidewaysFriction = _sidewaysCurve;
        }

        public void SetMotorTorque(float value)
        {
            if (!drive)
                return;

            WheelCollider.motorTorque = value;
        }

        public void SetSteerAngle(float value)
        {
            if (!steer)
                return;

            WheelCollider.steerAngle = value;
        }

        public void SetBrakeTorque(float value)
        {
            if (ForwardSlip() > 0.5f && Mathf.Abs(WheelCollider.rpm) < 1) //wheel locked
            {
                value = 0;
            }

            WheelCollider.brakeTorque = value;
        }

        public void UpdatePositionOffset(float value)
        {
            WheelCollider.center = new Vector3(-value, 0, 0);
            visual?.ResetLocalPosition();
        }

        public float ForwardSlip() => Mathf.Abs(_hit.forwardSlip);
        public float SidewaysSlip() => Mathf.Abs(_hit.sidewaysSlip);
        public void ResetTyremarkIndex() => _lastMarkIndex = -1;
    }
}