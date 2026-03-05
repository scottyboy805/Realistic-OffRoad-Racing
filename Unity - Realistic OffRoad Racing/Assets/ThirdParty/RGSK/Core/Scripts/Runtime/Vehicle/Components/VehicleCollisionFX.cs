using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class VehicleCollisionFX : VehicleComponent
    {
        AudioSource _collisionAudiosource;
        AudioSource _scrapeAudiosource;
        CollisionSurface _currentSurface;
        Dictionary<CollisionSurface, ParticleSystem> _particles = new Dictionary<CollisionSurface, ParticleSystem>();

        public override void Initialize(VehicleController vc)
        {
            base.Initialize(vc);

            _collisionAudiosource = AudioHelper.CreateAudioSource(null, false, false, false, 1, 1, AudioGroup.Vehicle.ToString(), vc.transform);
            _scrapeAudiosource = AudioHelper.CreateAudioSource(null, false, true, false, 1, 1, AudioGroup.Vehicle.ToString(), vc.transform);

            var surfaces = RGSKCore.Instance.VehicleSettings.collisionSurfaces;

            foreach (var s in surfaces)
            {
                if (s.hitParticles != null)
                {
                    var ps = GameObject.Instantiate(s.hitParticles, vc.transform.position, Quaternion.identity, vc.transform);
                    ps.Stop();
                    _particles.Add(s, ps);
                }
            }
        }

        public override void Update()
        {

        }

        public void CollisionEnter(Collision collision)
        {
            _currentSurface = RGSKCore.Instance.VehicleSettings.collisionSurfaces.FirstOrDefault
                (x => x.physicMaterial == collision.collider.sharedMaterial);

            if (_currentSurface == null)
            {
                _currentSurface = RGSKCore.Instance.VehicleSettings.fallbackCollisionSurface;
            }

            _collisionAudiosource.transform.localPosition =
            _scrapeAudiosource.transform.localPosition =
            Vehicle.transform.InverseTransformPoint(collision.contacts[0].point);

            _collisionAudiosource.clip = _scrapeAudiosource.clip = null;

            if (_currentSurface != null)
            {
                _collisionAudiosource.clip = _currentSurface.hitSounds.GetRandom();
                _scrapeAudiosource.clip = _currentSurface.scrapeSound;
            }

            _collisionAudiosource.volume = collision.relativeVelocity.magnitude * 0.1f;
            _collisionAudiosource.pitch = Random.Range(0.9f, 1.1f);
            _collisionAudiosource.Play();

            EmitParticles(collision);
            PlayerVehicleInput.Instance?.Rumble(Vehicle, collision.relativeVelocity.magnitude * 0.001f, 0.5f);
        }

        public void CollisionStay(Collision collision)
        {
            if (!_scrapeAudiosource.isPlaying)
            {
                _scrapeAudiosource.Play();
            }

            _scrapeAudiosource.pitch = Random.Range(0.9f, 1.1f);
            _scrapeAudiosource.volume = collision.relativeVelocity.magnitude * 0.1f;

            if (collision.relativeVelocity.magnitude > 2)
            {
                EmitParticles(collision);
            }
        }

        public void CollisionExit(Collision collision)
        {
            _scrapeAudiosource.Stop();
        }

        void EmitParticles(Collision collision)
        {
            if (_currentSurface == null || _particles.Count == 0)
                return;

            if (_particles.TryGetValue(_currentSurface, out var ps))
            {
                ps.transform.position = collision.contacts[0].point;
                ps.Play();
            }
        }
    }
}