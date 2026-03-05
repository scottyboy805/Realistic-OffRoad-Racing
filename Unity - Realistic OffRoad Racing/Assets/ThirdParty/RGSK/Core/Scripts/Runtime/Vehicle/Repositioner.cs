using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Extensions;
using System.Linq;
using RGSK.Helpers;

namespace RGSK
{
    public class Repositioner : RGSKEntityComponent
    {
        RepositionSettings _settings => RGSKCore.Instance.GeneralSettings.repositionSettings;
        Vector3 _size = new Vector3(2, 1, 4);
        List<Renderer> _renderers;
        Dictionary<Renderer, bool> _rendererActiveStates = new Dictionary<Renderer, bool>();
        float _upsideDownTimer;
        float _flickerRate;
        bool _canReposition;

        public override void Initialize(RGSKEntity e)
        {
            base.Initialize(e);
            var box = GetComponentInChildren<BoundingBox>();
            if (box != null)
            {
                _size = box.Size;
            }

            _canReposition = true;
        }

        void Update()
        {
            UpsideDownCheck();
        }

        void UpsideDownCheck()
        {
            if (_settings == null || !_settings.repositionIfUpsideDown)
                return;

            if (transform.IsUpsideDown() && Entity.CurrentSpeed < _settings.updsideDownMinSpeed)
            {
                _upsideDownTimer += Time.deltaTime;

                if (_upsideDownTimer > _settings.updsideDownWaitTime)
                {
                    Reposition();
                    _upsideDownTimer = 0;
                }
            }
            else
            {
                _upsideDownTimer = 0;
            }
        }

        public void Reposition()
        {
            if (_settings == null || !_canReposition)
                return;

            if (_renderers == null)
            {
                _renderers = GetComponentsInChildren<Renderer>().Where(x => !x.GetComponent<ParticleSystem>()).ToList();
            }

            var pos = transform.position;
            var rot = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            if (Entity.Competitor != null)
            {
                if (Entity.Competitor.RaceState == RaceState.Countdown)
                {
                    return;
                }

                if (Entity.Competitor.IsRacing())
                {
                    if (Entity.Competitor.TotalCheckpointsPassed > 0)
                    {
                        if (Entity.Competitor.PreviousCheckpoint != null)
                        {
                            pos = Entity.Competitor.PreviousCheckpoint.transform.position;

                            if (Physics.Raycast(pos, Vector3.down, out var hit))
                            {
                                pos = hit.point;
                            }

                            switch (_settings.raceRepositionRotationOption)
                            {
                                case RaceRepositionRotationOptions.NextCheckpointDirection:
                                    {
                                        if (Entity.Competitor.NextCheckpoint != null)
                                        {
                                            var dir = Entity.Competitor.NextCheckpoint.transform.position -
                                                      Entity.Competitor.PreviousCheckpoint.transform.position;

                                            rot = Quaternion.LookRotation(dir);
                                        }
                                        break;
                                    }

                                case RaceRepositionRotationOptions.PreviousCheckpointRotation:
                                    {
                                        rot = Entity.Competitor.PreviousCheckpoint.transform.rotation;
                                        break;
                                    }
                            }
                        }

                        Entity.Competitor.LapTracker.UpdateTotalPercentage(Entity.Competitor.CurrentLap - 1);
                        Entity.Competitor.FurthestDistanceTravelled = Entity.Competitor.PreviousCheckpoint.distance;
                        Entity.Competitor.WrongwayTracker?.SetFurthestDistance(Entity.Competitor.FurthestDistanceTravelled);
                    }
                }
            }

            transform.position = pos + _settings.repositionOffset;
            transform.rotation = rot;

            Entity.Vehicle?.OnReset();
            Entity.Rigid?.ResetVelocity();

            if (TryGetComponent<AIController>(out var ai))
            {
                ai.LookForClosestNodes();
            }

            StopAllCoroutines();
            StartCoroutine(GhostedRoutine());
        }

        IEnumerator GhostedRoutine()
        {
            _canReposition = false;

            var timer = 0f;
            var overlapping = false;
            GeneralHelper.ToggleGhostedMesh(gameObject, _settings.ghostMesh);

            _rendererActiveStates.Clear();
            foreach (var r in _renderers)
            {
                _rendererActiveStates.Add(r, r.enabled);
            }

            while (timer < _settings.ghostedDuration)
            {
                GeneralHelper.ToggleVehicleCollision(gameObject, false);

                var overlaps = Physics.OverlapBox(
                                transform.position,
                                _size / 2,
                                Quaternion.identity,
                                _settings.safeCheckLayers,
                                QueryTriggerInteraction.Ignore);

                overlapping = overlaps.Length > 0;

                if (!overlapping)
                {
                    timer += Time.deltaTime;
                }

                Flicker();
                yield return null;
            }

            EndFlicker();

            if (RaceManager.Instance != null && RaceManager.Instance.Initialized)
            {
                GeneralHelper.ToggleVehicleCollision(gameObject, !RaceManager.Instance.Session.disableCollision);
                GeneralHelper.ToggleGhostedMesh(gameObject, !Entity.IsPlayer && RaceManager.Instance.Session.disableCollision);
            }
            else
            {
                GeneralHelper.ToggleVehicleCollision(gameObject, true);
                GeneralHelper.ToggleGhostedMesh(gameObject, false);
            }

            _canReposition = true;
        }

        void Flicker()
        {
            if (!_settings.flickerMesh)
                return;

            _flickerRate = Mathf.Repeat(Time.time * _settings.flickerSpeed, 2);

            foreach (var r in _renderers)
            {
                if (_rendererActiveStates[r] == true)
                {
                    r.enabled = _flickerRate < 1;
                }
            }
        }

        void EndFlicker()
        {
            if (!_settings.flickerMesh)
                return;

            foreach (var r in _renderers)
            {
                r.enabled = _rendererActiveStates[r];
            }
        }
    }
}