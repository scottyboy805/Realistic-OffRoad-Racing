using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class Track : MonoBehaviour
    {
        [ExposedScriptableObject] public TrackDefinition definition;

        [Tooltip("How long the AI should maintain their starting position offset.")]
        public float raceStartOffsetDuration = 10;

        public CheckpointRoute CheckpointRoute
        {
            get
            {
                if (_checkpointRoute == null)
                    _checkpointRoute = GetComponentInChildren<CheckpointRoute>();

                return _checkpointRoute;
            }
        }

        public CameraRoute CameraRoute
        {
            get
            {
                if (_cameraRoute == null)
                    _cameraRoute = GetComponentInChildren<CameraRoute>();

                return _cameraRoute;
            }
        }

        CheckpointRoute _checkpointRoute;
        CameraRoute _cameraRoute;

        public void Setup()
        {
            CheckpointRoute?.SetupCheckpoints();
            CameraManager.Instance?.SetRouteCameras(CameraRoute);
        }

        public AIRoute GetAIRoute(int index)
        {
            var routes = GetComponentsInChildren<AIRoute>().ToList();

            if (routes.Count > 0)
            {
                routes = routes.OrderByDescending(x => x.priority).ToList();
                return routes[index];
            }

            return null;
        }

        public RaceGrid GetRaceGrid(RaceStartMode type)
        {
            var grids = GetComponentsInChildren<RaceGrid>().ToList();

            if (grids.Count > 0)
            {
                return grids.FirstOrDefault(x => x.gridType == type);
            }

            return null;
        }

        public bool IsPointToPoint() => !CheckpointRoute?.loop ?? false;
        public float GetBestLap() => definition?.LoadBestLap() ?? 0;
    }
}