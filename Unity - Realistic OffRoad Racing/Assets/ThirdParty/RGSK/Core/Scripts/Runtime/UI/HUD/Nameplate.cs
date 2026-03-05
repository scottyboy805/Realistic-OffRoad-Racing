using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RGSK.Extensions;
using RGSK.Helpers;

namespace RGSK
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Nameplate : EntityUIController
    {
        [SerializeField]
        AnimationCurve distanceAlphaCurve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(30, 1),
            new Keyframe(50, 0));

        CanvasGroup _canvas;
        CinemachineBrain _camera;
        bool _inReplay;

        protected override void OnEnable()
        {
            base.OnEnable();
            RGSKEvents.OnEntityRemoved.AddListener(OnEntityRemoved);
            RGSKEvents.OnReplayStart.AddListener(OnReplayStart);
            RGSKEvents.OnReplayStop.AddListener(OnReplayStop);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RGSKEvents.OnEntityRemoved.RemoveListener(OnEntityRemoved);
            RGSKEvents.OnReplayStart.RemoveListener(OnReplayStart);
            RGSKEvents.OnReplayStop.RemoveListener(OnReplayStop);
        }

        public override void BindElements(RGSKEntity e)
        {
            base.BindElements(e);

            if (TryGetComponent<TransformFollower>(out var follower))
            {
                follower.target = e.transform;
            }
        }

        void Awake()
        {
            _camera = CameraManager.Instance?.Camera;
            _canvas = GetComponent<CanvasGroup>();

            if (_camera != null)
            {
                if (TryGetComponent<Canvas>(out var c))
                {
                    c.worldCamera = _camera.OutputCamera;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_camera != null)
            {
                if (!RGSKCore.Instance.UISettings.showNameplates || _inReplay)
                {
                    _canvas.SetAlpha(0);
                    return;
                }

                var dist = Vector3.Distance(_camera.transform.position, transform.position);
                _canvas.SetAlpha(entity == GeneralHelper.GetFocusedEntity() ? 0 : distanceAlphaCurve.Evaluate(dist));
            }
        }

        void OnEntityRemoved(RGSKEntity e)
        {
            if (e != entity)
                return;

            Destroy(gameObject);
        }

        void OnReplayStart()
        {
            _inReplay = true;
        }

        void OnReplayStop()
        {
           _inReplay = false;
        }
    }
}