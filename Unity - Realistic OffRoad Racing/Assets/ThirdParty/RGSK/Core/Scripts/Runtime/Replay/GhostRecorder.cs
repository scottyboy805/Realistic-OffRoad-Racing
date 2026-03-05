using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGSK.Helpers;
using System.Linq;

namespace RGSK
{
    public class GhostRecorder : Recorder
    {
        [SerializeField] Vector3 hidePosition = new Vector3(0, -10000, 0);

        GameObject _ghost;
        RecordableObject _recordableObject;

        public void CreateGhost(GameObject referenceObj, GameObject cloneObj)
        {
            if (recordableObjects.Count > 0)
                return;

            StartCoroutine(CreateGhostRoutine(referenceObj, cloneObj));
        }

        IEnumerator CreateGhostRoutine(GameObject referenceObj, GameObject cloneObj)
        {
            AddRecordableObject(referenceObj);

            _ghost = Instantiate(cloneObj, hidePosition, Quaternion.identity, GeneralHelper.GetDynamicParent());
            _ghost.name = $"[Ghost] {referenceObj.name}";

            yield return null;

            if (GeneralHelper.CanApplyColor(referenceObj))
            {
                GeneralHelper.SetColor(_ghost.gameObject, GeneralHelper.GetColor(referenceObj), 0);
            }
            else if (GeneralHelper.CanApplyLivery(referenceObj))
            {
                GeneralHelper.SetLivery(_ghost.gameObject, GeneralHelper.GetLiveryIndex(referenceObj));
            }

            GeneralHelper.ToggleVehicleCollision(_ghost.gameObject, false);
            GeneralHelper.ToggleGhostedMesh(_ghost.gameObject, true);

            UpdateAndCleanComponents();

            _recordableObject = new RecordableObject
            {
                gameObject = _ghost,
                rigidbody = _ghost.GetComponent<Rigidbody>(),
                vehicle = _ghost.GetComponent<IVehicle>()
            };

            _recordableObject.AllocateFrames();
            HideGhost();
        }

        protected override void Playback()
        {
            if (_recordableObject == null)
                return;

            if (CurrentFrame >= GetTotalFrames(_recordableObject))
            {
                StopPlayback();
                return;
            }

            CurrentFrame += PlaybackSpeed;
            _recordableObject.PlayFrame(CurrentFrame);
        }

        public override void StartPlayback()
        {
            if (_recordableObject == null)
                return;

            StopPlayback();
            DeleteRecordedData();
            base.StartPlayback();
        }

        public override void StopPlayback()
        {
            base.StopPlayback();
            HideGhost();
        }

        public void CacheReplayData()
        {
            if (recordableObjects.Count == 0 || _recordableObject == null)
                return;

            if (recordableObjects[0].FrameData.Count > 0)
            {
                _recordableObject.DeleteFrameData();

                foreach (var data in recordableObjects[0].FrameData)
                {
                    _recordableObject.FrameData.Add(data);
                }
            }
        }

        public void DeleteGhostData()
        {
            if (_recordableObject == null)
                return;

            _ghost = null;
            _recordableObject.gameObject = null;
            _recordableObject.rigidbody = null;
            _recordableObject.vehicle = null;
            _recordableObject.DeleteFrameData();
        }

        void UpdateAndCleanComponents()
        {
            if (_ghost == null)
                return;

            var audio = _ghost.GetComponentsInChildren<AudioSource>(true).ToList();
            var particles = _ghost.GetComponentsInChildren<ParticleSystem>(true).ToList();
            var rbs = _ghost.GetComponentsInChildren<Rigidbody>(true).ToList();
            var lights = _ghost.GetComponentsInChildren<Light>(true).ToList();

            audio.ForEach(x => x.mute = true);
            particles.ForEach(x => x.gameObject.SetActive(false));
            rbs.ForEach(x => x.isKinematic = true);

            lights.ForEach(x =>
            {
                x.gameObject.SetActive(false);
                x.enabled = false;
                x.range = 0;
            });

            if (_ghost.TryGetComponent<RGSKEntity>(out var entity))
            {
                Destroy(entity);
            }

            if (_ghost.TryGetComponent<RecordableSceneObject>(out var recordable))
            {
                Destroy(recordable);
            }
        }

        void HideGhost()
        {
            if (_recordableObject == null || _ghost == null)
                return;

            _recordableObject.gameObject.transform.SetPositionAndRotation(hidePosition, Quaternion.identity);
            _recordableObject.rigidbody.isKinematic = true;
        }
    }
}