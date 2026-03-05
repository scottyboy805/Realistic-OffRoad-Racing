using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class RaceCatchupManager : MonoBehaviour
    {
        CatchupSettings _settings => RGSKCore.Instance.GeneralSettings.catchupSettings;
        RGSKEntity _playerEntity;

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
            RGSKEvents.OnEntityAdded.AddListener(OnEntityAdded);
        }

        void OnDisable()
        {
            RGSKEvents.OnRaceInitialized.RemoveListener(OnRaceInitialized);
            RGSKEvents.OnEntityAdded.RemoveListener(OnEntityAdded);
        }

        void OnRaceInitialized()
        {
            if (_settings != null)
            {
                _settings.enabled = RaceManager.Instance.Session.enableCatchup;
            }
        }

        void OnEntityAdded(RGSKEntity entity)
        {
            if (entity.IsPlayer)
            {
                _playerEntity = entity;
            }
        }

        void FixedUpdate()
        {
            if (_settings == null || !_settings.enabled || _playerEntity == null)
                return;

            RaceManager.Instance.Entities.Items.ForEach(x =>
            {
                if (_playerEntity.Competitor != null && !x.IsPlayer &&
                    x.Competitor != null && x.Competitor.IsRacing())
                {
                    ApplyEffect(x);
                }
            });
        }

        void ApplyEffect(RGSKEntity entity)
        {
            if (entity.Rigid != null && entity.Vehicle != null)
            {
                var factor = GetDistanceFactor(entity);

                var isAhead = entity.Competitor.Position < _playerEntity.Competitor.Position;

                var direction = isAhead ? Vector3.back : Vector3.forward;

                var strength = isAhead ? _settings.slowDownStrength * factor :
                                         _settings.speedUpStrength * factor;

                entity.Rigid.AddRelativeForce(direction * strength * entity.Vehicle.ThrottleInput, ForceMode.Acceleration);
            }
        }

        float GetDistanceFactor(RGSKEntity other)
        {
            if (_playerEntity == null)
                return 0;

            return Mathf.InverseLerp(_settings.effectRange.x,
                                     _settings.effectRange.y,
                                     Mathf.Abs(GeneralHelper.GetDistanceGapBetween
                                     (_playerEntity.Competitor, other.Competitor)));
        }

        float GetPositionFactor(int position)
        {
            return ((float)position / (float)RaceManager.Instance.ActiveCompetitorCount());
        }
    }
}